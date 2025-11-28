using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using SirgepPresentacion.ReferenciaDisco;

namespace SirgepPresentacion.Presentacion.Ventas.Reserva
{
    public partial class FormularioEspacio : System.Web.UI.Page
    {
        private EspacioWSClient espacioWS;
        protected DepartamentoWSClient departamentoWS;
        protected ProvinciaWSClient provinciaWS;
        protected DistritoWSClient distritoWS;
        protected HorarioEspacioWSClient horarioWS;
        public class Horario
        {
            public DateTime horaInicio { get; set; }
            public bool disponible { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            departamentoWS = new DepartamentoWSClient();
            provinciaWS = new ProvinciaWSClient();
            distritoWS = new DistritoWSClient();
            espacioWS = new EspacioWSClient();
            horarioWS = new HorarioEspacioWSClient();

            if (!IsPostBack)
            {
                CargarDepartamentos();

                txtFecha.Attributes["min"] = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");

                ddlDistrito.Items.Insert(0, new ListItem("Seleccione Distrito", ""));
                ddlProvincia.Items.Insert(0, new ListItem("Seleccione Provincia", ""));
                ddlEspacio.Items.Insert(0, new ListItem("Seleccione Espacio", ""));

                /* Hardcodeado pq' no lo tenemos en nuestra BD, además hay pocos datos */
                ddlCategoria.Items.Insert(0, new ListItem("Seleccione un tipo", ""));
                ddlCategoria.Items.Insert(1, new ListItem("Teatros", "TEATRO"));
                ddlCategoria.Items.Insert(2, new ListItem("Canchas", "CANCHA"));
                ddlCategoria.Items.Insert(3, new ListItem("Salones", "SALON"));
                ddlCategoria.Items.Insert(4, new ListItem("Parques", "PARQUE"));
            }
        }

        protected void CargarDepartamentos()
        {
            ddlDepartamento.DataSource = departamentoWS.listarDepas();
            ddlDepartamento.DataTextField = "Nombre";
            ddlDepartamento.DataValueField = "IdDepartamento";
            ddlDepartamento.DataBind();
            ddlDepartamento.Items.Insert(0, new ListItem("Seleccione Departamento", ""));
        }

        protected void ddlDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idDepto;
            provinciaWS = new ProvinciaWSClient();
            lblError.Visible = false;
            if (int.TryParse(ddlDepartamento.SelectedValue, out idDepto))
            {
                ddlProvincia.DataSource = provinciaWS.listarProvinciaPorDepa(idDepto);

                ddlProvincia.DataTextField = "Nombre";
                ddlProvincia.DataValueField = "IdProvincia";
                ddlProvincia.DataBind();
                ddlProvincia.Enabled = true;
                ddlProvincia.Items.Insert(0, new ListItem("Seleccione Provincia", ""));
            }
            else
            {
                // Manejar el caso en que la conversión falle
                ddlProvincia.Enabled = false;
            }
        }

        protected void ddlProvincia_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idProv;
            if (int.TryParse(ddlProvincia.SelectedValue, out idProv))
            {
                distritoWS = new DistritoWSClient();

                ddlDistrito.DataSource = distritoWS.listarDistritosFiltrados(idProv);
                ddlDistrito.DataTextField = "Nombre";
                ddlDistrito.DataValueField = "IdDistrito";
                ddlDistrito.DataBind();
                ddlDistrito.Enabled = true;
                ddlDistrito.Items.Insert(0, new ListItem("Seleccione Distrito", ""));
            }
            else
            {
                // Manejar el caso en que la conversión falle
                ddlDistrito.Enabled = false;
            }
        }

        protected void ddlDistrito_SelectedIndexChanged(object sender, EventArgs e)
        {
            // no sé si se deben cargar distritos sin categoría?
            int idDistrito = int.Parse(ddlDistrito.SelectedValue);
            ddlEspacio.DataSource = espacioWS.listarEspacioPorDistrito(idDistrito);
            ddlEspacio.DataTextField = "Nombre";
            ddlEspacio.DataValueField = "idEspacio";
            ddlEspacio.DataBind();
            lblError.Visible = false;
            ddlEspacio.Enabled = true;
            ddlEspacio.Items.Insert(0, new ListItem("Seleccione Espacio", ""));
        }

        protected void ddlEspacio_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblError.Visible = true;
            if (ddlEspacio.SelectedValue.Equals(""))
                return;
            else
            {
                int idEspacio = int.Parse(ddlEspacio.SelectedValue.ToString());
                espacio es = espacioWS.buscarEspacio(idEspacio);
                lblError.Visible = false;
                lblPrecioHora.Text = es.precioReserva.ToString();
                lblPrecioHora.Text = $"Precio por hora: S/ {es.precioReserva.ToString("F2", CultureInfo.InvariantCulture)}";
                lblPrecioTotal.Text = "Precio total seleccionado: Seleccione un horario";
                hdnPrecioHora.Value = es.precioReserva.ToString(CultureInfo.InvariantCulture);
            }
                
        }

        protected void ddlCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  int idCat = int.Parse(ddlCategoria.SelectedValue);
            // int idDist = int.Parse(ddlDistrito.SelectedValue);

            string filtroCat = ddlCategoria.Text;
            string filtroDist = ddlDistrito.Text;
            if (filtroCat != "" && filtroDist != "")
            {
                // llamamos a la filtración doble implementada en backend
                ddlEspacio.Items.Clear();
                ddlEspacio.DataSource = espacioWS.listarEspacioDistyCat(int.Parse(filtroDist), filtroCat);

                ddlEspacio.DataTextField = "nombre";
                ddlEspacio.DataValueField = "idEspacio";
                ddlEspacio.DataBind();
                if (ddlEspacio.DataSource == null)
                {
                    ddlEspacio.Items.Insert(0, new ListItem("Seleccione otra categoría.", ""));
                    lblError.Visible = true;
                    lblError.Text = "No hay espacios para el distrito y categoría seleccionados.";
                    return;
                }
                lblError.Visible = false;
                ddlEspacio.Items.Insert(0, new ListItem("Seleccione Espacio", ""));
            }
            else
            {
                // Si no hay filtro, no mostramos nada
                ddlEspacio.Items.Clear();
                ddlEspacio.Items.Insert(0, new ListItem("Seleccione Espacio", ""));
            }

        }

        protected void btnConsultarHorarios_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlEspacio.SelectedValue) && !string.IsNullOrEmpty(txtFecha.Text))
            {
                int idEspacio = int.Parse(ddlEspacio.SelectedValue);
                string fecha = txtFecha.Text;

                
                var horarios = horarioWS.listarHorariosDelEspacioYDia(idEspacio, fecha);

                if (horarios != null)
                {
                    lblError.Visible = false;
                    rptHorarios.DataSource = horarios;
                    rptHorarios.DataBind();
                }
                else
                {
                    rptHorarios.DataSource = null;
                    lblError.Visible = true;
                    lblError.Text = "El espacio no atiende el día que usted ha elegido.";
                }

            }
            else
            {
                lblError.Visible = true;
                lblError.Text = "Por favor, seleccione un Espacio y una Fecha.";
                string script = "setTimeout(function(){ mostrarModalError('Datos Faltantes','Por favor, seleccione un Espacio y una Fecha.'); }, 300);";
                ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalError", script, true);
            }
        }
        protected void btnReservar_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            if (!(ddlEspacio.SelectedIndex > 0))
            {
                lblError.Visible = true;
                lblError.Text = "Por favor, seleccione un Espacio.";
                string script = "setTimeout(function(){ mostrarModalError('Espacio Faltante','Por favor, seleccione un Espacio.'); }, 300);";
                ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalError", script, true);
            }
            else if (string.IsNullOrEmpty(txtFecha.Text))
            {
                lblError.Visible = true;
                lblError.Text = "Por favor, seleccione una Fecha.";
                string script = "setTimeout(function(){ mostrarModalError('Fecha Faltante','Por favor, seleccione una Fecha.'); }, 300);";
                ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalError", script, true);
            }
            else
            {
                string horaIni = hdnHoraInicioSeleccionada.Value;
                string horaFin = hdnHoraFinSeleccionada.Value;
                if (horaIni.Length==0 && horaFin.Length == 0)
                {
                    lblError.Visible = true;
                    lblError.Text = "Por favor, seleccione al menos un horario disponible.";
                    string script = "setTimeout(function(){ mostrarModalError('Horario Faltante','Por favor, seleccione al menos un horario disponible.'); }, 300);";
                    ClientScript.RegisterStartupScript(this.GetType(), "mostrarModalError", script, true);
                }
                else
                {
                    double cantidadHoras = (DateTime.Parse(horaFin) - DateTime.Parse(horaIni)).TotalHours +1;
                    int idEspacio = int.Parse(ddlEspacio.SelectedValue);
                    string fecha = txtFecha.Text;

                    string url = $"/Presentacion/Ventas/Reserva/CompraReserva.aspx?idEspacio={idEspacio}&fecha={fecha}&horaIni={horaIni}&horaFin={horaFin}&cant={cantidadHoras}";

                    Response.Redirect(url);
                }
            }
        }

    }
}