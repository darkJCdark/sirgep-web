using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Bibliography;
using SirgepPresentacion.ReferenciaDisco;

namespace SirgepPresentacion.Presentacion.Inicio
{
    public partial class SignIn : System.Web.UI.Page
    {
        protected DepartamentoWSClient departamentoWS;
        protected ProvinciaWSClient provinciaWS;
        protected DistritoWSClient distritoWS;
        protected CompradorWSClient compradorWS;
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                // Llenar departamentos con tu WS
                departamentoWS = new DepartamentoWSClient();
                provinciaWS = new ProvinciaWSClient();
                distritoWS = new DistritoWSClient();

                ddlDepartamento.DataSource = departamentoWS.listarDepas();
                ddlDepartamento.DataTextField = "Nombre";
                ddlDepartamento.DataValueField = "IdDepartamento";
                ddlDepartamento.DataBind();
                ddlDepartamento.Items.Insert(0, new ListItem("Departamento", ""));
                ddlProvincia.Items.Insert(0, new ListItem("Provincia", ""));
                ddlDistrito.Items.Insert(0, new ListItem("Distrito", ""));

            }
        }

        protected void ddlDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idDepto;
            provinciaWS = new ProvinciaWSClient();

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

        protected void cvDocumento_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string tipo = ddlTipoDocumento.SelectedValue;
            string numero = txtNumeroDocumento.Text.Trim();
            string mensaje = "";

            switch (tipo)
            {
                case "DNI":
                    args.IsValid = numero.Length == 8 && Regex.IsMatch(numero, @"^\d{8}$");
                    if (!args.IsValid) mensaje = "El DNI debe tener exactamente 8 dígitos numéricos.";
                    break;

                case "CARNETEXTRANJERIA":
                    args.IsValid = numero.Length == 12 && Regex.IsMatch(numero, @"^\d{12}$");
                    if (!args.IsValid) mensaje = "El Carnet de Extranjería debe tener exactamente 12 dígitos numéricos.";
                    break;

                case "PASAPORTE":
                    args.IsValid = numero.Length >= 8 && numero.Length <= 12 && Regex.IsMatch(numero, @"^[a-zA-Z0-9]+$");
                    if (!args.IsValid) mensaje = "El Pasaporte debe tener entre 8 y 12 caracteres alfanuméricos (sin símbolos).";
                    break;

                default:
                    args.IsValid = false;
                    mensaje = "Seleccione un tipo de documento válido.";
                    break;
            }

            ((CustomValidator)source).ErrorMessage = mensaje;
        }

        protected void cvCorreo_ServerValidate(object source, ServerValidateEventArgs args)
        {
            compradorWS = new CompradorWSClient();
            bool existe = compradorWS.validarCorreo(args.Value.Trim());

            args.IsValid = !existe;

            if (!args.IsValid)
                ((CustomValidator)source).ErrorMessage = "Este correo ya está registrado a una cuenta.";
        }

        protected void cvMonto_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (int.TryParse(txtMonto.Text.Trim(), out int monto))
            {
                args.IsValid = monto >= 50 && monto % 10 == 0;
            }
            else args.IsValid = false;
        }
         //comentario para push
        protected void btnCrearCuenta_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            compradorWS = new CompradorWSClient();
            bool existe = compradorWS.validarCorreo(txtCorreo.Text);

            if (!existe)
            {
                var nPersona = new comprador
                {

                    nombres = txtNombres.Text.Trim(),
                    primerApellido = txtApellidoPaterno.Text.Trim(),
                    segundoApellido = txtApellidoMaterno.Text.Trim(),
                    usuario = txtUsuario.Text.Trim(),
                    //tipoDocumento = ddlTipoDocumento.SelectedValue,
                    numDocumento = txtNumeroDocumento.Text.Trim(),
                    correo = txtCorreo.Text.Trim(),
                    contrasenia = txtContraseña.Text.Trim(),
                    // = int.Parse(ddlDistrito.SelectedValue),
                    monto = double.Parse(txtMonto.Text.Trim()),
                    registrado = 1
                };
                string tDocumento = ddlTipoDocumento.SelectedItem.Text;
                string nombreDistrito = ddlDistrito.SelectedItem.Text;
                var resultado = compradorWS.crearCuenta(nPersona, tDocumento, nombreDistrito);

                if (resultado > 0)
                {
                    Response.Redirect("~/Presentacion/Inicio/Login.aspx");
                }
            }
            
            lblCorreoError.Text= "Este correo ya está registrado a una cuenta.";
            return;

        }

        protected void lnkIniciarSesion_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Presentacion/Inicio/LogIn.aspx");
        }

        protected void txtCorreo_TextChanged(object sender, EventArgs e)
        {
            compradorWS = new CompradorWSClient();
            bool existe = compradorWS.validarCorreo(txtCorreo.Text);
            // args.IsValid = valido;

        }
    }
}