using DocumentFormat.OpenXml.Math;
using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SirgepPresentacion.Presentacion.Ventas.Reserva
{
    public partial class ListaReservasComprador : System.Web.UI.Page
    {
        private ReservaWSClient reservaWS;

        private detalleReservaDTO[] listaReservasComprador
        {
            get => ViewState["listaReservasComprador"] as detalleReservaDTO[] ?? Array.Empty<detalleReservaDTO>();
            set => ViewState["listaReservasComprador"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            reservaWS = new ReservaWSClient();

            if (!IsPostBack)
            {
                CargarReservasIniciales();
            }
        }

        private void CargarReservasIniciales()
        {
            int idComprador = ObtenerIdCompradorDesdeSesion();
            listaReservasComprador = reservaWS.listarReservasPorComprador(idComprador, null, null, null);
            RecargarGrilla(listaReservasComprador);
        }

        protected void GvListaReservasComprador_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GvListaReservasComprador.PageIndex = e.NewPageIndex;
            RecargarGrilla(listaReservasComprador);
        }

        private void RecargarGrilla(detalleReservaDTO[] lista)
        {
            GvListaReservasComprador.DataSource = lista;
            GvListaReservasComprador.DataBind();
        }

        protected void btnDescargar_Click(object sender, EventArgs e)
        {
            string script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);
            int idComprador = ObtenerIdCompradorDesdeSesion();
            ObtenerFiltros(out DateTime? fechaInicio, out DateTime? fechaFin, out string estado);

            bool resultado = reservaWS.crearLibroExcelReservas(
                idComprador,
                fechaInicio?.ToString("yyyy-MM-dd"),
                fechaFin?.ToString("yyyy-MM-dd"),
                estado
            );

            if (resultado)
                MostrarModalExito("Descarga exitosa", "La lista de reservas fue descargada correctamente.");
            else
                MostrarModalError("Error en la descarga", GenerarMensajeError(fechaInicio, fechaFin, estado));

            RefrescarGrillaYFiltros();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            string script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);
            ObtenerFiltros(out DateTime? fechaInicio, out DateTime? fechaFin, out string estado);

            if (fechaInicio != null && fechaFin != null && fechaInicio > fechaFin)
            {
                lblMensaje.Text = "La fecha de inicio no puede ser mayor que la fecha de fin.";
                return;
            }

            CargarDatosFiltrados(fechaInicio, fechaFin, estado);
            MostrarMensaje(fechaInicio, fechaFin, estado);
        }

        protected void btnMostrarTodos_Click(object sender, EventArgs e)
        {
            string script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);
            txtFechaInicio.Text = txtFechaFin.Text = "";
            rblEstados.ClearSelection();
            RecargarGrilla(listaReservasComprador);
            lblMensaje.Text = "Mostrando todas las reservas de hasta un año";
        }

        protected void btnConfirmarAccion_Click(object sender, EventArgs e)
        {
            string script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);
            int id = int.Parse(hdnReservaAEliminar.Value);
            reservaWS.cancelarReserva(id);
            CargarDatosFiltrados(null, null, null);

            MostrarModalExito("Reserva cancelada", "Reserva cancelada exitosamente.");
        }

        protected void BtnAbrir_Click(object sender, EventArgs e)
        {
            string idConstancia = ((LinkButton)sender).CommandArgument;
            string script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);
            Response.Redirect($"/Presentacion/Ventas/Reserva/ConstanciaReserva.aspx?idConstancia={idConstancia}");
        }

        private void CargarDatosFiltrados(DateTime? fechaInicio, DateTime? fechaFin, string estado)
        {
            int idComprador = ObtenerIdCompradorDesdeSesion();
            var lista = reservaWS.listarReservasPorComprador(
                idComprador,
                fechaInicio?.ToString("yyyy-MM-dd"),
                fechaFin?.ToString("yyyy-MM-dd"),
                estado
            );

            if (lista == null || lista.Length == 0)
            {
                lista = Array.Empty<detalleReservaDTO>();
                MostrarModalError("Error en la búsqueda", "No se encontró alguna reserva.");
            }

            RecargarGrilla(lista);
        }

        private int ObtenerIdCompradorDesdeSesion()
        {
            return int.Parse(Session["idUsuario"].ToString());
        }

        private void ObtenerFiltros(out DateTime? fechaInicio, out DateTime? fechaFin, out string estado)
        {
            fechaInicio = DateTime.TryParse(txtFechaInicio.Text, out var fi) ? fi : (DateTime?)null;
            fechaFin = DateTime.TryParse(txtFechaFin.Text, out var ff) ? ff : (DateTime?)null;

            string valor = rblEstados.SelectedValue?.Trim();
            estado = (valor == "Vigentes" || valor == "Finalizadas" || valor == "Canceladas") ? valor : null;
        }

        private void MostrarMensaje(DateTime? fechaInicio, DateTime? fechaFin, string estado)
        {
            var mensaje = new StringBuilder("Reservas filtradas de hasta un año");

            if (fechaInicio != null && fechaFin != null)
                mensaje.Append($" entre {fechaInicio:dd/MM/yyyy} y {fechaFin:dd/MM/yyyy}");
            else if (fechaInicio != null)
                mensaje.Append($" desde {fechaInicio:dd/MM/yyyy}");
            else if (fechaFin != null)
                mensaje.Append($" hasta {fechaFin:dd/MM/yyyy}");

            if (!string.IsNullOrEmpty(estado))
                mensaje.Append(" con estado " + estado);

            lblMensaje.Text = mensaje.ToString();
        }

        private void MostrarModalExito(string titulo, string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModal", $@"
                setTimeout(function() {{
                    mostrarModalExito('{titulo}', '{mensaje}');
                }}, 1000);", true);
        }

        private void MostrarModalError(string titulo, string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModal", $@"
                setTimeout(function() {{
                    mostrarModalError('{titulo}', '{mensaje}');
                }}, 1000);", true);
        }

        private void RefrescarGrillaYFiltros()
        {
            RecargarGrilla(listaReservasComprador);
            txtFechaInicio.Text = txtFechaFin.Text = "";
            rblEstados.ClearSelection();
            lblMensaje.Text = "Mostrando todas las reservas de hasta un año";
        }

        private string GenerarMensajeError(DateTime? fechaInicio, DateTime? fechaFin, string estado)
        {
            return (fechaInicio != null || fechaFin != null || estado != null)
                ? "No se pudo descargar la lista de reservas. Verifica los filtros seleccionados."
                : "No se pudo descargar la lista de reservas. No posees alguna reserva.";
        }
    }
}