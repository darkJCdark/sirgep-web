using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace SirgepPresentacion.Presentacion.Usuarios.Administrador
{
    public partial class CalendarioReservas : System.Web.UI.Page
    {
        public ReservaWSClient reservaWS = new ReservaWSClient();
        protected void Page_Load(object sender, EventArgs e)
        {
            int mes, anio;
            if (!IsPostBack)
            {
                // Lee mes y año de la URL o usa el actual
                if (!int.TryParse(Request.QueryString["mes"], out mes))
                    mes = DateTime.Now.Month;
                if (!int.TryParse(Request.QueryString["anio"], out anio))
                    anio = DateTime.Now.Year;

                hdnMes.Value = mes.ToString();
                hdnAnio.Value = anio.ToString();

                CargarReservasJson(mes, anio);
            }
        }



        private void CargarReservasJson(int mes, int anio)
        {
            reserva[] reservas = reservaWS.listarPorMesYAnio(mes, anio);

            if (reservas == null || reservas.Length == 0)
            {
                lblSinReservas.Visible = true;
                btnExportarPDF.Enabled = false;
                btnExportarExcel.Enabled = false;
                hdnReservasJson.Value = "[]";
                return;
            }

            lblSinReservas.Visible = false;
            btnExportarPDF.Enabled = true;
            btnExportarExcel.Enabled = true;

            Session["ReservasCalendario"] = reservas;

            var eventos = reservas.Select(r => new
            {
                title = $"{r.espacio?.nombre} - {r.persona?.nombres} {r.persona?.primerApellido} {r.persona?.segundoApellido}",
                start = r.fechaReserva.ToString("yyyy-MM-dd") + "T" + r.iniString.PadLeft(5, '0'),
                end = r.fechaReserva.ToString("yyyy-MM-dd") + "T" + r.finString.PadLeft(5, '0'),
                extendedProps = new
                {
                    numReserva = r.numReserva,
                    espacio = r.espacio?.nombre,
                    fecha = r.fechaReserva.ToString("yyyy-MM-dd"),
                    horaInicio = r.iniString,
                    horaFin = r.finString,
                    nombres = r.persona?.nombres,
                    apellidos = $"{r.persona?.primerApellido} {r.persona?.segundoApellido}",
                    tipoDoc = r.persona?.tipoDocumento.ToString(),
                    numDoc = r.persona?.numDocumento
                }
            }).ToList();

            var jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            hdnReservasJson.Value = jsSerializer.Serialize(eventos);
        }



        protected void btnExportarPDF_Click(object sender, EventArgs e)
        {
            var reservas = Session["ReservasCalendario"] as reserva[];
            if (reservas == null || reservas.Length == 0)
            {
                //Mostrar el modal de error
                string script = "mostrarModalError('Error de carga','No se han cargado reservas, pruebe recargar la página.');";
                ClientScript.RegisterStartupScript(GetType(), "mostrarModalError", script, true);
                return;
            }

            string rutaHtml = Server.MapPath("~/Resources/Pdfs/CalendarioReservasPdf.html");
            string paginaHTML_Texto = File.ReadAllText(rutaHtml, Encoding.UTF8);

            var reservasPorMes = reservas.GroupBy(r => new { r.fechaReserva.Year, r.fechaReserva.Month })
                                         .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

            StringBuilder filas = new StringBuilder();

            foreach (var grupo in reservasPorMes)
            {
                string mesNombre = new DateTime(grupo.Key.Year, grupo.Key.Month, 1)
                    .ToString("MMMM yyyy", new System.Globalization.CultureInfo("es-ES"))
                    .ToUpperInvariant();

                filas.Append($"<tr><td colspan='9' style='background:#f10909;color:#fff;font-weight:bold;font-size:1.1em;text-align:left;padding:8px 4px'>{mesNombre}</td></tr>");
                foreach (var r in grupo)
                {
                    filas.Append("<tr>");
                    filas.Append($"<td>{r.numReserva}</td>");
                    filas.Append($"<td>{r.fechaReserva:yyyy-MM-dd}</td>");
                    filas.Append($"<td>{r.iniString}</td>");
                    filas.Append($"<td>{r.finString}</td>");
                    filas.Append($"<td>{r.espacio?.nombre}</td>");
                    filas.Append($"<td>{r.persona?.nombres}</td>");
                    filas.Append($"<td>{r.persona?.primerApellido + " " + r.persona?.segundoApellido}</td>");
                    filas.Append($"<td>{r.persona?.tipoDocumento}</td>");
                    filas.Append($"<td>{r.persona?.numDocumento}</td>");
                    filas.Append("</tr>");
                }
            }


            paginaHTML_Texto = paginaHTML_Texto.Replace("{FILAS_RESERVAS}", filas.ToString());

            string rutaFisicaLogo = Server.MapPath("~/Images/grl/Escudo_Región_Lima_recortado.PNG");
            paginaHTML_Texto = paginaHTML_Texto.Replace("{RUTA_LOGO}", "file:///" + rutaFisicaLogo.Replace("\\", "/"));

            using (MemoryStream ms = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 30, 30, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();
                using (StringReader stringReader = new StringReader(paginaHTML_Texto))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, stringReader);
                }
                pdfDoc.Close();

                byte[] bytes = ms.ToArray();
                Response.Clear();
                Response.ContentType = "application/pdf";
                string nombreArchivo = $"Reporte_Reservas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename={nombreArchivo}");
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            }
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            var reservas = Session["ReservasCalendario"] as reserva[];
            if (reservas == null || reservas.Length == 0)
            {
                //Mostrar el modal de error
                string script = "mostrarModalError('Error de carga','No se han cargado reservas, pruebe recargar la página.');";
                ClientScript.RegisterStartupScript(GetType(), "mostrarModalError", script, true);
                return;
            }

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Reservas");

                int fila = 1;

                // 1. Insertar imagen (logo)
                string rutaLogo = Server.MapPath("~/Images/grl/Escudo_Región_Lima_recortado.PNG");
                if (File.Exists(rutaLogo))
                {
                    var imagen = ws.AddPicture(rutaLogo)
                                   .MoveTo(ws.Cell(fila, 1))
                                   .WithSize(80, 80); // ancho, alto
                }

                // 2. Insertar título
                ws.Cell(fila, 2).Value = "GOBIERNO REGIONAL DE LIMA | SIRGEP";
                ws.Range(fila, 2, fila, 9).Merge();
                ws.Range(fila, 2, fila, 9).Style.Font.Bold = true;
                ws.Range(fila, 2, fila, 9).Style.Font.FontSize = 16;
                ws.Range(fila, 2, fila, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                fila += 2;

                ws.Cell(fila, 1).Value = "CALENDARIO DE RESERVAS";
                ws.Range(fila, 1, fila, 9).Merge();
                ws.Range(fila, 1, fila, 9).Style.Font.Bold = true;
                ws.Range(fila, 1, fila, 9).Style.Font.FontSize = 14;
                ws.Range(fila, 1, fila, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                fila += 2;

                var reservasPorMes = reservas.GroupBy(r => new { r.fechaReserva.Year, r.fechaReserva.Month })
                                             .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

                foreach (var grupo in reservasPorMes)
                {
                    string mesNombre = new DateTime(grupo.Key.Year, grupo.Key.Month, 1)
                        .ToString("MMMM yyyy", new System.Globalization.CultureInfo("es-ES"))
                        .ToUpperInvariant();

                    ws.Cell(fila, 1).Value = mesNombre;
                    ws.Range(fila, 1, fila, 9).Merge().Style.Fill.BackgroundColor = XLColor.Red;
                    ws.Range(fila, 1, fila, 9).Style.Font.FontColor = XLColor.White;
                    ws.Range(fila, 1, fila, 9).Style.Font.Bold = true;
                    fila++;

                    ws.Cell(fila, 1).Value = "N° Reserva";
                    ws.Cell(fila, 2).Value = "Fecha";
                    ws.Cell(fila, 3).Value = "Hora Inicio";
                    ws.Cell(fila, 4).Value = "Hora Fin";
                    ws.Cell(fila, 5).Value = "Espacio";
                    ws.Cell(fila, 6).Value = "Nombres";
                    ws.Cell(fila, 7).Value = "Apellidos";
                    ws.Cell(fila, 8).Value = "Tipo Doc";
                    ws.Cell(fila, 9).Value = "N° Documento";
                    ws.Range(fila, 1, fila, 9).Style.Font.Bold = true;
                    ws.Range(fila, 1, fila, 9).Style.Fill.BackgroundColor = XLColor.LightGray;
                    fila++;

                    foreach (var r in grupo)
                    {
                        ws.Cell(fila, 1).Value = r.numReserva;
                        ws.Cell(fila, 2).Value = r.fechaReserva.ToString("yyyy-MM-dd");
                        ws.Cell(fila, 3).Value = r.iniString;
                        ws.Cell(fila, 4).Value = r.finString;
                        ws.Cell(fila, 5).Value = r.espacio?.nombre;
                        ws.Cell(fila, 6).Value = r.persona?.nombres;
                        ws.Cell(fila, 7).Value = r.persona?.primerApellido + " " + r.persona?.segundoApellido;
                        ws.Cell(fila, 8).Value = r.persona?.tipoDocumento.ToString();
                        ws.Cell(fila, 9).Value = r.persona?.numDocumento;
                        fila++;
                    }

                    fila++;
                }

                ws.Columns().AdjustToContents();

                using (MemoryStream ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    byte[] bytes = ms.ToArray();
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    string nombreArchivo = $"Reporte_Reservas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    Response.AddHeader("Content-Disposition", $"attachment; filename={nombreArchivo}");
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }
            }
        }
    }
}