using DocumentFormat.OpenXml.Math;
using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SirgepPresentacion.Presentacion.Ventas.Entrada
{
    public partial class ListaEntradasAdministrador : System.Web.UI.Page
    {
        private EntradaWS entradaWS;

        private const int TAMANIO_PAGINA = 10; // Cambia según límite
        private int PaginaActual
        {
            get
            {
                return ViewState["PaginaActual"] != null ? (int)ViewState["PaginaActual"] : 1;
            }
            set
            {
                ViewState["PaginaActual"] = value;
            }
        }

        protected void Page_Init()
        {
            entradaWS = new EntradaWSClient();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PaginaActual = 1;
                CargarEntradas();
            }
        }

        public void realizarPaginado(detalleEntradaDTO[] entradas)
        {
            var todasLasEntradas = entradas.ToList();
            int total = todasLasEntradas.Count;
            int inicio = (PaginaActual - 1) * TAMANIO_PAGINA;
            int totalPaginas = (int)Math.Ceiling((double)total / TAMANIO_PAGINA);
            var entradasPaginadas = todasLasEntradas
                .Skip(inicio)
                .Take(TAMANIO_PAGINA)
                .ToList();

            rptEntradas.DataSource = entradasPaginadas;
            rptEntradas.DataBind();

            lblPaginaActual.Text = $"Página {PaginaActual} / {totalPaginas}";
            btnAnterior.Enabled = PaginaActual > 1;
            btnSiguiente.Enabled = inicio + TAMANIO_PAGINA < total;
        }
        public void CargarEntradas()
        {
            detalleEntradaDTO[] entradas = entradaWS.listarDetalleEntradas(new listarDetalleEntradasRequest()).@return; ; // Lista completa
            realizarPaginado(entradas);
        }

        protected void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            string texto = txtBusqueda.Text;
            if (texto == "" || texto==null)
            {
                PaginaActual = 1;
                CargarEntradas();
                return;
            
            }
            detalleEntradaDTO[] entradas = entradaWS.buscarEntradasPorTexto(new buscarEntradasPorTextoRequest(texto)).@return;
            if(entradas == null)
            {
                mostrarModalErrorEntrada("RESULTADOS DE BUSQUEDA", "No se encontraron entradas con los parámetros actuales. Se litarán todas las entradas.");
                CargarEntradas();
                return;
            }
            realizarPaginado(entradas);
        }

        protected void btnEliminarConfirmado_Click(object sender, EventArgs e)
        {
            // Aquí el usuario ha confirmado que quiere eliminar el registro
            int idEntradaInt = int.Parse(hdnEntrada.Value);
            Boolean entradaEliminada = entradaWS.eliminarLogicoEntrada(new eliminarLogicoEntradaRequest(idEntradaInt)).@return;
            if (entradaEliminada)
            {
                mostrarModalExitoEntrada("VENTANA DE ÉXITO", "La entrada ha sido eliminada satisfactoriamente.");
                CargarEntradas();
                return;
            }
            mostrarModalErrorEntrada("VENTANA DE ERROR", "Ocurrió un error al eliminar la entrada.");
        }

        protected void btnAnterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                if (!string.IsNullOrEmpty(txtBusqueda.Text))
                {
                    txtBusqueda_TextChanged(sender, e);
                    return;
                }
                CargarEntradas();
            }
        }
        protected void btnSiguiente_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            if (!string.IsNullOrEmpty(txtBusqueda.Text))
            {
                txtBusqueda_TextChanged(sender, e);
                return;
            }
            CargarEntradas();
        }
        public void mostrarModalExitoEntrada(string titulo, string mensaje)
        {
            string script = $@"
                Sys.Application.add_load(function () {{
                    mostrarModalExito('{titulo}', '{mensaje}');
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalExito", script, true);
        }
        public void mostrarModalErrorEntrada(string titulo, string mensaje)
        {
            string script = $@"
                Sys.Application.add_load(function () {{
                    mostrarModalExito('{titulo}', '{mensaje}');
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", script, true);
        }
        public void mostrarModalConfEntrada()
        {
            string script = "mostrarModalConfEntrada();";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModal", script, true);
        }
        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            string commandArg = ((Button)sender).CommandArgument;
            string[] partes = commandArg.Split('|');

            string fechaCompraStr = partes[0].Trim();
            string idEntrada = partes[1].Trim();

            hdnEntrada.Value = idEntrada;
            mostrarModalConfEntrada(); // Confirmación real de eliminación
        }
        protected void btnProbarModal_Click(object sender, EventArgs e)
        {
            mostrarModalExitoEntrada("Éxito", "El modal se está mostrando correctamente.");
        }
        protected void rptEntradas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DateTime fechaConstancia;
                var dataItem = e.Item.DataItem;

                // Obtener la fecha
                fechaConstancia = Convert.ToDateTime(DataBinder.Eval(dataItem, "fechaConstancia"));
                string estado = Convert.ToString(DataBinder.Eval(dataItem, "Estado"));
                bool cancelado = estado == "69";

                // Verificar si pasaron 5 años
                bool habilitado = (DateTime.Now - fechaConstancia).TotalDays >= 5 * 365 && !cancelado;

                // Obtener el botón y asignar su estado
                Button btnEliminar = (Button)e.Item.FindControl("btnEliminar");
                if (btnEliminar != null)
                {
                    btnEliminar.ToolTip = habilitado ?
                        "Puedes eliminar esta entrada, ya que han pasado 5 años desde la compra"
                        : "No puedes eliminar esta entrada. Aún no han pasado 5 años desde la compra.";
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