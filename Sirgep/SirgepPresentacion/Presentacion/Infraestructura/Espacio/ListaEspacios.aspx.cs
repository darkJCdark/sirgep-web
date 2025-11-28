using DocumentFormat.OpenXml.Wordprocessing;
using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace SirgepPresentacion.Presentacion.Infraestructura.Espacio
{
    public partial class ListaEspacios : System.Web.UI.Page
    {
        private EspacioWS espacioWS;
        private DistritoWS distritoWS;
        private DepartamentoWS departamentoWS;
        private ProvinciaWS provinciaWS;
        private EspacioDiaSemWS diaSemWS;
        private int PaginaActual
        {
            get => ViewState["PaginaActual"] != null ? (int)ViewState["PaginaActual"] : 1;
            set => ViewState["PaginaActual"] = value;
        }

        private const int TAM_PAGINA = 10;
        private int TotalPaginas = 1; // se calcula en CargarEspacios()
        private const int AGREGAR_ESPACIO = 1; // código para abrir el modal de "Agregar Espacio" al final de las validaciones
        private const int EDICION_ESPACIO = 2; // para abrir el omdal de edición al finalizar las validaciones

        protected void Page_Init(object sender, EventArgs e)
        {
            espacioWS = new EspacioWSClient();
            distritoWS = new DistritoWSClient();
            departamentoWS = new DepartamentoWSClient();
            provinciaWS = new ProvinciaWSClient();
            diaSemWS = new EspacioDiaSemWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                incializarHoras();
                PaginaActual = 1;
                CargarEspacios();
                CargarDistritos();
                CargarDepas();
            }
        }

        public void incializarHoras()
        {
            ddlHoraInicioInsert.Items.Clear();
            ddlHoraInicioInsert.Items.Add(new ListItem("Hora:minutos", ""));
            ddlHoraFinInsert.Items.Clear();
            ddlHoraFinInsert.Items.Add(new ListItem("Hora:minutos", ""));
            ddlHoraInicioEdit.Items.Clear();
            ddlHoraInicioEdit.Items.Add(new ListItem("Hora:minutos", ""));
            ddlHoraFinEdit.Items.Clear();
            ddlHoraFinEdit.Items.Add(new ListItem("Hora:minutos", ""));
            for (int i = 0; i < 24; i++)
            {
                string hora = i.ToString("D2") + ":00";
                ddlHoraInicioInsert.Items.Add(new ListItem(hora, hora));
                ddlHoraFinInsert.Items.Add(new ListItem(hora, hora));
                ddlHoraInicioEdit.Items.Add(new ListItem(hora, hora));
                ddlHoraFinEdit.Items.Add(new ListItem(hora, hora));
            }
        }
        public void CargarDepas()
        {
            listarDepasResponse responseDepas = departamentoWS.listarDepas(new listarDepasRequest());
            ddlDepartamentoAgregar.DataSource = responseDepas.@return;
            ddlDepartamentoAgregar.DataTextField = "Nombre";
            ddlDepartamentoAgregar.DataValueField = "IdDepartamento";
            ddlDepartamentoAgregar.DataBind();

            ddlDepartamentoEdit.DataSource = responseDepas.@return;
            ddlDepartamentoEdit.DataTextField = "Nombre";
            ddlDepartamentoEdit.DataValueField = "IdDepartamento";
            ddlDepartamentoEdit.DataBind();

            /* Hardcodeado pq' no lo tenemos en nuestra BD, además hay pocos datos */
            ddlTipoEspacioAgregar.Items.Insert(0, new ListItem("Seleccione un tipo", ""));
            ddlTipoEspacioAgregar.Items.Insert(1, new ListItem("Teatros", "TEATRO"));
            ddlTipoEspacioAgregar.Items.Insert(2, new ListItem("Canchas", "CANCHA"));
            ddlTipoEspacioAgregar.Items.Insert(3, new ListItem("Salones", "SALON"));
            ddlTipoEspacioAgregar.Items.Insert(4, new ListItem("Parques", "PARQUE"));

            if (ddlProvinciaAgregar.Items.Count == 0)
                ddlProvinciaAgregar.Items.Insert(0, new ListItem("Seleccione una provincia", ""));
            if (ddlDistritoAgregar.Items.Count == 0)
                ddlDistritoAgregar.Items.Insert(0, new ListItem("Seleccione un distrito", ""));
        }
        protected void Paginar_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Anterior")
            {
                PaginaActual--;
                if (!string.IsNullOrEmpty(txtBusqueda.Text))
                {
                    txtBusqueda_TextChanged(sender, e);
                    return;
                }
            }
            if (e.CommandName == "Siguiente")
            {
                PaginaActual++;
                if (!string.IsNullOrEmpty(txtBusqueda.Text))
                {
                    txtBusqueda_TextChanged(sender, e);
                    return;
                }
            }

            CargarEspacios();
        }
        private void CargarDistritos()
        {
            ddlDistrito.DataSource = distritoWS.listarTodosDistritos(new listarTodosDistritosRequest()).@return;
            ddlDistrito.DataTextField = "Nombre";
            ddlDistrito.DataValueField = "IdDistrito";
            ddlDistrito.DataBind();
            if (ddlDistrito.Items[0].Text != "Seleccione un distrito")
                ddlDistrito.Items.Insert(0, new ListItem("Seleccione un distrito", ""));
        }
        public void realizarPaginado(espacio[] response)
        {
            if (response == null)
            {
                mostrarModalErrorEsp("RESULTADO DE BUSQUEDA", "No se encontraron espacios con los parámetros actuales; se listarán todos los espacios...");
                CargarEspacios();
                return;
            }

            var todos = response.ToList(); // lo convertimos a lista
            CalcularTotalPaginas(todos.Count);

            AjustarPaginaActual();

            var paginaActual = ObtenerPaginaActual(todos);

            rptEspacios.DataSource = paginaActual;
            rptEspacios.DataBind();

            ActualizarFooter();
        }
        private void CalcularTotalPaginas(int totalEspacios)
        {
            TotalPaginas = (int)Math.Ceiling((double)totalEspacios / TAM_PAGINA);
        }

        private void AjustarPaginaActual()
        {
            if (PaginaActual < 1) PaginaActual = 1;
            if (PaginaActual > TotalPaginas) PaginaActual = TotalPaginas;
        }

        private List<espacio> ObtenerPaginaActual(List<espacio> todos)
        {
            return todos.Skip((PaginaActual - 1) * TAM_PAGINA).Take(TAM_PAGINA).ToList();
        }

        private void ActualizarFooter()
        {
            if (rptEspacios.Controls.Count > 0)
            {
                var footer = rptEspacios.Controls[rptEspacios.Controls.Count - 1];

                var lblPagina = footer.FindControl("lblPaginaFoot") as Label;
                if (lblPagina != null)
                    lblPagina.Text = $"Página {PaginaActual} / {TotalPaginas}";

                var btnAnterior = footer.FindControl("btnAnteriorFoot") as Button;
                var btnSiguiente = footer.FindControl("btnSiguienteFoot") as Button;

                if (btnAnterior != null)
                    btnAnterior.Enabled = PaginaActual > 1;

                if (btnSiguiente != null)
                    btnSiguiente.Enabled = PaginaActual < TotalPaginas;
            }
        }

        private void CargarEspacios()
        {
            listarEspacioResponse response = espacioWS.listarEspacio(new listarEspacioRequest());
            espacio[] espacios = response.@return;
            realizarPaginado(espacios);
        }
        protected void btnAgregarEspacio_Click(object sender, EventArgs e)
        {
            // Arriba a la derecha [el botón verde] que abre el modal para el agregado...

            // Limpiar campos si es necesario
            txtNombreEspacioAgregar.Text = "";
            txtUbicacionAgregar.Text = "";
            txtSuperficieAgregar.Text = "";
            txtPrecioReservaAgregar.Text = "";
            ddlHoraFinInsert.Text = "";
            ddlHoraInicioInsert.Text = "";

            string test = ddlDepartamentoAgregar.Items[0].ToString();
            if (ddlDepartamentoAgregar.Items[0].ToString() != "Escoja un departamento")
                ddlDepartamentoAgregar.Items.Insert(0, new ListItem("Escoja un departamento", ""));

            // Asegurarnos de que apunten al primer index
            ddlTipoEspacioAgregar.SelectedIndex = 0;
            ddlDepartamentoAgregar.SelectedIndex = 0;
            ddlProvinciaAgregar.SelectedIndex = 0;
            ddlDistritoAgregar.SelectedIndex = 0;

            // Mostrar el modal usando JavaScript
            abrirModalAgregarEspacio();
        }
        public void MarcarDiasCheck(espacioDiaSem[] dias)
        {
            // Crear una tabla hash con los días atendidos (en mayúsculas para estandarizar)
            HashSet<string> diasAtendidos = new HashSet<string>();
            foreach (espacioDiaSem e in dias)
            {
                diasAtendidos.Add(e.dia.ToString().ToUpperInvariant());
            }

            // Marcar los checkboxes si el día está en el HashSet
            lunesEdit.Checked = diasAtendidos.Contains("LUNES");
            martesEdit.Checked = diasAtendidos.Contains("MARTES");
            miercolesEdit.Checked = diasAtendidos.Contains("MIERCOLES");
            juevesEdit.Checked = diasAtendidos.Contains("JUEVES");
            viernesEdit.Checked = diasAtendidos.Contains("VIERNES");
            sabadoEdit.Checked = diasAtendidos.Contains("SABADO");
            domingoEdit.Checked = diasAtendidos.Contains("DOMINGO");
        }
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            int idEspacio = int.Parse(((Button)sender).CommandArgument.ToString());
            hiddenIdEspacio.Value = idEspacio.ToString();

            espacioDTO espDTO = espacioWS.obtenerEspacioDTO(new obtenerEspacioDTORequest(idEspacio)).@return;
            hiddenIdDistrito.Value = espDTO.idDistrito.ToString();

            CargarTipoEspacio(espDTO.tipo.ToString());
            CargarDatosEspacio(espDTO);
            CargarUbicacion(espDTO);
            CargarHorario(espDTO);
            MarcarDiasCheck(espDTO.dias);

            abrirModalEditarEspacio();
        }
        private void CargarTipoEspacio(string tipoSeleccionado)
        {
            ddlTipoEspacioEdit.Items.Clear();
            ddlTipoEspacioEdit.Items.Insert(0, new ListItem(tipoSeleccionado, tipoSeleccionado));
            ddlTipoEspacioEdit.Items.Insert(1, new ListItem("Teatros", "TEATRO"));
            ddlTipoEspacioEdit.Items.Insert(2, new ListItem("Canchas", "CANCHA"));
            ddlTipoEspacioEdit.Items.Insert(3, new ListItem("Salones", "SALON"));
            ddlTipoEspacioEdit.Items.Insert(4, new ListItem("Parques", "PARQUE"));
        }

        private void CargarDatosEspacio(espacioDTO espDTO)
        {
            txtNombreEdit.Text = espDTO.nombre;
            txtUbicacionEdit.Text = espDTO.ubicacion;
            txtSuperficieEdit.Text = espDTO.superficie.ToString();
            txtPrecioEdit.Text = espDTO.precioReserva.ToString();
        }

        private void CargarUbicacion(espacioDTO espDTO)
        {
            ddlDepartamentoEdit.SelectedValue = espDTO.idDepartamento.ToString();
            ddlDepartamentoEdit.Enabled = false;

            ddlProvinciaEdit.DataTextField = "Nombre";
            ddlProvinciaEdit.DataValueField = "IdProvincia";
            ddlProvinciaEdit.Items.Insert(0, new ListItem(espDTO.nombreProvincia.ToString(), espDTO.idProvincia.ToString()));
            ddlProvinciaEdit.SelectedValue = espDTO.idProvincia.ToString();
            ddlProvinciaEdit.Enabled = false;

            ddlDistritoEdit.DataTextField = "Nombre";
            ddlDistritoEdit.DataValueField = "IdDistrito";
            ddlDistritoEdit.Items.Insert(0, new ListItem(espDTO.nombreDistrito.ToString(), espDTO.idDistrito.ToString()));
            ddlDistritoEdit.SelectedValue = espDTO.idDistrito.ToString();
            ddlDistritoEdit.Enabled = false;
        }

        private void CargarHorario(espacioDTO espDTO)
        {
            ddlHoraInicioEdit.Text = espDTO.horaInicio.ToString().Substring(0, 5);
            ddlHoraFinEdit.Text = espDTO.horaFin.ToString().Substring(0, 5);
        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            // Text ya es el VALOR y es un string
            string filtroCat = ddlCategoria.Text;
            string filtroDist = ddlDistrito.Text;
            if (filtroCat != "" && filtroDist != "")
            {
                // llamamos a la filtración doble implementada en backend
                listarEspacioDistyCatResponse response = null;
                response = espacioWS.listarEspacioDistyCat(new listarEspacioDistyCatRequest(int.Parse(filtroDist), filtroCat));
                realizarPaginado(response.@return);
            }
            else if (filtroCat != "")
            {
                listarEspacioPorCategoriaResponse responseFiltroCat = espacioWS.listarEspacioPorCategoria(new listarEspacioPorCategoriaRequest(filtroCat));
                realizarPaginado(responseFiltroCat.@return);
            }
            else if (filtroDist != "")
            {
                int idDistrito = int.Parse(filtroDist);
                listarEspacioPorDistritoResponse responseFiltroDist = espacioWS.listarEspacioPorDistrito(new listarEspacioPorDistritoRequest(idDistrito));
                realizarPaginado(responseFiltroDist.@return);
            }
            else
            {
                CargarEspacios();
            }
            // el último caso, es por si no se quieren filtros
        }
        protected void btnConfirmarAccion_Click(object sender, EventArgs e)
        {
            int id = int.Parse(hdnIdAEliminar.Value);

            eliminarLogicoResponse response = espacioWS.eliminarLogico(new eliminarLogicoRequest(id));

            Boolean estado = response.@return;

            if (estado)
            {
                // debemos eliminar también los días de la semana del espacio
                Boolean diaEliminado = diaSemWS.eliminarDiasPorEspacio(new eliminarDiasPorEspacioRequest(id)).@return;
                if (diaEliminado)
                {
                    mostrarModalExitoEsp("ÉXITO AL ELIMINAR EL ESPACIO", "Los días del espacio y el espacio mismo han sido eliminados satisfactoriamente.");
                    CargarEspacios(); // Refresca la tabla
                }
                else
                {
                    mostrarModalErrorEsp("FALLO AL ELIMINAR Dias del Espacio", "Ocurrió un error al momento de eliminar los días del espacio.");
                }

            }
            else
            {
                mostrarModalErrorEsp("FALLO AL ELIMINAR ESPACIO", "Ocurrió un error al momento de eliminar el espacio.");
            }
        }
        protected void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            buscarEspacioPorTextoResponse response = espacioWS.buscarEspacioPorTexto(new buscarEspacioPorTextoRequest(txtBusqueda.Text));
            espacio[] espacios = response.@return;
            realizarPaginado(espacios);
        }

        protected void ddlDepartamentoAgregar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlDepartamentoAgregar.SelectedValue))
            {
                int idDepartamento = int.Parse(ddlDepartamentoAgregar.SelectedValue);
                listarProvinciaPorDepaResponse responseProvincia = provinciaWS.listarProvinciaPorDepa(
                    new listarProvinciaPorDepaRequest(idDepartamento)
                );

                ddlProvinciaAgregar.DataSource = responseProvincia.@return;
                ddlProvinciaAgregar.DataTextField = "Nombre";
                ddlProvinciaAgregar.DataValueField = "IdProvincia";
                ddlProvinciaAgregar.DataBind();
                ddlProvinciaAgregar.Items.Insert(0, new ListItem("Seleccione una provincia", ""));
                string lblErrorDepa = lblError.Text;
                if (!string.IsNullOrEmpty(lblErrorDepa) && lblErrorDepa.Contains("departamento")) lblError.Text = "";
            }
            else
            {
                ddlProvinciaAgregar.Items.Clear();
                ddlProvinciaAgregar.Items.Insert(0, new ListItem("Seleccione un departamento primero", ""));
            }
            // Para mantener el modal abierto tras el postback
            abrirModalAgregarEspacio();
        }

        protected void ddlProvinciaAgregar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlProvinciaAgregar.SelectedValue))
            {
                int idProvincia = int.Parse(ddlProvinciaAgregar.SelectedValue);
                listarDistritosFiltradosResponse responseDistrito = distritoWS.listarDistritosFiltrados(
                    new listarDistritosFiltradosRequest(idProvincia)
                );

                ddlDistritoAgregar.DataSource = responseDistrito.@return;
                ddlDistritoAgregar.DataTextField = "Nombre";
                ddlDistritoAgregar.DataValueField = "IdDistrito";
                ddlDistritoAgregar.DataBind();
                ddlDistritoAgregar.Items.Insert(0, new ListItem("Seleccione un distrito", ""));
                string lblErrorProv = lblError.Text;
                if (!string.IsNullOrEmpty(lblErrorProv) && lblErrorProv.Contains("provincia")) lblError.Text = "";
            }
            else
            {
                ddlDistritoAgregar.Items.Clear();
                ddlDistritoAgregar.Items.Insert(0, new ListItem("Seleccione una provincia primero", ""));
            }

            // Para que el modal siga abierto
            abrirModalAgregarEspacio();
        }
        protected void btnActualizarEspacioEdit_Click(object sender, EventArgs e)
        {
            if (!EsFormularioEspacioValido()) return;

            int idEspacio = int.Parse(hiddenIdEspacio.Value);
            int idDistrito = int.Parse(hiddenIdDistrito.Value);
            espacio espacioActualizar = ConstruirEspacioActualizado(idEspacio, idDistrito);

            bool actualizado = espacioWS.actualizarEspacio(new actualizarEspacioRequest(espacioActualizar)).@return;

            MostrarResultadoActualizacion(actualizado);
        }
        private bool EsFormularioEspacioValido()
        {
            return validarEspacio(
                txtNombreEdit.Text,
                txtUbicacionEdit.Text,
                txtSuperficieEdit.Text,
                txtPrecioEdit.Text,
                ddlTipoEspacioEdit.SelectedValue,
                ddlHoraInicioEdit.Text,
                ddlHoraFinEdit.Text,
                ddlDepartamentoEdit.Text,
                ddlProvinciaEdit.Text,
                ddlDistritoEdit.Text,
                diasSeleccionados.Value,
                EDICION_ESPACIO
            );
        }

        private espacio ConstruirEspacioActualizado(int idEspacio, int idDistrito)
        {
            return new espacio()
            {
                idEspacio = idEspacio,
                nombre = txtNombreEdit.Text.Trim(),
                ubicacion = txtUbicacionEdit.Text.Trim(),
                superficie = ObtenerDouble(txtSuperficieEdit.Text),
                precioReserva = ObtenerDouble(txtPrecioEdit.Text),
                tipoEspacio = ParsearTipoEspacio(ddlTipoEspacioEdit.SelectedValue),
                tipoEspacioSpecified = true,
                horarioInicioAtencion = NormalizarHora(ddlHoraInicioEdit.Text),
                horarioFinAtencion = NormalizarHora(ddlHoraFinEdit.Text),
                distrito = new distrito { idDistrito = idDistrito }
            };
        }

        private double ObtenerDouble(string texto)
        {
            return double.TryParse(texto, out double valor) ? valor : 0.0;
        }

        private eTipoEspacio ParsearTipoEspacio(string valor)
        {
            Enum.TryParse(valor, out eTipoEspacio tipo);
            return tipo;
        }

        private string NormalizarHora(string hora)
        {
            return hora.Length == 5 ? hora + ":00" : hora;
        }

        private void MostrarResultadoActualizacion(bool actualizado)
        {
            if (actualizado)
            {
                mostrarModalExitoEsp("ACTUALIZACIÓN EXITOSA", "Espacio actualizado exitosamente.");
                CargarEspacios();
            }
            else
            {
                mostrarModalErrorEsp("ERROR AL ACTUALIZAR ESPACIO", "Ocurrió un error al momento de actualizar el espacio.");
            }
        }

        private bool validarEspacio(string nombreInsert, string ubicacionInsert, string superficieTexto, string precioTexto, 
            string tipoEspacioInsumoInsert,string horaIniInsert, string horaFinInsert, string depa, string prov, string dist, 
            string dias, int current)
        {
            string[] diasArray = dias.Split(',');

            if (!EsTextoConLetras(nombreInsert, "El nombre debe contener letras.", current)) return false;
            if (!EsTextoConLetras(ubicacionInsert, "La ubicación debe contener letras.", current)) return false;

            if (!ValidarCamposObligatorios(nombreInsert, ubicacionInsert, tipoEspacioInsumoInsert, current)) return false;
            if (!ValidarSuperficie(superficieTexto, current)) return false;
            if (!ValidarPrecio(precioTexto, current)) return false;
            if (!ValidarHoras(horaIniInsert, horaFinInsert, current)) return false;
            if (!ValidarUbicacion(depa, prov, dist, current)) return false;

            if (!ValidarDias(diasArray, dias, current)) return false;

            return true;
        }
        private bool EsTextoConLetras(string texto, string mensajeError, int pagina)
        {
            bool contieneLetras = !string.IsNullOrWhiteSpace(texto) && texto.Any(c => char.IsLetter(c));
            if (!contieneLetras)
            {
                MostrarError(mensajeError, pagina);
                return false;
            }
            return true;
        }

        private bool ValidarCamposObligatorios(string nombre, string ubicacion, string tipoEspacio, int pagina)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MostrarError("Debe ingresar el nombre del espacio.", pagina);
                return false;
            }
            if (nombre.Length > 45)
            {
                MostrarError("El nombre superó el máximo de 45 caracteres.", pagina);
                return false;
            }

            if (string.IsNullOrWhiteSpace(ubicacion))
            {
                MostrarError("Debe ingresar la ubicación del espacio.", pagina);
                return false;
            }
            if (ubicacion.Length > 100)
            {
                MostrarError("La ubicación superó el máximo de 100 caracteres.", pagina);
                return false;
            }

            if (string.IsNullOrWhiteSpace(tipoEspacio) || tipoEspacio == "0")
            {
                MostrarError("Debe seleccionar un tipo de espacio.", pagina);
                return false;
            }

            return true;
        }

        private bool ValidarSuperficie(string superficieTexto, int pagina)
        {
            if (!double.TryParse(superficieTexto, out double superficieInsert) || superficieInsert < 10 || superficieInsert > 1000)
            {
                MostrarError("La superficie debe ser un número positivo menor o igual a 1000 y como mínimo de 10 metros cuadrados.", pagina);
                return false;
            }
            return true;
        }

        private bool ValidarPrecio(string precioTexto, int pagina)
        {
            if (!double.TryParse(precioTexto, out double precioReservaInsert) || precioReservaInsert <= 0 || precioReservaInsert > 1000)
            {
                MostrarError("El precio de reserva debe ser un número positivo menor o igual a 1000.", pagina);
                return false;
            }
            return true;
        }

        private bool ValidarHoras(string horaIni, string horaFin, int pagina)
        {
            if (string.IsNullOrWhiteSpace(horaIni) || string.IsNullOrWhiteSpace(horaFin))
            {
                MostrarError("Debe ingresar la hora de inicio y la hora de fin.", pagina);
                return false;
            }
            return true;
        }

        private bool ValidarUbicacion(string depa, string prov, string dist, int pagina)
        {
            if (string.IsNullOrEmpty(depa))
            {
                MostrarError("Debe elegir un departamento.", pagina);
                return false;
            }

            if (string.IsNullOrEmpty(prov))
            {
                MostrarError("Debe elegir una provincia.", pagina);
                return false;
            }

            if (string.IsNullOrEmpty(dist))
            {
                MostrarError("Debe elegir un distrito.", pagina);
                return false;
            }

            return true;
        }

        private bool ValidarDias(string[] diasArray, string dias, int pagina)
        {
            if (!((diasArray.Length > 0) || dias == "") && pagina == AGREGAR_ESPACIO)
            {
                MostrarError("Debe seleccionar al menos 1 día de atención.", pagina);
                return false;
            }
            return true;
        }


        public void MostrarError(string mensaje, int current)
        {
            if (current == AGREGAR_ESPACIO)
            {
                lblError.Text = mensaje;
                abrirModalAgregarEspacio();
                return;
            }
            abrirModalEditarEspacio();
        }

        protected void guardarImgEspacio(ref string urlParaBd)
        {
            // 1. Obtener nombre original y asegurar que sea único
            string nombreArchivo = Path.GetFileName(fuAgregar.FileName);
            string nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(nombreArchivo); // evita sobrescribir

            // 2. Ruta física en el servidor
            string rutaRelativa = "~/Images/img/espacios/" + nombreUnico;
            string rutaFisica = Server.MapPath(rutaRelativa); // obtiene la ruta absoluta

            // 3. Guardar archivo
            fuAgregar.SaveAs(rutaFisica);
            // 4. URL relativa para la base de datos
            urlParaBd = "Images/img/espacios/" + nombreUnico;
        }

        protected void btnGuardarInsertado_Click(object sender, EventArgs e)
        {
            if (!ValidarFormularioEspacio()) return;

            int idDistritoHdn = int.Parse(hiddenIdDistrito.Value);
            string urlParaBd = "";
            guardarImgEspacio(ref urlParaBd);

            espacio espacioInsertar = ConstruirEspacioInsertar(idDistritoHdn, urlParaBd);
            int insertado = InsertarEspacio(espacioInsertar);

            if (insertado > 0)
            {
                if (!InsertarDiasFuncionamiento(insertado)) return;

                CargarEspacios();
                EnviarCorreosEspacioRegistrado(espacioInsertar);
                MostrarModalExitoCorreos();
            }
            else
            {
                MostrarErrorInsercionEspacio();
            }
        }
        private bool ValidarFormularioEspacio()
        {
            return validarEspacio(
                txtNombreEspacioAgregar.Text,
                txtUbicacionAgregar.Text,
                txtSuperficieAgregar.Text,
                txtPrecioReservaAgregar.Text,
                ddlTipoEspacioAgregar.SelectedValue,
                ddlHoraInicioInsert.Text,
                ddlHoraFinInsert.Text,
                ddlDepartamentoAgregar.Text,
                ddlProvinciaAgregar.Text,
                ddlDistritoAgregar.Text,
                diasSeleccionados.Value,
                AGREGAR_ESPACIO
            );
        }

        private espacio ConstruirEspacioInsertar(int idDistrito, string urlParaBd)
        {
            eTipoEspacio tipo;
            eTipoEspacio.TryParse(ddlTipoEspacioAgregar.SelectedValue, false, out tipo);

            return new espacio
            {
                idEspacio = 0,
                nombre = txtNombreEspacioAgregar.Text.Trim(),
                ubicacion = txtUbicacionAgregar.Text.Trim(),
                superficie = double.Parse(txtSuperficieAgregar.Text),
                precioReserva = double.Parse(txtPrecioReservaAgregar.Text),
                tipoEspacio = tipo,
                tipoEspacioSpecified = true,
                horarioInicioAtencion = ddlHoraInicioInsert.Text + ":00",
                horarioFinAtencion = ddlHoraFinInsert.Text + ":00",
                foto = urlParaBd,
                distrito = new distrito { idDistrito = idDistrito }
            };
        }

        private int InsertarEspacio(espacio espacioInsertar)
        {
            return espacioWS.insertarEspacio(new insertarEspacioRequest(espacioInsertar)).@return;
        }

        private bool InsertarDiasFuncionamiento(int idEspacioInsertado)
        {
            string dias = diasSeleccionados.Value;
            if (string.IsNullOrWhiteSpace(dias)) return true;

            foreach (string diaSem in dias.Split(','))
            {
                eDiaSemana diaParsed;
                eDiaSemana.TryParse<eDiaSemana>(diaSem, ignoreCase: true, out diaParsed);

                espacioDiaSem espacioDia = new espacioDiaSem
                {
                    idEspacio = idEspacioInsertado,
                    dia = diaParsed,
                    diaSpecified = true
                };

                bool insertado = diaSemWS.insertarDia(new insertarDiaRequest(espacioDia)).@return;
                if (!insertado)
                {
                    mostrarModalErrorEsp("ERROR AL INSERTAR DIA de SEMANA", "Se produjo un error al insertar el día de la semana del espacio.");
                    return false;
                }
            }

            return true;
        }

        private void EnviarCorreosEspacioRegistrado(espacio espacioInsertar)
        {
            string nombreDistrito = distritoWS.buscarDistPorId(
                new buscarDistPorIdRequest(espacioInsertar.distrito.idDistrito)
            ).@return.nombre;

            Thread thread = new Thread(() => enviarCorreosEspacio(nombreDistrito, espacioInsertar));
            thread.Start();
        }

        private void MostrarModalExitoCorreos()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModal", @"
        setTimeout(function() {
            mostrarModalExito('Correos enviados exitosamente',
            'Se enviaron correos a los compradores cuyo distrito favorito coincide con el distrito del espacio registrado.');
        }, 1000);", true);
        }

        private void MostrarErrorInsercionEspacio()
        {
            mostrarModalErrorEsp("ERROR AL INSERTAR ESPACIO", "Se produjo un error al insertar el espacio.");
        }

        protected void enviarCorreosEspacio(string nombreDistrito, espacio espacioInsertar)
        {
            string asunto = ConstruirAsuntoCorreoEspacio(nombreDistrito, espacioInsertar);
            string contenido = ConstruirContenidoCorreoEspacio(nombreDistrito, espacioInsertar);

            bool resultado = espacioWS.enviarCorreosCompradoresPorDistritoDeEspacio(
                new enviarCorreosCompradoresPorDistritoDeEspacioRequest(asunto, contenido, espacioInsertar.distrito.idDistrito)
            ).@return;

            if (resultado)
            {
                mostrarModalExitoEsp("ESPACIO INSERTADO CON ÉXITO",
                    "Se ha guardado el espacio satisfactoriamente y se enviaron correos a los compradores cuyo distrito favorito coincide con el distrito del espacio registrado.");
            }
            else
            {
                mostrarModalExitoEsp("ESPACIO INSERTADO CON ÉXITO",
                    "Se insertó el espacio correctamente, pero no se encontró compradores cuyo distrito favorito coincida para enviar el correo.");
            }
        }
        private string ConstruirAsuntoCorreoEspacio(string nombreDistrito, espacio espacioInsertar)
        {
            return $"¡Nuevo espacio disponible en tu distrito favorito {nombreDistrito}: {espacioInsertar.nombre}!";
        }
        private string ConstruirContenidoCorreoEspacio(string nombreDistrito, espacio espacioInsertar)
        {
            string estilos = ObtenerEstilosCorreo();

            return $@"
        <html>
        <head>
          <style>
            {estilos}
          </style>
        </head>
        <body>
          <div class='container'>
            {ConstruirHeaderCorreo(nombreDistrito)}
            {ConstruirBodyCorreo(nombreDistrito, espacioInsertar)}
            {ConstruirLogoCorreo()}
            {ConstruirFooterCorreo()}
          </div>
        </body>
        </html>";
        }
        private string ObtenerEstilosCorreo()
        {
            return string.Join(Environment.NewLine, new[]
            {
        ObtenerEstiloBase(),
        ObtenerEstiloContainer(),
        ObtenerEstiloHeader(),
        ObtenerEstiloBody(),
        ObtenerEstiloDetails(),
        ObtenerEstiloCTA(),
        ObtenerEstiloLogo(),
        ObtenerEstiloFooter()
    });
        }

        private string ObtenerEstiloBase()
        {
            return @"
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f8f9fa;
            margin: 0;
            padding: 0;
        }";
        }

        private string ObtenerEstiloContainer()
        {
            return @"
        .container {
            max-width: 600px;
            margin: 20px auto;
            background-color: #fff;
            border: 1px solid #dee2e6;
            border-radius: 8px;
            padding: 30px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.08);
        }";
        }

        private string ObtenerEstiloHeader()
        {
            return @"
        .header {
            background-color: #f10909;
            color: white;
            padding: 15px;
            border-radius: 6px 6px 0 0;
            text-align: center;
        }
        .header h2 {
            margin: 0;
            font-size: 22px;
            color: white;
        }";
        }

        private string ObtenerEstiloBody()
        {
            return @"
        .body {
            color: #212529;
            padding: 20px 0;
            font-size: 16px;
        }";
        }

        private string ObtenerEstiloDetails()
        {
            return @"
        .details {
            background-color: #f1f3f5;
            border-radius: 6px;
            padding: 15px;
            margin-bottom: 20px;
            list-style: none;
        }
        .details li {
            margin-bottom: 10px;
            font-size: 15px;
        }
        .details li strong {
            color: #000;
            font-weight: bold;
        }";
        }

        private string ObtenerEstiloCTA()
        {
            return @"
        .cta {
            display: inline-block;
            background-color: #f10909;
            color: #fff !important;
            padding: 10px 20px;
            border-radius: 5px;
            text-decoration: none;
            font-weight: bold;
            margin-top: 10px;
        }
        .cta:hover {
            background-color: #c40808;
        }";
        }

        private string ObtenerEstiloLogo()
        {
            return @"
        .logo {
            text-align: center;
            margin-top: 20px;
        }
        .logo img {
            width: 100px;
        }";
        }

        private string ObtenerEstiloFooter()
        {
            return @"
        .footer {
            text-align: center;
            font-size: 12px;
            color: #6c757d;
            margin-top: 20px;
        }";
        }


        private string ConstruirHeaderCorreo(string nombreDistrito)
        {
            return $@"
        <div class='header'>
          <h2>¡Nuevo espacio en tu distrito favorito {nombreDistrito}!</h2>
        </div>";
        }

        private string ConstruirBodyCorreo(string nombreDistrito, espacio espacioInsertar)
        {
            return $@"
        <div class='body'>
          <p>Estimado usuario,</p>
          <p>Nos complace informarte que se ha registrado un nuevo espacio disponible en tu distrito favorito: <strong>{nombreDistrito}</strong>.</p>
          {ConstruirListaDetallesEspacio(espacioInsertar)}
          <p>Si estás interesado, te invitamos a revisar más detalles o reservar tu espacio.</p>
          <a href='http://54.91.139.38/Presentacion/Inicio/PrincipalInvitado.aspx' class='cta'>Ver más</a>
        </div>";
        }

        private string ConstruirListaDetallesEspacio(espacio espacioInsertar)
        {
            return $@"
        <ul class='details'>
            <li><strong>📌 Nombre:</strong> {espacioInsertar.nombre}</li>
            <li><strong>🏷 Tipo:</strong> {espacioInsertar.tipoEspacio}</li>
            <li><strong>📍 Ubicación:</strong> {espacioInsertar.ubicacion}</li>
            <li><strong>📐 Superficie:</strong> {espacioInsertar.superficie} m²</li>
            <li><strong>💵 Precio de Reserva:</strong> S/ {espacioInsertar.precioReserva}</li>
            <li><strong>⏰ Horario de Atención:</strong> {espacioInsertar.horarioInicioAtencion} - {espacioInsertar.horarioFinAtencion}</li>
        </ul>";
        }

        private string ConstruirLogoCorreo()
        {
            return @"
        <div class='logo'>
            <img src='https://upload.wikimedia.org/wikipedia/commons/4/43/Escudo_Regi%C3%B3n_Lima.png' alt='Logo Región Lima' />
        </div>";
        }

        private string ConstruirFooterCorreo()
        {
            return @"
        <div class='footer'>
            Este mensaje fue enviado automáticamente por el sistema SIRGEP.<br>
            © 2025 Gobierno Regional de Lima
        </div>";
        }


        public void LimpiarDatosAgregados()
        {
            txtUbicacionAgregar.Text = "";
            txtSuperficieAgregar.Text = "";
            txtPrecioReservaAgregar.Text = "";
            txtUbicacionAgregar.Text = "";
            txtNombreEspacioAgregar.Text = "";
            lblError.Text = "";
        }

        protected void ddlDistritoAgregar_SelectedIndexChanged(object sender, EventArgs e)
        {
            hiddenIdDistrito.Value = ddlDistritoAgregar.SelectedValue.ToString();
            string lblErrorDist = lblError.Text;
            if (!string.IsNullOrEmpty(lblErrorDist) && lblErrorDist.Contains("distrito")) lblError.Text = "";
        }

        protected void txtHoraFinInsert_TextChanged(object sender, EventArgs e)
        {
            string horaInicioStr = ddlHoraInicioInsert.Text.Trim();
            string horaFinStr = ddlHoraFinInsert.Text.Trim();

            if (!ValidarHorasNoVacias(horaInicioStr, horaFinStr)) return;
            if (!ValidarHorasEnPunto(horaInicioStr, horaFinStr)) return;
            if (!ValidarFormatoHoras(horaInicioStr, horaFinStr, out TimeSpan horaInicio, out TimeSpan horaFin)) return;
            if (!ValidarOrdenHoras(horaInicio, horaFin)) return;

            lblError.Text = "";
            abrirModalAgregarEspacio();
        }
        private bool ValidarHorasNoVacias(string horaInicio, string horaFin)
        {
            if (string.IsNullOrEmpty(horaInicio) || string.IsNullOrEmpty(horaFin))
            {
                lblError.Text = "Las horas no pueden estar vacías.";
                abrirModalAgregarEspacio();
                return false;
            }
            return true;
        }

        private bool ValidarHorasEnPunto(string horaInicio, string horaFin)
        {
            TimeSpan ini = TimeSpan.Parse(horaInicio);
            TimeSpan fin = TimeSpan.Parse(horaFin);

            if (ini.Minutes != 0 || fin.Minutes != 0)
            {
                lblError.Text = "Las horas deben finalizar en :00";
                abrirModalAgregarEspacio();
                return false;
            }
            return true;
        }

        private bool ValidarFormatoHoras(string horaInicioStr, string horaFinStr, out TimeSpan horaInicio, out TimeSpan horaFin)
        {
            bool inicioOk = TimeSpan.TryParse(horaInicioStr, out horaInicio);
            bool finOk = TimeSpan.TryParse(horaFinStr, out horaFin);

            if (!inicioOk || !finOk)
            {
                lblError.Text = "Formato de hora no válido.";
                abrirModalAgregarEspacio();
                return false;
            }
            return true;
        }

        private bool ValidarOrdenHoras(TimeSpan horaInicio, TimeSpan horaFin)
        {
            if (horaInicio >= horaFin)
            {
                lblError.Text = "La hora de inicio debe ser menor que la hora de fin.";
                abrirModalAgregarEspacio();
                return false;
            }
            return true;
        }


        public void abrirModalAgregarEspacio()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModalPaso1",
                    "var modal1 = new bootstrap.Modal(document.getElementById('modalPaso1')); modal1.show();", true);
        }

        public void abrirModalEditarEspacio()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModalEdicion",
                "var modalEdicion = new bootstrap.Modal(document.getElementById('modalEdicionEspacio')); modalEdicion.show();", true);
        }

        public void mostrarModalExitoEsp(string titulo, string mensaje)
        {
            string script = $@"
                Sys.Application.add_load(function () {{
                    mostrarModalExito('{titulo}', '{mensaje}');
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalExito", script, true);
        }

        private void mostrarModalErrorEsp(string titulo, string mensaje)
        {
            string script = $@"
                Sys.Application.add_load(function () {{
                    mostrarModalError('{titulo}', '{mensaje}');
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", script, true);
        }

        protected void ddlDepartamentoEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlDepartamentoEdit.SelectedValue))
            {
                int idDepartamento = int.Parse(ddlDepartamentoEdit.SelectedValue);
                listarProvinciaPorDepaResponse responseProvincia = provinciaWS.listarProvinciaPorDepa(
                    new listarProvinciaPorDepaRequest(idDepartamento)
                );
                if (ddlProvinciaEdit.Enabled == false) ddlProvinciaEdit.Enabled = true;
                ddlProvinciaEdit.DataSource = responseProvincia.@return;
                ddlProvinciaEdit.DataTextField = "Nombre";
                ddlProvinciaEdit.DataValueField = "IdProvincia";
                ddlProvinciaEdit.DataBind();
                ddlProvinciaEdit.Items.Insert(0, new ListItem("Seleccione una provincia", ""));
                ddlDistritoEdit.Items.Clear();
                ddlDistritoEdit.Items.Insert(0, new ListItem("Seleccione una provincia primero", ""));

                string lblErrorDepa = lblError.Text;
                if (!string.IsNullOrEmpty(lblErrorDepa) && lblErrorDepa.Contains("departamento")) lblError.Text = "";

            }
            else
            {
                ddlProvinciaEdit.Items.Clear();
                ddlProvinciaEdit.Items.Insert(0, new ListItem("Seleccione un departamento primero", ""));
            }

            // Para mantener el modal abierto tras el postback
            abrirModalEditarEspacio();
        }

        protected void ddlProvinciaEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlProvinciaEdit.SelectedValue))
            {
                int idProvincia = int.Parse(ddlProvinciaEdit.SelectedValue);
                listarDistritosFiltradosResponse responseDistrito = distritoWS.listarDistritosFiltrados(
                    new listarDistritosFiltradosRequest(idProvincia)
                );
                if (ddlDistritoEdit.Enabled == false) ddlDistritoEdit.Enabled = true;
                ddlDistritoEdit.DataSource = responseDistrito.@return;
                ddlDistritoEdit.DataTextField = "Nombre";
                ddlDistritoEdit.DataValueField = "IdDistrito";
                ddlDistritoEdit.DataBind();
                ddlDistritoEdit.Items.Insert(0, new ListItem("Seleccione un distrito", ""));
                string lblErrorProv = lblError.Text;
                if (!string.IsNullOrEmpty(lblErrorProv) && lblErrorProv.Contains("provincia")) lblError.Text = "";
            }
            else
            {
                ddlDistritoEdit.Items.Clear();
                ddlDistritoEdit.Items.Insert(0, new ListItem("Seleccione una provincia primero", ""));
            }

            // Para que el modal siga abierto
            abrirModalEditarEspacio();
        }

        protected void ddlDistritoEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            hiddenIdDistrito.Value = ddlDistritoEdit.SelectedValue.ToString();
            string lblErrorDist = lblError.Text;
            if (!string.IsNullOrEmpty(lblErrorDist) && lblErrorDist.Contains("distrito")) lblError.Text = "";
        }

        protected void btnEditUbigeo_Click(object sender, EventArgs e)
        {
            ddlDepartamentoEdit.Enabled = true;
            abrirModalEditarEspacio();
        }

        protected void btnGuardarEdicion_Click(object sender, EventArgs e)
        {

        }
        public void mostrarModalFoto(string dataUrl)
        {
            // script para cambiar src del <img> y mostrar el modal
            string script = $@"
            Sys.Application.add_load(function () {{
                document.getElementById('imgPreviewModal').src = '{dataUrl}';
                var myModal = new bootstrap.Modal(document.getElementById('modalPreview'));
                myModal.show();
            }});";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarPreview", script, true);
        }
        protected void lnkVerImagen_Command(object sender, CommandEventArgs e)
        {
            string ruta = e.CommandArgument as string;

            // Si no hay ruta o es una cadena vacía, no hacemos nada
            if (string.IsNullOrWhiteSpace(ruta) || ruta == "null")
                return;
            mostrarModalFoto("/" + ruta); // Asegúrate de que comience con "/"
        }
    }
}