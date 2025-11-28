using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SirgepPresentacion.ReferenciaDisco;

namespace SirgepPresentacion.Presentacion.Usuarios.Comprador
{
    public partial class ListaComprador : System.Web.UI.Page
    {
        private CompradorWS compradorWS;
        private List<compradorDTO> compradoresFiltrados
        {
            get => ViewState["compradoresFiltrados"] as List<compradorDTO> ?? new List<compradorDTO>();
            set => ViewState["compradoresFiltrados"] = value;
        }
        private const int TAMANIO_PAGINA = 10;
        private int PaginaActual
        {
            get => ViewState["PaginaActual"] != null ? (int)ViewState["PaginaActual"] : 1;
            set => ViewState["PaginaActual"] = value;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            compradorWS = new CompradorWSClient();

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarUsuarios();

            }
        }
        private void CargarUsuarios(string filtro = "")
        {
            var request = new listarCompradoresDTORequest();
            var response = compradorWS.listarCompradoresDTO(request);
            var compradores = response.@return;

            filtro = filtro?.ToLower() ?? "";

            var listaFiltrada = compradores
                .Where(c =>
                    string.IsNullOrEmpty(filtro) ||
                    (c.nombres != null && c.nombres.ToLower().Contains(filtro)) ||
                    (c.primerApellido != null && c.primerApellido.ToLower().Contains(filtro)) ||
                    (c.segundoApellido != null && c.segundoApellido.ToLower().Contains(filtro)) ||
                    (c.correo != null && c.correo.ToLower().Contains(filtro)) ||
                    (c.numeroDocumento != null && c.numeroDocumento.ToLower().Contains(filtro))
                )
                .ToList();

            compradoresFiltrados = listaFiltrada;

            if (listaFiltrada.Count == 0 && !string.IsNullOrEmpty(filtro))
            {
                CargarUsuarios(""); // recarga sin filtro
                string script = "setTimeout(function(){ mostrarModalError('Búsqueda sin resultados.', 'No se encontraron coincidencias. Se mostrarán todos los usuarios.'); }, 300);";
                ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModalError", script, true);
                return;
            }
            MostrarPaginado();
        }
        private void MostrarPaginado()
        {
            int total = compradoresFiltrados.Count;
            int inicio = (PaginaActual - 1) * TAMANIO_PAGINA;
            int totalPaginas = (int)Math.Ceiling((double)total / TAMANIO_PAGINA);

            var paginaActual = compradoresFiltrados
                .Skip(inicio)
                .Take(TAMANIO_PAGINA)
                .Select(c =>
                {
                    DateTime? fechaUltima = null;
                    if (DateTime.TryParse(c.fechaUltimaCompra, out DateTime fecha))
                        fechaUltima = fecha;

                    return new
                    {
                        c.idComprador,
                        c.nombres,
                        c.primerApellido,
                        c.segundoApellido,
                        c.tipoDocumento,
                        c.numeroDocumento,
                        c.correo,
                        fechaUltimaCompra = string.IsNullOrEmpty(c.fechaUltimaCompra) ? "No ha comprado aún" : c.fechaUltimaCompra,
                        PuedeEliminar = PuedeEliminar(fechaUltima)
                    };
                })
                .ToList();

            GvListaCompradores.DataSource = paginaActual;
            GvListaCompradores.DataBind();

            lblPaginaActual.Text = $"Página {PaginaActual} / {totalPaginas}";
            btnAnterior.Enabled = PaginaActual > 1;
            btnSiguiente.Enabled = inicio + TAMANIO_PAGINA < total;
        }


        protected void btnAnterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                MostrarPaginado();
            }
        }

        protected void btnSiguiente_Click(object sender, EventArgs e)
        {
            PaginaActual++;
            MostrarPaginado();
        }
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarUsuarios(txtBusqueda.Text.Trim());
        }

        protected void btnConfirmarAccion_Click(object sender, EventArgs e)
        {
            int id = int.Parse(hdnIdAEliminar.Value);
            var request = new eliminarUsuarioCompradorRequest { idComprador = id };
            compradorWS.eliminarUsuarioComprador(request);
            CargarUsuarios();
        }

        // Lógica para determinar si puede eliminarse (más de 3 años desde la última compra)
        private bool PuedeEliminar(DateTime? fechaUltimaCompra)
        {
            if (!fechaUltimaCompra.HasValue)
                return false;
            return fechaUltimaCompra.Value.AddYears(3) <= DateTime.Now;
        }



    }
}