using System;
using System.Web.UI;

namespace SirgepPresentacion.Presentacion.Inicio
{
    public partial class PrincipalAdministrador : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnManejarEspacios_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Presentacion/Infraestructura/Espacio/ListaEspacios.aspx");
        }

        protected void btnConsultarReservas_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Presentacion/Ventas/Reserva/ListaReservasAdministrador.aspx");
            //string script = "setTimeout(function(){ mostrarModalError('En proceso...','Esta opción aún está en desarrollo :)'); }, 300);";
            //ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModalError", script, true);
            //Response.Redirect("/Presentacion/Ventas/Reserva/ListaReservas.aspx"); 
        }

        protected void btnManejarEventos_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Presentacion/Infraestructura/Evento/GestionaEventos.aspx");
        }

        protected void btnStats_click(object sender, EventArgs e)
        {
            Response.Redirect("/Presentacion/Usuarios/Administrador/Stats.aspx");
        }

        protected void btnConsultarEntradas_Click(object sender, EventArgs e)
        {
            //string script = "setTimeout(function(){ mostrarModalError('En proceso...','Esta opción aún está en desarrollo :)'); }, 300);";
            //ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModalError", script, true);
            Response.Redirect("/Presentacion/Ventas/Entrada/ListaEntradasAdministrador.aspx"); 
        }

        protected void btnAdministrarCompradores_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Presentacion/Usuarios/Comprador/ListaComprador.aspx");
        }


    }
}
