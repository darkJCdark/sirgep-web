using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using SirgepPresentacion.ReferenciaDisco;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace SirgepPresentacion.Presentacion.Ventas.Reserva
{
    public partial class ConstanciaReserva : System.Web.UI.Page
    {
        private ReservaWSClient reservaWS;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                reservaWS = new ReservaWSClient();
                //int idConstancia = 1;
                int idConstancia = int.Parse(Request.QueryString["idConstancia"]);
                CargarDatosEnPantalla(idConstancia);
            }
        }
        protected void CargarDatosEnPantalla(int idConstancia)
        {
            constanciaReservaDTO constanciaReservaDTO = reservaWS.buscarConstanciaReserva(idConstancia);
            // Datos del espacio
            lblEspacio.Text = constanciaReservaDTO.detalleReserva.nombreEspacio;
            lblTipoEspacio.Text = constanciaReservaDTO.detalleReserva.categoria;
            lblSuperficie.Text = constanciaReservaDTO.detalleReserva.superficie.ToString() + " m²";
            lblUbicacion.Text = constanciaReservaDTO.detalleReserva.ubicacion;
            lblDistrito.Text = constanciaReservaDTO.detalleReserva.nombreDistrito;
            // Datos de la Reserva
            lblNumReserva.Text = constanciaReservaDTO.detalleReserva.numReserva.ToString();
            lblFechaReserva.Text = constanciaReservaDTO.detalleReserva.fecha.ToString("dd/MM/yyyy");
            lblHoraInicio.Text = constanciaReservaDTO.detalleReserva.horaInicio.ToString("HH:mm");
            lblHoraFin.Text = constanciaReservaDTO.detalleReserva.horaFin.ToString("HH:mm");
            //Datos del comprador
            lblNombres.Text = constanciaReservaDTO.nombresComprador;
            lblApellidos.Text = constanciaReservaDTO.apellidosComprador;
            lblTipoDocumento.Text = constanciaReservaDTO.tipoDocumento;
            lblTNumDocumento.Text = constanciaReservaDTO.numDocumento;
            lblCorreo.Text = constanciaReservaDTO.correo;
            // Datos de la constancia del pago
            lblFechaConstancia.Text = constanciaReservaDTO.fecha.ToString("dd/MM/yyyy");
            lblMetodoPago.Text = constanciaReservaDTO.metodoPago;
            lblDetallePago.Text = constanciaReservaDTO.detallePago;
            lblTotal.Text = "S/. " + constanciaReservaDTO.monto.ToString();
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            string tipoUsuario = Session["tipoUsuario"] as string;
            // si es Administrador, no se pide calificación y, a su vez, la riderección es al PrincipalAdministrador…
            if (tipoUsuario == "administrador")
            {
                Response.Redirect("~/Presentacion/Ventas/Reserva/ListaReservasAdministrador.aspx");
            }
            // Flujo de compra (muestra modal de feedback)
            if (Session["MostrarFeedback"] != null && (bool)Session["MostrarFeedback"])
            {
                Session["tipoServicio"] = "reserva";
                Session["MostrarFeedback"] = null; // Limpiar para evitar mostrarlo de nuevo
                string script = "setTimeout(function(){ mostrarModalFeedback(); }, 300);";
                ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalFeedback", script, true);
            }
            else // Flujo de consulta (redirecciona a la página de consulta de entradas)
            {
                Response.Redirect("/Presentacion/Ventas/Reserva/ListaReservasComprador.aspx");
            }
        }

        protected void btnDescargar_Click(object sender, EventArgs e)
        {
            string rutaHtml = Server.MapPath("~/Resources/Pdfs/ReservaPdf.html");
            string paginaHTML_Texto = File.ReadAllText(rutaHtml);
            CargarDatosEnPdf(ref paginaHTML_Texto);
            using (MemoryStream ms = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();
                pdfDoc.Add(new Phrase(""));
                // Ruta física y reemplazo del logo
                string rutaFisicaLogo = Server.MapPath("~/Images/grl/Escudo_Región_Lima_recortado.PNG");
                string rutaLogoFormatoFile = "file:///" + rutaFisicaLogo.Replace("\\", "/");
                paginaHTML_Texto = paginaHTML_Texto.Replace("{RUTA_LOGO}", rutaLogoFormatoFile);
                using (StringReader stringReader = new StringReader(paginaHTML_Texto))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, stringReader);
                }
                pdfDoc.Close();
                // Envía el PDF al navegador
                byte[] bytes = ms.ToArray();
                Response.Clear();
                Response.ContentType = "application/pdf";
                string fechaFormato = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string nombresLimpios = Regex.Replace(lblNombres.Text, @"[^A-Za-z0-9]", "_");
                string nombreArchivo = $"Constancia_Reserva_{lblNumReserva.Text}_{nombresLimpios}_{fechaFormato}.pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename={nombreArchivo}");
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                Response.Flush();
                Response.End();
            }
        }
        protected void CargarDatosEnPdf(ref string paginaHTML_Texto)
        {
            paginaHTML_Texto = paginaHTML_Texto.Replace("{NUMERO_RESERVA}", lblNumReserva.Text);
            // Datos del Espacio
            paginaHTML_Texto = paginaHTML_Texto.Replace("{NOMBRE_ESPACIO}", lblEspacio.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{TIPO_ESPACIO}", lblTipoEspacio.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{SUPERFICIE}", lblSuperficie.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{UBICACION}", lblUbicacion.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{DISTRITO}", lblDistrito.Text);
            // Datos de la Reserva
            paginaHTML_Texto = paginaHTML_Texto.Replace("{FECHA_RESERVA}", lblFechaReserva.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{HORA_INICIO}", lblHoraInicio.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{HORA_FIN}", lblHoraFin.Text);
            // Datos del comprador
            paginaHTML_Texto = paginaHTML_Texto.Replace("{NOMBRES}", lblNombres.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{APELLIDOS}", lblApellidos.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{TIPO_DOCUMENTO}", lblTipoDocumento.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{NUM_DOCUMENTO}", lblTNumDocumento.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{CORREO}", lblCorreo.Text);
            // Datos de la constancia de pago
            paginaHTML_Texto = paginaHTML_Texto.Replace("{FECHA_CONSTANCIA}", lblFechaConstancia.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{METODO_PAGO}", lblMetodoPago.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{TOTAL}", lblTotal.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{DETALLE_PAGO}", lblDetallePago.Text);
            //paginaHTML_Texto = paginaHTML_Texto.Replace("@FECHA", DateTime.Now.ToString("dd/MM/yyyy"));
        }
    }
}