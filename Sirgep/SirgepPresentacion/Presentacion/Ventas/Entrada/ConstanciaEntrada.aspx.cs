using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using SirgepPresentacion.Presentacion.Inicio;
using SirgepPresentacion.ReferenciaDisco;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace SirgepPresentacion.Presentacion.Ventas.Entrada
{
    public partial class ConstanciaEntrada : System.Web.UI.Page
    {
        private EntradaWSClient entradaWS;
        //private CompradorWSClient compradorWS;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                entradaWS = new EntradaWSClient();
                //int idConstancia = 2;
                int idConstancia = int.Parse(Request.QueryString["idConstancia"]);
                CargarDatosEnPantalla(idConstancia);
            }
        }
        protected void CargarDatosEnPantalla(int idConstancia)
        {
            constanciaEntradaDTO constanciaEntradaDTO = entradaWS.buscarConstanciaEntrada(idConstancia);
            // Datos de la entrada
            lblNumEntrada.Text = constanciaEntradaDTO.detalleEntrada.numEntrada.ToString();
            // Datos del evento
            lblEvento.Text = constanciaEntradaDTO.detalleEntrada.nombreEvento;
            lblUbicacion.Text = constanciaEntradaDTO.detalleEntrada.ubicacion;
            //lblReferencias.Text = constanciaEntradaDTO.detalleEntrada.referencia;
            lblDistrito.Text = constanciaEntradaDTO.detalleEntrada.nombreDistrito;
            // Datos de la funcion
            lblFechaFuncion.Text = constanciaEntradaDTO.detalleEntrada.fechaFuncion.ToString("dd/MM/yyyy");
            lblHoraInicio.Text = constanciaEntradaDTO.detalleEntrada.horaInicio.ToString("HH:mm");
            lblHoraFin.Text = constanciaEntradaDTO.detalleEntrada.horaFin.ToString("HH:mm");

            //Datos del comprador
            lblNombres.Text = constanciaEntradaDTO.nombresComprador;
            lblApellidos.Text = constanciaEntradaDTO.apellidosComprador;
            lblTipoDocumento.Text = constanciaEntradaDTO.tipoDocumento.ToString();
            lblTNumDocumento.Text = constanciaEntradaDTO.numDocumento.ToString();
            lblCorreo.Text = constanciaEntradaDTO.correo;
            // Datos de la constancia del pago
            lblFechaConstancia.Text = constanciaEntradaDTO.fecha.ToString("dd/MM/yyyy");
            lblMetodoPago.Text = constanciaEntradaDTO.metodoPago.ToString();
            lblDetallePago.Text = constanciaEntradaDTO.detallePago.ToString();
            //lblPrecio.Text = eventoDomain.precioEntrada.ToString("C2");
            lblTotal.Text = "S/. " + constanciaEntradaDTO.monto.ToString();
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            string tipoUsuario = Session["tipoUsuario"] as string;
            // si es Administrador, no se pide calificación y, a su vez, la riderección es al PrincipalAdministrador…
            if(tipoUsuario == "administrador")
            {
                Response.Redirect("~/Presentacion/Ventas/Entrada/ListaEntradasAdministrador.aspx");
            }
            Session["tipoServicio"] = "entrada";
            string script = "setTimeout(function(){ mostrarModalFeedback(); }, 300);";
            ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalFeedback", script, true);
            // Flujo de compra (muestra modal de feedback)
            if (Session["MostrarFeedback"] != null && (bool)Session["MostrarFeedback"])
            {
                Session["tipoServicio"] = "entrada";
                Session["MostrarFeedback"] = null; // Limpiar para evitar mostrarlo de nuevo
                script = "setTimeout(function(){ mostrarModalFeedback(); }, 300);";
                ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalFeedback", script, true);
            }
            else // Flujo de consulta (redirecciona a la página de consulta de entradas)
            {
                Response.Redirect("/Presentacion/Ventas/Entrada/ListaEntradasComprador.aspx");
            }
        }

        protected void btnDescargar_Click(object sender, EventArgs e)
        {
            string rutaHtml = Server.MapPath("~/Resources/Pdfs/EntradaPdf.html");
            string paginaHTML_Texto = File.ReadAllText(rutaHtml, Encoding.UTF8);
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
                string nombreArchivo = $"Constancia_Entrada_{lblNumEntrada.Text}_{nombresLimpios}_{fechaFormato}.pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename={nombreArchivo}");
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                Response.Flush();
                Response.End();
            }
        }
        protected void CargarDatosEnPdf(ref string paginaHTML_Texto)
        {
            paginaHTML_Texto = paginaHTML_Texto.Replace("{NUMERO_ENTRADA}", lblNumEntrada.Text);
            // Datos de la entrada
            paginaHTML_Texto = paginaHTML_Texto.Replace("{NOMBRE_EVENTO}", lblEvento.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{UBICACION}", lblUbicacion.Text);
            paginaHTML_Texto = paginaHTML_Texto.Replace("{DISTRITO}", lblDistrito.Text);
            //paginaHTML_Texto = paginaHTML_Texto.Replace("{REFERENCIAS}", lblReferencias.Text);
            // Datos del evento
            paginaHTML_Texto = paginaHTML_Texto.Replace("{FECHA_FUNCION}", lblFechaFuncion.Text);
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