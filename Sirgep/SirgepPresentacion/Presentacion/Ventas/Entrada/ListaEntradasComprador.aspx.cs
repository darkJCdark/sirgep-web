
using DocumentFormat.OpenXml.Math;
using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SirgepPresentacion.Presentacion.Ventas.Entrada
{
    public partial class ListaEntradasComprador : System.Web.UI.Page
    {
        private EntradaWSClient entradaWS;

        private detalleEntradaDTO[] listaEntradasComprador
        {
            get => ViewState["listaEntradasComprador"] as detalleEntradaDTO[] ?? Array.Empty<detalleEntradaDTO>();
            set => ViewState["listaEntradasComprador"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            entradaWS = new EntradaWSClient();

            if (!IsPostBack)
            {
                CargarEntradasIniciales();
            }
        }

        private void CargarEntradasIniciales()
        {
            int idComprador = ObtenerIdCompradorDesdeSesion();
            listaEntradasComprador = entradaWS.listarEntradasPorComprador(idComprador, null, null, null);
            RecargarGrid(listaEntradasComprador);
        }

        private void RecargarGrid(detalleEntradaDTO[] lista)
        {
            GvListaEntradasComprador.DataSource = lista;
            GvListaEntradasComprador.DataBind();
        }

        protected void GvListaEntradasComprador_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GvListaEntradasComprador.PageIndex = e.NewPageIndex;
            RecargarGrid(listaEntradasComprador);
        }
        //        mostrarModalCarga('Cargando...', 'Redireccionando a Gestión de Espacios.');
        protected void btnDescargar_Click(object sender, EventArgs e)
        {
            int idComprador = ObtenerIdCompradorDesdeSesion();
            ObtenerFiltros(out DateTime? fechaInicio, out DateTime? fechaFin, out string estado);
            bool resultado = entradaWS.crearLibroExcelEntradas(
                idComprador,
                fechaInicio?.ToString("yyyy-MM-dd"),
                fechaFin?.ToString("yyyy-MM-dd"),
                estado
            );
            string script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);
            RefrescarGrillaYFiltros();
            if (resultado)
                MostrarModalExito("Descarga exitosa", "La lista de entradas fue descargada correctamente.");
            else
                MostrarModalError("Error en la descarga", GenerarMensajeError(fechaInicio, fechaFin, estado));
        }

        private void RefrescarGrillaYFiltros()
        {
            RecargarGrid(listaEntradasComprador);
            lblMensaje.Text = "Mostrando todas las reservas de hasta un año";
            txtFechaInicio.Text = txtFechaFin.Text = "";
            rblEstados.ClearSelection();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            ObtenerFiltros(out DateTime? fechaInicio, out DateTime? fechaFin, out string estado);
            if (fechaInicio != null && fechaFin != null && fechaInicio > fechaFin)
            {
                lblMensaje.Text = "La fecha de inicio no puede ser mayor que la fecha de fin.";
                return;
            }
            string script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);
            CargarDatosFiltrados(fechaInicio, fechaFin, estado);
            MostrarMensaje(fechaInicio, fechaFin, estado);
        }

        protected void btnMostrarTodos_Click(object sender, EventArgs e)
        {
            txtFechaInicio.Text = txtFechaFin.Text = "";
            rblEstados.ClearSelection();
            RecargarGrid(listaEntradasComprador);
            lblMensaje.Text = "Mostrando todas las entradas de hasta un año";
            string script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);
        }

        private void CargarDatosFiltrados(DateTime? fechaInicio, DateTime? fechaFin, string estado)
        {
            int idComprador = ObtenerIdCompradorDesdeSesion();
            var lista = entradaWS.listarEntradasPorComprador(
                idComprador,
                fechaInicio?.ToString("yyyy-MM-dd"),
                fechaFin?.ToString("yyyy-MM-dd"),
                estado
            );
            if (lista == null || lista.Length == 0)
            {
                lista = Array.Empty<detalleEntradaDTO>();
                MostrarModalError("Error en la búsqueda", "No se encontró alguna entrada.");
            }
            RecargarGrid(lista);
        }

        private int ObtenerIdCompradorDesdeSesion()
        {
            return int.Parse(Session["idUsuario"].ToString());
        }

        protected void ObtenerFiltros(out DateTime? fechaInicio, out DateTime? fechaFin, out string estado)
        {
            fechaInicio = DateTime.TryParse(txtFechaInicio.Text, out var fi) ? fi : (DateTime?)null;
            fechaFin = DateTime.TryParse(txtFechaFin.Text, out var ff) ? ff : (DateTime?)null;

            var valor = rblEstados.SelectedValue?.Trim();
            estado = (valor == "Vigentes" || valor == "Finalizadas") ? valor : null;
        }

        protected void MostrarMensaje(DateTime? fechaInicio, DateTime? fechaFin, string estado)
        {
            var mensaje = new StringBuilder("Entradas filtradas de hasta un año");

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

        private string GenerarMensajeError(DateTime? fechaInicio, DateTime? fechaFin, string estado)
        {
            return (fechaInicio != null || fechaFin != null || estado != null)
                ? "No se pudo descargar la lista de entradas. Verifica los filtros seleccionados."
                : "No se pudo descargar la lista de entradas. No posees alguna entrada.";
        }

        protected void BtnAbrir_Click(object sender, EventArgs e)
        {
            string idConstancia = ((LinkButton)sender).CommandArgument;
            string script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);
            Response.Redirect($"/Presentacion/Ventas/Entrada/ConstanciaEntrada.aspx?idConstancia={idConstancia}");
        }
    }
}