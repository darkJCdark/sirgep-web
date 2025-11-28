using DocumentFormat.OpenXml.Office2016.Drawing.Charts;
using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SirgepPresentacion.Presentacion.Ventas.Reserva
{
    public partial class ListaReservasAdministrador : System.Web.UI.Page
    {
        private const int ReservasPorPagina = 20;
        private List<reservaDTO> Reservas
        {
            get => ViewState["Reservas"] as List<reservaDTO> ?? new List<reservaDTO>();
            set => ViewState["Reservas"] = value;
        }

        private ReservaWSClient client;
        private DistritoWSClient distrClient;

        protected void Page_Init(object sender, EventArgs e)
        {
            client = new ReservaWSClient();
            distrClient = new DistritoWSClient();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["PaginaActual"] = 1;

                // Solo carga los filtros, no las reservas
                ddlDistritos.DataSource = distrClient.listarTodosDistritos().ToList();
                ddlDistritos.DataTextField = "nombre";
                ddlDistritos.DataValueField = "idDistrito";
                ddlDistritos.DataBind();
                ddlDistritos.Items.Insert(0, new ListItem("Seleccione un distrito", ""));
                ddlDistritos.Visible = false;
                btnLimpiarFiltro.Visible = false;

                CargarPagina();
            }
        }

        protected void CargarPagina()
        {
            // Solo cargar todas las reservas si no hay datos en ViewState para evitar que se sobreescriban la lista filtrada
            if (Reservas == null || !Reservas.Any())
            {
                Reservas = client.listarTodasReservas().ToList();
            }

            int paginaActual = (int)(ViewState["PaginaActual"] ?? 1);
            var reservas = Reservas;

            int totalPaginas = (int)Math.Ceiling((double)reservas.Count / ReservasPorPagina);

            var reservasPagina = reservas
                .Skip((paginaActual - 1) * ReservasPorPagina)
                .Take(ReservasPorPagina)
                .ToList();

            gvReservas.DataSource = reservasPagina;
            gvReservas.DataBind();

            lblPagina.Text = $"Página {paginaActual} de {totalPaginas}";
            btnAnterior.Enabled = paginaActual > 1;
            btnSiguiente.Enabled = paginaActual < totalPaginas;
        }


        protected void ddlFiltros_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlFiltros.SelectedValue == "distrito")
            {
                txtFecha.Visible = false;
                ddlDistritos.Visible = true;
            }
            else
            {
                txtFecha.Visible = true;
                ddlDistritos.Visible = false;
            }
            CargarPagina();
        }

        protected void btnAnterior_Click(object sender, EventArgs e)
        {
            int paginaActual = (int)(ViewState["PaginaActual"] ?? 1);
            if (paginaActual > 1)
                ViewState["PaginaActual"] = paginaActual - 1;

            CargarPagina();
        }

        protected void btnSiguiente_Click(object sender, EventArgs e)
        {
            int paginaActual = (int)(ViewState["PaginaActual"] ?? 1);
            int totalPaginas = (int)Math.Ceiling((double)Reservas.Count / ReservasPorPagina);

            if (paginaActual < totalPaginas)
                ViewState["PaginaActual"] = paginaActual + 1;

            CargarPagina();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            string filtro = ddlFiltros.SelectedValue;
            string textoBusqueda = input_busqueda.Value.Trim();
            bool activo = chkActivos.Checked;

            try
            {
                List<reservaDTO> filtradas = new List<reservaDTO>();
                if (filtro == "fecha" && !string.IsNullOrWhiteSpace(txtFecha.Text))
                {
                    if (DateTime.TryParseExact(txtFecha.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))
                    {
                        string fechaStr = fecha.ToString("yyyy-MM-dd");
                        filtradas = client.listarReservaPorFecha(fechaStr, activo).ToList();
                    }
                }
                else if (filtro == "distrito" && ddlDistritos.SelectedValue != "")
                {
                    int idDistrito = int.Parse(ddlDistritos.SelectedValue);
                    filtradas = client.listarReservaPorDistrito(idDistrito, activo).ToList();
                }
                else
                {
                    var todas = client.listarTodasReservas().ToList();
                    filtradas = activo ? todas.Where(r => (char)r.activo == 'A').ToList() : todas;
                }

                Reservas = filtradas;
                ViewState["PaginaActual"] = 1;
                btnLimpiarFiltro.Visible = true;
                CargarPagina();
            }
            catch
            {
                Reservas = new List<reservaDTO>();
                mostrarModalErrorReserva("Sin resultados", "No se encontró alguna coincidencia");
                CargarPagina();
            }
        }
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string textoBusqueda = input_busqueda.Value.Trim().ToLower();

            try
            {
                // Siempre buscar dentro de la lista actual
                var listadoActual = Reservas;

                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    listadoActual = listadoActual.Where(r =>
                        ("#" + r.numReserva.ToString("D3")).ToLower().Contains(textoBusqueda) ||
                        r.fechaReserva.ToString("yyyy-MM-dd").Contains(textoBusqueda) ||
                        (r.distrito?.ToLower().Contains(textoBusqueda) ?? false) ||
                        (r.espacio?.ToLower().Contains(textoBusqueda) ?? false) ||
                        (r.correo?.ToLower().Contains(textoBusqueda) ?? false)
                    ).ToList();
                    btnLimpiarFiltro.Visible = true;
                }

                // Guardar resultado y mostrar modal si está vacío
                Reservas = listadoActual;
                ViewState["PaginaActual"] = 1;
                CargarPagina();

                if (!listadoActual.Any())
                {
                    mostrarModalErrorReserva("Sin resultados", "No se encontró alguna coincidencia.");
                    btnLimpiarFiltro.Visible = false;
                }
            }
            catch
            {
                Reservas = new List<reservaDTO>();
                mostrarModalErrorReserva("ERROR", "Ocurrió un error durante la búsqueda.");
                CargarPagina();
            }
        }

        protected void btnVerCalendario_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Presentacion/Usuarios/Administrador/CalendarioReservas.aspx");
        }

        protected void btnEliminarReserva_Click(object sender, EventArgs e)
        {
            int idReserva = int.Parse(((Button)sender).CommandArgument);

            // Buscar la reserva en la lista cargada
            var reserva = Reservas.FirstOrDefault(r => r.numReserva == idReserva);

            if (reserva == null)
            {
                mostrarModalErrorReserva("ERROR", "No se encontró la reserva.");
                return;
            }

            // Validar si han pasado al menos 5 años desde la fecha de constancia
            if (reserva.fechaConstancia.AddYears(5).Date > DateTime.Today)
            {
                mostrarModalErrorReserva("No permitido", "Solo se pueden eliminar reservas con más de 5 años de antigüedad.");
                return;
            }

            // Validar si está activo (asumo que 'A' == activo)
            /*if ((char)reserva.activo != 'A')
            {
                mostrarModalErrorReserva("No permitido", "La reserva no está activa.");
                return;
            }*/

            bool reservaEliminada = client.eliminarLogicoReserva(idReserva);

            if (reservaEliminada)
            {
                //Elimina la reserva del listado actual
                Reservas = Reservas.Where(r => r.numReserva != idReserva).ToList();

                mostrarModalExitoReserva("Éxito", "La reserva ha sido eliminada correctamente.");
            }
            else
            {
                mostrarModalErrorReserva("Error", "Ocurrió un problema al eliminar la reserva.");
            }

            CargarPagina();
        }

        public void mostrarModalExitoReserva(string titulo, string mensaje)
        {
            string script = $@"
                Sys.Application.add_load(function () {{
                    mostrarModalExito('{titulo}', '{mensaje}');
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalExito", script, true);
        }

        public void mostrarModalErrorReserva(string titulo, string mensaje)
        {
            string script = $@"
                Sys.Application.add_load(function () {{
                    mostrarModalExito('{titulo}', '{mensaje}');
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", script, true);
        }

        protected void gvReservas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var reserva = (reservaDTO)e.Row.DataItem;

                DateTime fechaConstancia = reserva.fechaConstancia;
                char estado = (char)reserva.activo;

                Button btnEliminar = (Button)e.Row.FindControl("btnEliminarReserva");

                if (btnEliminar != null)
                {
                    bool habilitado = fechaConstancia.AddYears(5).Date <= DateTime.Today;
                    btnEliminar.Enabled = habilitado;

                    btnEliminar.ToolTip = habilitado
                        ? "Puedes eliminar esta entrada, ya que han pasado 5 años desde la constancia"
                        : "No puedes eliminar esta entrada. Aún no han pasado 5 años desde la constancia";
                }
            }
        }

        protected void btnLimpiarFiltro_Click(object sender, EventArgs e)
        {
            // Limpiar controles de filtro
            ddlFiltros.SelectedIndex = 0;
            txtFecha.Text = "";
            ddlDistritos.SelectedIndex = 0;
            chkActivos.Checked = false;
            input_busqueda.Value = "";

            // Ocultar controles no necesarios según filtro inicial
            txtFecha.Visible = true;
            ddlDistritos.Visible = false;

            // Recargar todas las reservas
            Reservas = client.listarTodasReservas().ToList();
            ViewState["PaginaActual"] = 1;

            // Ocultar el botón de limpiar filtro
            btnLimpiarFiltro.Visible = false;

            CargarPagina();
        }
    }
}