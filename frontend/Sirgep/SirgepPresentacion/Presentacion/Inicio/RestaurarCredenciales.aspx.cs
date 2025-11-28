using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SirgepPresentacion.Presentacion.Inicio
{
    public partial class RestaurarCredenciales : System.Web.UI.Page
    {
        PersonaWSClient personaWS = new PersonaWSClient();
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }



        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            string script;
            if (!Page.IsValid)
            {
                // Cerrar modal de carga
                script = "setTimeout(function() { (cerrarModalCarga()); }, 300);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "cerrarModalCarga", script, true);
                return;
            }

            // Bloquear el botón para evitar múltiples envíos
            btnEnviar.Enabled = false;
            btnEnviar.Text = "Enviando...";
            // Bloquear el campo de correo para evitar cambios
            txtCorreo.Enabled = false;


            // Enviar correo electrónico para restaurar credenciales
            string correo = txtCorreo.Text.Trim();

            // Envio de correo
            bool resultado = enviarCorreoRestauracion(correo);
            // Cerrar modal de carga
            script = "setTimeout(function() { (cerrarModalCarga()); }, 300);";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "cerrarModalCarga", script, true);

            if (resultado)
            {
                // Mostrar modal de éxito
                script = "setTimeout(function() { (mostrarModalExito('Envio exitoso.','Solicitud enviada, pronto se contactarán con usted.')); }, 300);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalExito", script, true);
                // Limpiar campos
                btnEnviar.Text = "Enviado";
            }
            else
            {
                // Mostrar modal de error
                script = "setTimeout(function() { (mostrarModalError('Error al enviar el correo.','Por favor, inténtelo de nuevo más tarde.')); }, 300);";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "mostrarModalError", script, true);
                // Rehabilitar el botón y el campo de correo para un nuevo intento
                btnEnviar.Enabled = true;
                btnEnviar.Text = "Enviar";
                txtCorreo.Enabled = true;
            }
        }
        private bool enviarCorreoRestauracion(string correoSolicitante)
        {
            string asunto = $"Solicitud de recuperación de contraseña - {correoSolicitante}";

            string contenido = $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: 'Segoe UI', sans-serif;
                    background-color: #f9f9f9;
                    padding: 20px;
                }}
                .card {{
                    background-color: #ffffff;
                    border-radius: 8px;
                    padding: 20px;
                    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                }}
                .card h2 {{
                    color: #dc3545;
                }}
                .card p {{
                    font-size: 16px;
                    color: #212529;
                }}
                .footer {{
                    font-size: 12px;
                    color: #6c757d;
                    margin-top: 20px;
                    text-align: center;
                }}
            </style>
        </head>
        <body>
            <div class='card'>
                <h2>Solicitud de recuperación</h2>
                <p>Se ha solicitado el restablecimiento de contraseña por parte del siguiente correo:</p>
                <p><strong>{correoSolicitante}</strong></p>
                <p>Favor de tomar contacto con el solicitante a la brevedad.</p>
                <div class='footer'>
                    Este mensaje fue generado automáticamente por el sistema SIRGEP.<br/>
                    © 2025 Gobierno Regional de Lima
                </div>
            </div>
        </body>
        </html>";

            // Llamada al Web Service de envío de correo
            bool resultado = personaWS.enviarCorreoRecuperacion(asunto, contenido);
            return resultado;
        }
    }
}