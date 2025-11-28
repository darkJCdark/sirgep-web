using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SirgepPresentacion.Presentacion.Inicio
{
    public partial class LogIn : System.Web.UI.Page
    {
        PersonaWSClient personaWS = new PersonaWSClient();
        public LogIn()
        {
            personaWS = new PersonaWSClient();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // Impedir que se guarde en caché (clave para botón de retroceso)
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(-1));
            Response.Cache.SetNoStore();

            if (!IsPostBack)
            {
                // Cierra la sesión y la cookie de autenticación al entrar al login
                FormsAuthentication.SignOut();
                Session.Clear();
                Session.Abandon();
                // Opcional: establece tipoUsuario como invitado para el flujo de invitado
                Session["tipoUsuario"] = "invitado";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string script;
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
                ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);
                script = "setTimeout(function(){ mostrarModalError('Campos faltantes.','Por favor, complete todos los campos obligatorios.'); }, 300);";
                ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalError", script, true);
                return;
            }
            string correo = txtEmail.Text.Trim();
            string contrasena = txtPassword.Text.Trim();
            int resultado = personaWS.validarCuenta(correo, contrasena);
            int id = resultado / 10;
            int tipo = resultado % 10;
            script = "setTimeout(function(){ cerrarModalCarga(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "cerrarModalCarga", script, true);

            switch (tipo)
            {
                case 0:
                    script = "setTimeout(function(){ mostrarModalError('Credenciales fallidas.','Las credenciales no son correctas. Intente nuevamente.'); }, 300);";
                    ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalError", script, true);
                    break;
                case 1: //Administrador
                case 2: //Comprador
                    // Autenticación por formularios
                    FormsAuthenticationTicket tkt = new FormsAuthenticationTicket(
                        1, correo, DateTime.Now, DateTime.Now.AddMinutes(30), false, tipo == 1 ? "admin" : "comprador"
                    );
                    string cookiestr = FormsAuthentication.Encrypt(tkt);
                    HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
                    ck.Expires = tkt.Expiration;
                    ck.Path = FormsAuthentication.FormsCookiePath;
                    Response.Cookies.Add(ck);

                    // Guarda en sesión si lo necesitas
                    Session["tipoUsuario"] = tipo == 1 ? "administrador" : "comprador";
                    Session["idUsuario"] = id;
                    Session["nombreUsuario"] = personaWS.obtenerNombreUsuario(id);

                    // Redirige a la página correspondiente
                    string redirectUrl = tipo == 1
                        ? "/Presentacion/Inicio/PrincipalAdministrador.aspx"
                        : "/Presentacion/Inicio/PrincipalInvitado.aspx";
                    Response.Redirect(redirectUrl, true);
                    break;
                case -1:
                    script = "setTimeout(function(){ mostrarModalError('Credenciales inexistentes.','Las credenciales no pertenecen a ninguna cuenta. Intente nuevamente.'); }, 300);";
                    ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalError", script, true);
                    break;
                default:
                    script = "setTimeout(function(){ mostrarModalError('Error Desconocido.','Intente nuevamente.'); }, 300);";
                    ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalError", script, true);
                    break;
            }
        }

        private void setNombreMenu(int id, string controlId)
        {
            string nombre = personaWS.obtenerNombreUsuario(id);
            HtmlAnchor menuAnchor = (HtmlAnchor)BuscarControlRecursive(Master, controlId);


            if (menuAnchor != null)
            {
                menuAnchor.InnerText = string.IsNullOrEmpty(nombre) ? "Usuario" : nombre;
            }
        }
        private Control BuscarControlRecursive(Control raiz, string id)
        {
            if (raiz.ID == id)
                return raiz;

            foreach (Control ctrl in raiz.Controls)
            {
                Control encontrado = BuscarControlRecursive(ctrl, id);
                if (encontrado != null)
                    return encontrado;
            }
            return null;
        }

    }
}