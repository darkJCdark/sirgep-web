using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SirgepPresentacion.Presentacion.Usuarios.Comprador
{
    public partial class PerfilComprador : System.Web.UI.Page
    {
        private CompradorWSClient compradorWS;
        protected void Page_Load(object sender, EventArgs e)
        {
            compradorWS = new CompradorWSClient();
            if (!IsPostBack)
            {
                cargarPerfil();
            }
        }
        protected void cargarPerfil()
        {
            //int idComprador = 3;
            int idComprador = int.Parse(Session["idUsuario"].ToString());
            detalleComprador detalleCompradorDTO = compradorWS.buscarDetalleCompradorPorId(idComprador);
            lblNombres.Text = detalleCompradorDTO.nombres;
            lblPrimerApellido.Text = detalleCompradorDTO.primerApellido;
            lblSegundoApellido.Text = detalleCompradorDTO.segundoApellido;
            lblTipoDocumento.Text = detalleCompradorDTO.tipoDocumento;
            lblNumeroDocumento.Text = detalleCompradorDTO.numeroDocumento;
            lblMontoBilletera.Text = "S/. " + detalleCompradorDTO.montoBilletera.ToString();
            lblDepartamento.Text = detalleCompradorDTO.departamentoFavorito;
            lblProvincia.Text = detalleCompradorDTO.provinciaFavorita;
            txtDistrito.Text = detalleCompradorDTO.distritoFavorito;
            lblCorreo.Text = detalleCompradorDTO.correo;
            lblContrasenia.Text = new string('*', detalleCompradorDTO.contrasenia.Length);
        }
        protected void btnGuardarDistrito_Click(object sender, EventArgs e)
        {
            string nuevoDistrito = txtDistrito.Text;
            if (!string.IsNullOrEmpty(nuevoDistrito))
            {
                //int idComprador = 3;
                int idComprador = int.Parse(Session["idUsuario"].ToString());
                bool resultado = compradorWS.actualizarDistritoFavoritoPorIdComprador(nuevoDistrito, idComprador);
                cargarPerfil();
                if (resultado)
                {
                    nuevoDistrito = txtDistrito.Text.Trim();
                    nuevoDistrito = nuevoDistrito.Replace("'", "\\'");
                    TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                    string distritoFormateado = ti.ToTitleCase(nuevoDistrito.ToLower());
                    string titulo = "¡Distrito favorito actualizado!";
                    string mensaje = $"Tu nuevo distrito favorito es {distritoFormateado}.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModal", $@"setTimeout(function() {{
                    mostrarModalExito('{titulo}', '{mensaje}');}}, 1000);", true);
                }
                else
                {
                    string titulo = "¡Error al actualizar distrito favorito!";
                    string mensaje = $"El distrito {nuevoDistrito} que ingresaste no es válido. Porfavor, ingresa nuevamente un distrito válido.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModal", $@"setTimeout(function() {{
                    mostrarModalExito('{titulo}', '{mensaje}');}}, 1000);", true);
                }
            }
        }

        protected void btnEliminarCuenta_Click(object sender, EventArgs e)
        {
            int idComprador = int.Parse(Session["idUsuario"].ToString());
           
            compradorWS.eliminarUsuarioComprador(idComprador);

            Session.Clear();
            Response.Redirect("~/Presentacion/Inicio/LogIn.aspx");
        }

    }
}