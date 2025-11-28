using System;
using System.Web.UI;

namespace SirgepPresentacion.Presentacion.Inicio
{
    public partial class PrincipalInvitado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnReservar_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Presentacion/Infraestructura/Espacio/FormularioEspacio.aspx");
        }

        protected void btnVerEventos_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Presentacion/Ubicacion/Distrito/EligeDistrito.aspx");
        }
    }
}