using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Bibliography;
using SirgepPresentacion.ReferenciaDisco;

namespace SirgepPresentacion.Presentacion.Usuarios.Administrador
{
    public partial class Stats : System.Web.UI.Page
    {
        public string DataLineChartJson;
        public string DataPieChartJson;
        public string DataPieChart2Json;
        private ReporteWSClient service;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                service = new ReporteWSClient();

                // Asumiendo que estos métodos ya están listosa
                int[] reservasPorMesArray = service.reservasPorMes();
                int[] entradasPorMesArray = service.entradasPorMes();

                List<int> reservasPorMes = new List<int>(reservasPorMesArray);

                List<int> entradasPorMes = new List<int>(entradasPorMesArray);

                // pasando un array de objetos con clave `mes` y `cantidad`
                espacioRepDTO[] espaciosFavoritosArray = service.espaciosFavoritosDelMes();
                List<espacioRepDTO> espaciosFavoritos = new List<espacioRepDTO>(espaciosFavoritosArray);

                espacioRepDTO[] eventosFavoritosArray = service.eventosFavoritosDelMes();
                List<espacioRepDTO> eventosFavoritos = new List<espacioRepDTO>(eventosFavoritosArray);
                // Convertir al formato que el JS espera
                JavaScriptSerializer js = new JavaScriptSerializer();

                var lineChartData = new
                {
                    reservas = reservasPorMes,
                    entradas = entradasPorMes
                };

                DataLineChartJson = js.Serialize(lineChartData);

                // Convertimos a un array de objetos con clave `nombre` y `cantidad`
                var pieData = new List<object>();
                foreach (var esp in espaciosFavoritos)
                {
                    pieData.Add(new { nombre = esp.nombre, cantidad = esp.cantReservas });
                }
                DataPieChartJson = js.Serialize(pieData);

                var eventosPieData = new List<object>();
                foreach (var evt in eventosFavoritos)
                {
                    eventosPieData.Add(new { nombre = evt.nombre, cantidad = evt.cantReservas });
                }
                DataPieChart2Json = js.Serialize(eventosPieData);

            }
        }
    }
}
