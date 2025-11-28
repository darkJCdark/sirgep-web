using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static SirgepPresentacion.Presentacion.Ventas.Reserva.FormularioEspacio;

namespace SirgepPresentacion.Presentacion.Infraestructura.Evento
{
    public partial class GestionaEventos : System.Web.UI.Page
    {
        private EventoWS eventoWS;
        private FuncionWS funcionWS;
        private DepartamentoWS depaWS;
        private ProvinciaWS provWS;
        private DistritoWS distWS;

        private const int MODAL_AGREGAR = 0;
        private const int MODAL_EDITAR = 1;

        public List<funcion> funcionesEditar
        {
            get
            {
                if (ViewState["funcionesEditar"] == null)
                    ViewState["funcionesEditar"] = new List<funcion>();
                return (List<funcion>)ViewState["funcionesEditar"];
            }
            set
            {
                ViewState["funcionesEditar"] = value;
            }
        }
        private int PaginaActual
        {
            get => ViewState["PaginaActual"] != null ? (int)ViewState["PaginaActual"] : 1;
            set => ViewState["PaginaActual"] = value;
        }

        private const int TAM_PAGINA = 10;
        private int TotalPaginas = 1; // se calcula en CargarEspacios()

        protected void Page_Init(object sender, EventArgs e)
        {
            funcionesEditar = new List<funcion>();
            eventoWS = new EventoWSClient();
            funcionWS = new FuncionWSClient();
            depaWS = new DepartamentoWSClient();
            provWS = new ProvinciaWSClient();
            distWS = new DistritoWSClient();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtFechaFuncion.Attributes["min"] = DateTime.Now.ToString("yyyy-MM-dd");
                PaginaActual = 1;
                CargarEventos();
            }
        }
        protected void Paginar_Click(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Anterior") PaginaActual--;
            if (e.CommandName == "Siguiente") PaginaActual++;

            CargarEventos();
        }
        public void realizarPaginado(evento[] response)
        {
            if (!ValidarRespuesta(response)) return;

            var todos = response.ToList();
            CalcularPaginacion(todos.Count);
            AjustarPaginaActual();

            var paginaActual = ObtenerPaginaActual(todos);
            CargarRepeater(paginaActual);
            ActualizarFooterPaginacion();
        }
        private bool ValidarRespuesta(evento[] response)
        {
            if (response == null)
            {
                mostrarModalErrorEvento("RESULTADO DE BUSQUEDA", "No se encontraron eventos con los parámetros actuales; se listarán todos los eventos...");
                CargarEventos();
                return false;
            }
            return true;
        }

        private void CalcularPaginacion(int totalItems)
        {
            TotalPaginas = (int)Math.Ceiling((double)totalItems / TAM_PAGINA);
        }

        private void AjustarPaginaActual()
        {
            if (PaginaActual < 1)
                PaginaActual = 1;
            if (PaginaActual > TotalPaginas)
                PaginaActual = TotalPaginas;
        }

        private List<evento> ObtenerPaginaActual(List<evento> todos)
        {
            return todos.Skip((PaginaActual - 1) * TAM_PAGINA).Take(TAM_PAGINA).ToList();
        }

        private void CargarRepeater(List<evento> eventosPagina)
        {
            rptEventos.DataSource = eventosPagina;
            rptEventos.DataBind();
        }

        private void ActualizarFooterPaginacion()
        {
            if (rptEventos.Controls.Count == 0)
                return;

            var footer = rptEventos.Controls[rptEventos.Controls.Count - 1];

            var lblPagina = footer.FindControl("lblPaginaFootEvento") as Label;
            if (lblPagina != null)
                lblPagina.Text = $"Página {PaginaActual} / {TotalPaginas}";

            var btnAnterior = footer.FindControl("btnAnteriorFootEvento") as Button;
            var btnSiguiente = footer.FindControl("btnSiguienteFootEvento") as Button;

            if (btnAnterior != null)
                btnAnterior.Enabled = PaginaActual > 1;

            if (btnSiguiente != null)
                btnSiguiente.Enabled = PaginaActual < TotalPaginas;
        }


        protected void CargarEventos()
        {
            evento[] eventos= eventoWS.listarEvento(new listarEventoRequest()).@return; ;
            realizarPaginado(eventos);
        }

        protected void ddlFiltroFechas_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblError.Text = "";
        }
        public void cargarUbicacion()
        {
            /* Depas */
            ddlDepaAgregar.DataSource = depaWS.listarDepas(new listarDepasRequest()).@return;
            ddlDepaAgregar.DataTextField = "Nombre";
            ddlDepaAgregar.DataValueField = "idDepartamento";
            ddlDepaAgregar.DataBind();
            ddlDepaAgregar.Items.Insert(0, new ListItem("Seleccione un departamento", ""));

            /* Provincias */
            // ddlProvAgregar.DataSource = provWS.listarProvinciaPorDepa(new // id).@return;
        }

        protected void btnMostrarModalAgregarEvento_Click(object sender, EventArgs e)
        {
            cargarUbicacion();
            abrirModalAgregar();
        }

        protected void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            buscarEventoPorTextoResponse response = eventoWS.buscarEventoPorTexto(new buscarEventoPorTextoRequest(txtBusqueda.Text));
            evento[] eventos = response.@return;
            realizarPaginado(eventos);
        }

        public void LimpiarDatosAgregados()
        {
            txtFechaInicioEvento.Text = "";
            txtFechaInicioEvento.Text="";
            txtNomEvent.Text = "";
            txtDescAgregar.Text = "";
            txtUbicacionAgregar.Text = "";
            ddlDistAgregar.SelectedValue = ""; // obteniendo ID de distrito seleccionado
            txtPrecioEntrada.Text = "";
            txtDisponibles.Text = "";
            txtVendidas.Text = "";
            txtReferencia.Text = "";
            lblError.Text = "";
        }

        private bool ValidarDatosEvento(string txtNombre, string txtDesc, string txtUbi, string txtPrecio, string txtDispo, 
            string txtVend, string txtRef, string ddlDepas, string ddlDist, string ddlProv, bool fileUpdate, int pagina)
        {
            if (!ValidarFechaMinima(pagina)) return false;
            if (!ValidarCamposConLetras(txtNombre, txtDesc, txtUbi, txtRef, pagina)) return false;
            if (!ValidarLongitudesYObligatorios(txtNombre, txtDesc, txtUbi, txtRef, pagina)) return false;
            if (!ValidarUbicacion(ddlDepas, ddlProv, ddlDist, pagina)) return false;
            if (!ValidarNumericos(txtPrecio, txtDispo, txtVend, pagina)) return false;
            if (!ValidarImagen(fileUpdate, pagina)) return false;

            return true;
        }
        private bool ValidarFechaMinima(int pagina)
        {
            if (Session["fechaMinima"] == null)
            {
                MostrarError("Debe agregar al menos 1 función.", pagina);
                return false;
            }
            return true;
        }

        private bool ValidarCamposConLetras(string nombre, string desc, string ubi, string refEvento, int pagina)
        {
            bool ContieneLetras(string texto) => !string.IsNullOrWhiteSpace(texto) && texto.Any(c => char.IsLetter(c));

            if (!ContieneLetras(nombre))
            {
                MostrarError("El nombre debe contener letras.", pagina);
                return false;
            }

            if (!ContieneLetras(desc))
            {
                MostrarError("La descripción debe contener letras.", pagina);
                return false;
            }

            if (!ContieneLetras(ubi))
            {
                MostrarError("La ubicación debe contener letras.", pagina);
                return false;
            }

            if (!ContieneLetras(refEvento))
            {
                MostrarError("La referencia debe contener letras.", pagina);
                return false;
            }

            return true;
        }
        private bool ValidarLongitudesYObligatorios(string nombre, string desc, string ubi, string refEvento, int pagina)
        {
            if (!ValidarCampo(nombre, 45, "nombre del evento", pagina)) return false;
            if (!ValidarCampo(desc, 350, "descripción del evento", pagina)) return false;
            if (!ValidarCampo(ubi, 45, "ubicación del evento", pagina)) return false;
            if (!ValidarCampo(refEvento, 45, "referencia del evento", pagina)) return false;

            return true;
        }
        private bool ValidarCampo(string valor, int maxLength, string nombreCampo, int pagina)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                MostrarError($"Debe ingresar {nombreCampo}.", pagina);
                return false;
            }

            if (valor.Length > maxLength)
            {
                MostrarError($"La longitud de {nombreCampo} ha superado los {maxLength} caracteres.", pagina);
                return false;
            }

            return true;
        }


        private bool ValidarUbicacion(string depa, string prov, string dist, int pagina)
        {
            if (string.IsNullOrWhiteSpace(depa) || depa == "0")
            {
                MostrarError("Debe seleccionar un departamento.", pagina);
                return false;
            }

            if (string.IsNullOrWhiteSpace(prov) || prov == "0")
            {
                MostrarError("Debe seleccionar una provincia.", pagina);
                return false;
            }

            if (string.IsNullOrWhiteSpace(dist) || dist == "0")
            {
                MostrarError("Debe seleccionar un distrito.", pagina);
                return false;
            }

            return true;
        }

        private bool ValidarNumericos(string precioTxt, string dispoTxt, string vendTxt, int pagina)
        {
            if (!int.TryParse(precioTxt, out int precio) || precio < 0 || precio > 1000)
            {
                MostrarError("El precio de la entrada debe ser un número entre 0 y 1000.", pagina);
                return false;
            }

            if (!int.TryParse(dispoTxt, out int disponibles) || disponibles < 0 || disponibles > 1000)
            {
                MostrarError("La cantidad de entradas disponibles debe ser un número entre 0 y 1000.", pagina);
                return false;
            }

            int.TryParse(vendTxt, out int vendidas);
            if (vendidas < 0 || vendidas > 1000)
            {
                MostrarError("La cantidad de entradas vendidas debe ser un número entre 0 y 1000.", pagina);
                return false;
            }

            if (vendidas > disponibles)
            {
                MostrarError("La cantidad de entradas vendidas debe ser menor que la cantidad disponible (total de entradas).", pagina);
                return false;
            }

            return true;
        }

        private bool ValidarImagen(bool fileUpdate, int pagina)
        {
            if (!fileUpdate)
            {
                MostrarError("Debe seleccionar una imagen obligatoriamente", pagina);
                return false;
            }
            return true;
        }


        public void guardarImgEvento(ref string urlParaBD)
        {
            // 1. Obtener nombre original y asegurar que sea único
            string nombreArchivo = Path.GetFileName(fuAgregar.FileName);
            string nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(nombreArchivo); // evita sobrescribir

            // 2. Ruta física en el servidor
            string rutaRelativa = "~/Images/img/eventos/" + nombreUnico;
            string rutaFisica = Server.MapPath(rutaRelativa); // obtiene la ruta absoluta

            // 3. Guardar archivo
            fuAgregar.SaveAs(rutaFisica);
            // 4. URL relativa para la base de datos
            urlParaBD = "Images/img/eventos/" + nombreUnico;
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarDatosAgregar()) return;

            string urlParaBd = "";
            guardarImgEvento(ref urlParaBd);

            evento eventoAgregar = CrearEventoParaAgregar(urlParaBd);
            int idEventoInsertado = eventoWS.insertarEvento(new insertarEventoRequest(eventoAgregar)).@return;

            if (idEventoInsertado >= 1)
            {
                if (!AgregarFuncionesEvento(idEventoInsertado)) return;
                EnviarCorreosEventoNuevo(eventoAgregar.distrito.idDistrito, eventoAgregar);
                mostrarModalExitoEvento("VENTANA DE ÉXITO", "Se insertó el EVENTO correctamente y se enviarán correos a los compradores cuyo distrito favorito coincide con el distrito del evento registrado.");
            }
            else
            {
                mostrarModalErrorEvento("VENTANA DE ERROR", "Error al insertar el evento");
            }

            LimpiarDatosAgregados();
        }
        private bool ValidarDatosAgregar()
        {
            return ValidarDatosEvento(txtNomEvent.Text, txtDescAgregar.Text,
                txtUbicacionAgregar.Text, txtPrecioEntrada.Text, txtDisponibles.Text,
                txtVendidas.Text, txtReferencia.Text, ddlDepaAgregar.Text, ddlDistAgregar.Text,
                ddlProvAgregar.Text, fuAgregar.HasFile, MODAL_AGREGAR);
        }
        private evento CrearEventoParaAgregar(string urlParaBd)
        {
            string fechaInicioEvento = Session["fechaMinima"].ToString();
            string fechaFinEvento = Session["fechaMaxima"].ToString();

            return new evento
            {
                fecha_inicio = fechaInicioEvento.Split(' ')[0],
                fecha_fin = fechaFinEvento.Split(' ')[0],
                nombre = txtNomEvent.Text,
                descripcion = txtDescAgregar.Text,
                ubicacion = txtUbicacionAgregar.Text,
                distrito = new distrito
                {
                    idDistrito = int.Parse(ddlDistAgregar.SelectedValue)
                },
                precioEntrada = double.Parse(txtPrecioEntrada.Text),
                cantEntradasDispo = int.Parse(txtDisponibles.Text),
                cantEntradasVendidas = int.Parse(txtVendidas.Text),
                referencia = txtReferencia.Text,
                archivoImagen = urlParaBd
            };
        }
        private bool AgregarFuncionesEvento(int idEvento)
        {
            foreach (ListItem item in ddlFuncionesAgregar.Items)
            {
                if (item.Text.StartsWith("P")) continue;

                string[] partes = item.Text.Split(new[] { " - " }, StringSplitOptions.None);
                string fechaFuncion = partes[0];
                string[] horas = partes.Length > 1 ? partes[1].Split(new[] { " a " }, StringSplitOptions.None) : new string[0];

                string horaIniFuncion = horas.Length > 0 ? horas[0] : "";
                string horaFinFuncion = horas.Length > 1 ? horas[1] : "";

                funcion funcionAgregar = new funcion
                {
                    fecha = fechaFuncion,
                    horaInicio = horaIniFuncion,
                    horaFin = horaFinFuncion,
                    evento = new evento { idEvento = idEvento }
                };

                int idFuncion = funcionWS.insertarFuncion(new insertarFuncionRequest(funcionAgregar)).@return;
                if (idFuncion < 1)
                {
                    mostrarModalErrorEvento("VENTANA DE ERROR", "Error al insertar la función con ID: " + idFuncion);
                    return false;
                }
            }
            return true;
        }
        private void EnviarCorreosEventoNuevo(int idDistrito, evento eventoInsertado)
        {
            string nombreDistrito = distWS.buscarDistPorId(
                new buscarDistPorIdRequest(idDistrito)
            ).@return.nombre;

            Thread thread = new Thread(() => enviarCorreosEvento(nombreDistrito, eventoInsertado));
            thread.Start();
        }

        protected void enviarCorreosEvento(string nombreDistrito, evento eventoAgregar)
        {
            string asunto = GenerarAsuntoCorreo(nombreDistrito, eventoAgregar.nombre);
            string contenido = GenerarContenidoCorreo(nombreDistrito, eventoAgregar);

            bool resultado = eventoWS.enviarCorreosCompradoresPorDistritoDeEvento(
                new enviarCorreosCompradoresPorDistritoDeEventoRequest(asunto, contenido, eventoAgregar.distrito.idDistrito)
            ).@return;
        }
        private string GenerarAsuntoCorreo(string nombreDistrito, string nombreEvento)
        {
            return $"¡Nuevo evento en tu distrito favorito {nombreDistrito}: {nombreEvento}!";
        }
        private string GenerarContenidoCorreo(string nombreDistrito, evento eventoAgregar)
        {
            string estilos = ObtenerEstilosCorreo();
            string header = GenerarHeaderCorreo(nombreDistrito);
            string cuerpo = GenerarCuerpoCorreo(nombreDistrito, eventoAgregar);
            string logo = GenerarLogoCorreo();
            string footer = GenerarFooterCorreo();

            return $@"
        <html>
        <head>
            <style>{estilos}</style>
        </head>
        <body>
            <div class='container'>
                {header}
                {cuerpo}
                {logo}
                {footer}
            </div>
        </body>
        </html>";
        }
        private string GenerarHeaderCorreo(string nombreDistrito)
        {
            return $@"
        <div class='header'>
            <h2>¡No te pierdas este evento en tu distrito favorito {nombreDistrito}!</h2>
        </div>";
        }
        private string GenerarCuerpoCorreo(string nombreDistrito, evento eventoAgregar)
        {
            return $@"
        <div class='body'>
            <p>Nos alegra informarte que se ha registrado un nuevo evento en tu distrito favorito: <strong>{nombreDistrito}</strong>.</p>
            <ul class='details'>
                <li><strong>🎉 Nombre del evento:</strong> {eventoAgregar.nombre}</li>
                <li><strong>📅 Fecha:</strong> {eventoAgregar.fecha_inicio} al {eventoAgregar.fecha_fin}</li>
                <li><strong>📍 Ubicación:</strong> {eventoAgregar.ubicacion}</li>
                <li><strong>🗺 Referencia:</strong> {eventoAgregar.referencia}</li>
                <li><strong>🎟 Entradas disponibles:</strong> {eventoAgregar.cantEntradasDispo}</li>
                <li><strong>💵 Precio por entrada:</strong> S/ {eventoAgregar.precioEntrada}</li>
            </ul>
            <p>Si deseas más información o comprar entradas, haz clic en el botón de abajo:</p>
            <a href='http://54.91.139.38/Presentacion/Inicio/PrincipalInvitado.aspx' class='cta'>Ver más</a>
        </div>";
        }
        private string GenerarLogoCorreo()
        {
            return @"
        <div class='logo'>
            <img src='https://upload.wikimedia.org/wikipedia/commons/4/43/Escudo_Regi%C3%B3n_Lima.png' alt='Logo Región Lima' />
        </div>";
        }
        private string GenerarFooterCorreo()
        {
            return @"
        <div class='footer'>
            Este mensaje fue enviado automáticamente por el sistema SIRGEP.<br>
            © 2025 Gobierno Regional de Lima
        </div>";
        }

        private string ObtenerEstilosCorreo()
        {
            return string.Join(Environment.NewLine, new[]
            {
                EstiloGlobal(),
                EstiloContainer(),
                EstiloHeader(),
                EstiloBody(),
                EstiloDetails(),
                EstiloCTA(),
                EstiloLogo(),
                EstiloFooter()
            });
        }
        private string EstiloGlobal()
        {
            return @"
    body {
        font-family: 'Segoe UI', sans-serif;
        background-color: #f8f9fa;
        margin: 0;
        padding: 0;
    }";
        }

        private string EstiloContainer()
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

        private string EstiloHeader()
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

        private string EstiloBody()
        {
            return @"
    .body {
        color: #212529;
        padding: 20px 0;
        font-size: 16px;
    }";
        }

        private string EstiloDetails()
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

        private string EstiloCTA()
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

        private string EstiloLogo()
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

        private string EstiloFooter()
        {
            return @"
    .footer {
        text-align: center;
        font-size: 12px;
        color: #6c757d;
        margin-top: 20px;
    }";
        }

        protected void ddlProvAgregar_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cuando la provincia cambia, debemos listar los distritos!!!
            if (!string.IsNullOrEmpty(ddlProvAgregar.SelectedValue))
            {
                int idProvincia = int.Parse(ddlProvAgregar.SelectedValue);
                listarDistritosFiltradosResponse responseDistrito = distWS.listarDistritosFiltrados(
                    new listarDistritosFiltradosRequest(idProvincia)
                );

                ddlDistAgregar.DataSource = responseDistrito.@return;
                ddlDistAgregar.DataTextField = "Nombre";
                ddlDistAgregar.DataValueField = "IdDistrito";
                ddlDistAgregar.DataBind();
                ddlDistAgregar.Items.Insert(0, new ListItem("Seleccione un distrito", ""));
            }
            else
            {
                ddlDistAgregar.Items.Clear();
                ddlDistAgregar.Items.Insert(0, new ListItem("Seleccione una provincia primero", ""));
            }

            // Para mantener el modal abierto tras el postback
            abrirModalAgregar();
        }

        protected void ddlDepaAgregar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlDepaAgregar.SelectedValue))
            {
                int idDepartamento = int.Parse(ddlDepaAgregar.SelectedValue);
                listarProvinciaPorDepaResponse responseProvincia = provWS.listarProvinciaPorDepa(
                    new listarProvinciaPorDepaRequest(idDepartamento)
                );

                ddlProvAgregar.DataSource = responseProvincia.@return;
                ddlProvAgregar.DataTextField = "Nombre";
                ddlProvAgregar.DataValueField = "IdProvincia";
                ddlProvAgregar.DataBind();
                ddlProvAgregar.Items.Insert(0, new ListItem("Seleccione una provincia", ""));
            }
            else
            {
                ddlProvAgregar.Items.Clear();
                ddlProvAgregar.Items.Insert(0, new ListItem("Seleccione un departamento primero", ""));
            }

            // Para mantener el modal abierto tras el postback
            abrirModalAgregar();
        }

        public void abrirModalAgregar()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModalEvento",
            "var modalEvento = new bootstrap.Modal(document.getElementById('modalAgregarEvento')); modalEvento.show();", true);
        }

        protected void btnAgregarFuncion_Click(object sender, EventArgs e)
        {
            lblErrorAgregar.Text = "";
            string fecha = txtFechaFuncion.Text.Trim();
            string horaInicio = txtHoraIniFuncion.Text.Trim();
            string horaFin = txtHoraFinFuncion.Text.Trim();

            string valor = $"{fecha}_{horaInicio}_{horaFin}";
            string texto = $"{fecha} - {horaInicio} a {horaFin}";

            const int MODAL_AGREGAR = 0;
            if (!ValidaFuncion(fecha,horaInicio,horaFin,valor, MODAL_AGREGAR)) return;
            lblErrorEditar.Text = "";
            DateTime.TryParse(fecha, out DateTime fechaSeleccionada);

            ddlFuncionesAgregar.Items.Add(new ListItem(texto, valor));
            ActualizarFechasMinMax(fechaSeleccionada);
            LimpiarFunciones();
            // Para mantener el modal abierto tras el postback
            ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModalEvento",
            "var modalEvento = new bootstrap.Modal(document.getElementById('modalAgregarEvento')); modalEvento.show();", true);
        }

        public bool validaFormatoHora(TimeSpan tsInicio, TimeSpan tsFin)
        {
            bool EsValido(TimeSpan t) =>
                    t.Seconds == 0 &&
                (t.Minutes == 0 || t.Minutes == 15 || t.Minutes == 30 || t.Minutes == 45);

            return EsValido(tsInicio) && EsValido(tsFin);
        }
        private bool ValidaFuncion(string fecha, string horaInicio, string horaFin, string valor, int pagina)
        {
            if (!ValidarCamposCompletos(fecha, horaInicio, horaFin, pagina)) return false;
            if (!ValidarFecha(fecha, pagina, out DateTime fechaSeleccionada)) return false;
            if (!ValidarHoras(horaInicio, horaFin, pagina, out TimeSpan tsInicio, out TimeSpan tsFin)) return false;
            if (!ValidarFormatoHora(tsInicio, tsFin, pagina)) return false;
            if (!ValidarFuncionDuplicada(valor, pagina)) return false;

            return true;
        }
        private bool ValidarCamposCompletos(string fecha, string horaInicio, string horaFin, int pagina)
        {
            if (string.IsNullOrWhiteSpace(fecha) || string.IsNullOrWhiteSpace(horaInicio) || string.IsNullOrWhiteSpace(horaFin))
            {
                MostrarError("Por favor complete todos los campos de la función.", pagina);
                return false;
            }
            return true;
        }

        private bool ValidarFecha(string fecha, int pagina, out DateTime fechaSeleccionada)
        {
            fechaSeleccionada = default;

            if (!DateTime.TryParse(fecha, out fechaSeleccionada))
            {
                MostrarError("La fecha no tiene un formato válido.", pagina);
                return false;
            }

            DateTime hoy = DateTime.Today;

            if (fechaSeleccionada.Year != hoy.Year)
            {
                MostrarError("La fecha debe estar en el año actual.", pagina);
                return false;
            }

            if (fechaSeleccionada < hoy)
            {
                MostrarError("La fecha no puede ser anterior a hoy.", pagina);
                return false;
            }

            return true;
        }

        private bool ValidarHoras(string horaInicio, string horaFin, int pagina, out TimeSpan tsInicio, out TimeSpan tsFin)
        {
            tsInicio = tsFin = default;

            if (!TimeSpan.TryParse(horaInicio, out tsInicio) || !TimeSpan.TryParse(horaFin, out tsFin))
            {
                MostrarError("Las horas no tienen un formato válido.", pagina);
                return false;
            }

            if (tsInicio >= tsFin)
            {
                MostrarError("La hora de inicio debe ser menor a la hora de fin.", pagina);
                return false;
            }

            return true;
        }

        private bool ValidarFormatoHora(TimeSpan tsInicio, TimeSpan tsFin, int pagina)
        {
            if (!validaFormatoHora(tsInicio, tsFin))
            {
                MostrarError("La horas deben terminar en algún múltiplo de 15 (00,15,30,45).", pagina);
                return false;
            }
            return true;
        }

        private bool ValidarFuncionDuplicada(string valor, int pagina)
        {
            if (ddlFuncionesAgregar.Items.Cast<ListItem>().Any(i => i.Value == valor))
            {
                MostrarError("Esa función ya ha sido agregada.", pagina);
                return false;
            }
            return true;
        }


        public void LimpiarFunciones()
        {
            txtFechaFuncion.Text = "";
            txtHoraIniFuncion.Text = "";
            txtHoraFinFuncion.Text = "";

            txtFechaEditar.Text = "";
            txtHoraIniEditar.Text = "";
            txtHoraFinEditar.Text = "";
        }

        private void MostrarError(string mensaje, int pagina)
        {
            if(pagina == MODAL_AGREGAR)
            {
                lblErrorAgregar.Text = mensaje;
                abrirModalAgregar();
                return;
            }
            lblErrorEditar.Text = mensaje;
            abrirModalEdicion();
        }

        private void ActualizarFechasMinMax(DateTime fecha)
        {
            DateTime? fechaMinima = Session["fechaMinima"] as DateTime?;
            DateTime? fechaMaxima = Session["fechaMaxima"] as DateTime?;

            if (fechaMinima == null || fecha < fechaMinima)
                Session["fechaMinima"] = fecha;

            if (fechaMaxima == null || fecha > fechaMaxima)
                Session["fechaMaxima"] = fecha;
        }

        public bool validarFechas(string fechaIniFiltro, string fechaFinFiltro)
        {
            if ((fechaIniFiltro!="" && string.IsNullOrEmpty(fechaFinFiltro)) || (fechaFinFiltro!="" && string.IsNullOrEmpty(fechaIniFiltro)))
            {
                mostrarModalErrorEvento("VALIDACION", "Por favor, complete la fecha faltante.");
                return false;
            }

            if (DateTime.TryParse(fechaIniFiltro, out DateTime fechaInicio) && DateTime.TryParse(fechaFinFiltro, out DateTime fechaFin))
            {
                if(fechaInicio > fechaFin)
                {
                    mostrarModalErrorEvento("VALIDACION","La fecha de inicio no puede ser mayor a la fecha fin");
                    return false;
                }
            }
            return true;
        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            string fechaIniFiltro = txtFechaInicioFiltro.Text;
            string fechaFinFiltro = txtFechaFinFiltro.Text;

            if(!validarFechas(fechaIniFiltro,fechaFinFiltro)) return;
            buscarEventosPorFechasResponse rsp = null;
            if (!string.IsNullOrEmpty(fechaFinFiltro) && !string.IsNullOrEmpty(fechaIniFiltro))
            {
                rsp = eventoWS.buscarEventosPorFechas(new buscarEventosPorFechasRequest(fechaIniFiltro, fechaFinFiltro));
                if (rsp.@return != null)
                {
                    realizarPaginado(rsp.@return);
                }
                else
                {
                    mostrarModalErrorEvento("RESULTADO DE BUSQUEDA", "No se encontraron eventos siguiendo el cirterio de los filtros seleccionados. Se listarán todos los eventos.");
                    CargarEventos();
                }
            }
            else
            {
                mostrarModalErrorEvento("RESULTADO DE BUSQUEDA", "No se detectó ninguna fecha. Se listarán todos los eventos.");
                CargarEventos();
            }
            
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string id = btn.CommandArgument;
            hdnIdEvento.Value = id;
            ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "mostrarModal",
            $"var modal = new bootstrap.Modal(document.getElementById('{modalConfirmacionEliminado.ClientID}')); modal.show();",
            true
            );
        }

        protected void btnConfirmarEliminado_Click(object sender, EventArgs e)
        {
            int idEvento = int.Parse(hdnIdEvento.Value);

            // Lógica para eliminar...
            Boolean eliminado = eventoWS.eliminarLogico(new eliminarLogicoRequest(idEvento)).@return;
            if (!eliminado)
            {
                // error al eliminar
                mostrarModalErrorEvento("VENTANA DE ERROR", "El evento no fue eliminado correctamente.");
            }
            else
            {
                mostrarModalExitoEvento("VENTANA DE ÉXITO", "El evento fue eliminado correctamente.");
            }
            CargarEventos();
        }

        protected void ddlProvEditar_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cuando la provincia cambia, debemos listar los distritos!!!
            if (!string.IsNullOrEmpty(ddlProvEditar.SelectedValue))
            {
                int idProvincia = int.Parse(ddlProvEditar.SelectedValue);
                listarDistritosFiltradosResponse responseDistrito = distWS.listarDistritosFiltrados(
                    new listarDistritosFiltradosRequest(idProvincia)
                );

                ddlDistEditar.DataSource = responseDistrito.@return;
                ddlDistEditar.DataTextField = "Nombre";
                ddlDistEditar.DataValueField = "IdDistrito";
                ddlDistEditar.DataBind();
                ddlDistEditar.Items.Insert(0, new ListItem("Seleccione un distrito", ""));
                if (!ddlDistEditar.Enabled) ddlDistEditar.Enabled = true;
            }
            else
            {
                limpiarDistEdicion();
            }
            abrirModalEdicion();
        }

        protected void ddlDistEditar_SelectedIndexChanged(object sender, EventArgs e)
        {
            abrirModalEdicion();
        }

        protected void ddlDepasEditar_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cuando la provincia cambia, debemos listar los distritos!!!
            if (!string.IsNullOrEmpty(ddlDepasEditar.SelectedValue))
            {
                int idDepa = int.Parse(ddlDepasEditar.SelectedValue);
                listarProvinciaPorDepaResponse responseDepas = provWS.listarProvinciaPorDepa(
                    new listarProvinciaPorDepaRequest(idDepa)
                );

                ddlProvEditar.DataSource = responseDepas.@return;
                ddlProvEditar.DataTextField = "Nombre";
                ddlProvEditar.DataValueField = "IdProvincia";
                ddlProvEditar.DataBind();
                ddlProvEditar.Items.Insert(0, new ListItem("Seleccione una provincia", ""));
                if (!ddlProvEditar.Enabled) ddlProvEditar.Enabled=true;
            }
            else
            {
                ddlProvEditar.Items.Clear();
                ddlProvEditar.Items.Insert(0, new ListItem("Seleccione un departamento primero", ""));
            }
            limpiarDistEdicion();
            abrirModalEdicion();
        }

        public void limpiarDistEdicion()
        {
            // reiniciar distritos
            ddlDistEditar.Items.Clear();
            ddlDistEditar.Items.Insert(0, new ListItem("Seleccione una provincia primero", ""));
        }

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int idEvento = int.Parse(btn.CommandArgument);
            hdnIdEvento.Value = idEvento.ToString();
            funcionesEditar.Clear();

            eventoDTO eventoEditar = eventoWS.buscarEventoDTOporID(new buscarEventoDTOporIDRequest(idEvento)).@return;
            CargarDatosEventoEnFormulario(eventoEditar);
            CargarUbicacionEventoEnFormulario(eventoEditar);
            CargarFuncionesEventoEnFormulario(idEvento);

            abrirModalEdicion();
        }
        private void CargarDatosEventoEnFormulario(eventoDTO eventoEditar)
        {
            txtNombreEditar.Text = eventoEditar.nombre;
            txtDescEditar.Text = eventoEditar.descripcion;
            txtUbiEditar.Text = eventoEditar.ubicacion;
            txtRefEditar.Text = eventoEditar.referencia;
            txtPrecioEditar.Text = eventoEditar.precioEntrada.ToString();
            txtDispoEditar.Text = eventoEditar.cantEntradasDispo.ToString();
            txtVendEditar.Text = eventoEditar.cantEntradasVendidas.ToString();
        }

        private void CargarUbicacionEventoEnFormulario(eventoDTO eventoEditar)
        {
            ddlDepasEditar.DataTextField = "Nombre";
            ddlDepasEditar.DataValueField = "IdDepartamento";
            ddlDepasEditar.Items.Insert(0, new ListItem(eventoEditar.nombreProv.ToString(), eventoEditar.idDepa.ToString()));
            ddlDepasEditar.SelectedValue = eventoEditar.idDepa.ToString();
            ddlDepasEditar.Enabled = false;

            ddlProvEditar.DataTextField = "Nombre";
            ddlProvEditar.DataValueField = "IdProvincia";
            ddlProvEditar.Items.Insert(0, new ListItem(eventoEditar.nombreProv.ToString(), eventoEditar.idProv.ToString()));
            ddlProvEditar.SelectedValue = eventoEditar.idProv.ToString();
            ddlProvEditar.Enabled = false;

            ddlDistEditar.DataTextField = "Nombre";
            ddlDistEditar.DataValueField = "IdDistrito";
            ddlDistEditar.Items.Insert(0, new ListItem(eventoEditar.nombreDist.ToString(), eventoEditar.idDist.ToString()));
            ddlDistEditar.SelectedValue = eventoEditar.idDist.ToString();
            ddlDistEditar.Enabled = false;
        }

        private void CargarFuncionesEventoEnFormulario(int idEvento)
        {
            var listaFunciones = funcionWS.listarFuncionesPorIdEvento(new listarFuncionesPorIdEventoRequest(idEvento)).@return;

            var listaFormateada = listaFunciones.Select(f => new
            {
                Texto = $"{f.fecha:yyyy-MM-dd} - {f.horaInicio.Substring(0, 5)} a {f.horaFin.Substring(0, 5)}",
                Valor = f.idFuncion
            }).ToList();

            ddlFuncEditar.DataSource = listaFormateada;
            ddlFuncEditar.DataTextField = "Texto";
            ddlFuncEditar.DataValueField = "Valor";
            ddlFuncEditar.DataBind();
            ddlFuncEditar.Items.Insert(0, new ListItem("Presione para ver las funciones del evento", ""));
        }


        public void abrirModalEdicion()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "abrirModalEditar",
            "var modalEvento = new bootstrap.Modal(document.getElementById('modalEditarEvento')); modalEvento.show();", true);
        }

        protected void btnAgregarFuncionEditar_Click(object sender, EventArgs e)
        {
            string fecha = txtFechaEditar.Text.Trim();
            string horaInicio = txtHoraIniEditar.Text.Trim();
            string horaFin = txtHoraFinEditar.Text.Trim();

            string valor = $"{fecha}_{horaInicio}_{horaFin}";
            string texto = $"{fecha} - {horaInicio} a {horaFin}";

            // 2 hace referencia al Modal Editar
            if (!ValidaFuncion(fecha, horaInicio, horaFin, valor, MODAL_EDITAR)) return;
            DateTime.TryParse(fecha, out DateTime fechaSeleccionada);

            funcion nuevaFuncion = new funcion
            {
                fecha = fecha,
                horaInicio = horaInicio,
                horaFin = horaFin,
                evento = new evento { idEvento = int.Parse(hdnIdEvento.Value) }
            };

            // Obtener la lista del ViewState
            var listaActual = funcionesEditar;
            listaActual.Add(nuevaFuncion);
            funcionesEditar = listaActual; // guardar de nuevo en ViewState

            ddlFuncEditar.Items.Add(new ListItem(texto, valor));
            ActualizarFechasMinMax(fechaSeleccionada);
            LimpiarFunciones();
            abrirModalEdicion();
        }
        public void ActualizarFechasMinMaxEditar()
        {
            DateTime fechaMin = DateTime.MaxValue;
            DateTime fechaMax = DateTime.MinValue;

            foreach (ListItem item in ddlFuncEditar.Items)
            {
                string texto = item.Text;

                // Validación rápida: salto si el ítem está vacío o no tiene el formato esperado
                if (string.IsNullOrWhiteSpace(texto) || !texto.Contains("-")) continue;

                // Separar por " - " para obtener la fecha
                string[] partes = texto.Split(new[] { " - " }, StringSplitOptions.None);
                if (partes.Length < 2) continue;

                string fechaStr = partes[0].Trim();

                if (DateTime.TryParse(fechaStr, out DateTime fecha))
                {
                    if (fecha < fechaMin) fechaMin = fecha;
                    if (fecha > fechaMax) fechaMax = fecha;
                }
            }

            // Guardar en sesión si encontramos alguna fecha válida
            if (fechaMin != DateTime.MaxValue)
                Session["fechaMinima"] = fechaMin;

            if (fechaMax != DateTime.MinValue)
                Session["fechaMaxima"] = fechaMax;
        }

        protected void btnAceptarEditar_Click(object sender, EventArgs e)
        {
            ActualizarFechasMinMaxEditar();

            if (!ValidarEdicionEvento()) return;

            evento eventoActualizar = ConstruirEventoDesdeFormulario();
            bool actualizado = eventoWS.actualizarEvento(new actualizarEventoRequest(eventoActualizar)).@return;

            if (actualizado)
            {
                if (!InsertarFuncionesEditar())
                {
                    mostrarModalErrorEvento("VENTANA DE ERROR", "Error al insertar la función");
                    return;
                }

                mostrarModalExitoEvento("VENTANA DE ÉXITO", "EVENTO actualizado exitosamente");
                CargarEventos();
            }
            else
            {
                mostrarModalErrorEvento("VENTANA DE ERROR", "Ocurrió un problema al actualizar el EVENTO");
            }
        }
        private bool ValidarEdicionEvento()
        {
            return ValidarDatosEvento(
                txtNombreEditar.Text,
                txtDescEditar.Text,
                txtUbiEditar.Text,
                txtPrecioEditar.Text,
                txtDispoEditar.Text,
                txtVendEditar.Text,
                txtRefEditar.Text,
                ddlDepasEditar.Text,
                ddlDistEditar.Text,
                ddlProvEditar.Text,
                true,
                MODAL_EDITAR
            );
        }

        private evento ConstruirEventoDesdeFormulario()
        {
            string fechaInicioEvento = Session["fechaMinima"].ToString();
            string fechaFinEvento = Session["fechaMaxima"].ToString();

            return new evento
            {
                idEvento = int.Parse(hdnIdEvento.Value),
                fecha_inicio = fechaInicioEvento.Split(' ')[0],
                fecha_fin = fechaFinEvento.Split(' ')[0],
                nombre = txtNombreEditar.Text,
                descripcion = txtDescEditar.Text,
                ubicacion = txtUbiEditar.Text,
                distrito = new distrito
                {
                    idDistrito = int.Parse(ddlDistEditar.SelectedValue)
                },
                precioEntrada = double.Parse(txtPrecioEditar.Text),
                cantEntradasVendidas = int.Parse(txtVendEditar.Text),
                cantEntradasDispo = int.Parse(txtDispoEditar.Text),
                referencia = txtRefEditar.Text
            };
        }

        private bool InsertarFuncionesEditar()
        {
            foreach (funcion f in funcionesEditar)
            {
                int idFuncion = funcionWS.insertarFuncion(new insertarFuncionRequest(f)).@return;
                if (idFuncion < 1)
                {
                    return false;
                }
            }
            return true;
        }

        public void mostrarModalExitoEvento(string titulo, string mensaje)
        {
            string script = $@"
                Sys.Application.add_load(function () {{
                    mostrarModalExito('{titulo}', '{mensaje}');
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalExito", script, true);
        }

        public void mostrarModalErrorEvento(string titulo, string mensaje)
        {
            string script = $@"
                Sys.Application.add_load(function () {{
                    mostrarModalError('{titulo}', '{mensaje}');
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", script, true);
        }

        protected void btnEditUbigeo_Click(object sender, EventArgs e)
        {
            ddlDepasEditar.Enabled = true;
            ddlDepasEditar.DataSource = depaWS.listarDepas(new listarDepasRequest()).@return;
            ddlDepasEditar.DataBind();
            ddlDepasEditar.Items.Insert(0, new ListItem("Seleccione un departamento", ""));
            abrirModalEdicion();
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

        protected void rptEventos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = e.Item.DataItem;

                // Obtener la fecha
                string estado = Convert.ToString(DataBinder.Eval(dataItem, "Activo"));
                bool eliminado = estado == "69";

                // Verificar si pasaron 5 años
                bool habilitado = !eliminado;

                // Obtener el botón y asignar su estado
                Button btnEliminar = (Button)e.Item.FindControl("btnEliminar");
                if (btnEliminar != null)
                {
                    btnEliminar.ToolTip = habilitado ?
                        "Puedes eliminar este evento"
                        : "Este evento ya fue eliminado.";
                }
                if (!habilitado)
                {
                    btnEliminar.Attributes["onclick"] = "return false;"; // Evita que haga postback
                    btnEliminar.CssClass += " disabled-button"; // Agrega estilo visual
                }
            }
        }
    }
}