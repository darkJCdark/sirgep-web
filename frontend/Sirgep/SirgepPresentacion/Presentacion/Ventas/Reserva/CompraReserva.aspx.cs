using System;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using SirgepPresentacion.ReferenciaDisco;
using static SirgepPresentacion.Presentacion.Ventas.Reserva.FormularioEspacio;

namespace SirgepPresentacion.Presentacion.Ventas.Reserva
{
    public partial class CompraReserva : System.Web.UI.Page
    {
        EspacioWSClient espacioService;
        CompraWSClient compraService;
        ReservaWSClient reservaService;

        protected void Page_Init(object sender, EventArgs e)
        {
            espacioService = new EspacioWSClient();
            compraService = new CompraWSClient();
            reservaService = new ReservaWSClient();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                // Aseguramos que el session para el feedback esté en false al cargar la página
                Session["MostrarFeedback"] = false;
                int idEspacio = int.Parse(Request.QueryString["idEspacio"]);
                string fechaR = Request.QueryString["fecha"];
                string horaIni = Request.QueryString["horaIni"];
                string horaFin = Request.QueryString["horaFin"];
                int cantidadHoras = int.Parse(Request.QueryString["cant"]);
                espacio espacio = espacioService.buscarEspacio(idEspacio);

                if (!string.IsNullOrEmpty(espacio.foto))
                {
                    imgEvento.ImageUrl = "~/" + espacio.foto;
                    imgEvento.Visible = true;
                }
                else
                {
                    imgEvento.Visible = false;
                }

                LblEspacio.Text = espacio.nombre;
                LblUbicacionReserva.Text = espacio.ubicacion;
                TimeSpan tFin = TimeSpan.Parse(horaFin).Add(TimeSpan.FromHours(1));
                LblHorarioReserva.Text = $"{horaIni} - {tFin.ToString()}";
                LblFechaReserva.Text = DateTime.Parse(fechaR).ToString("dd/MM/yyyy");
                lblPrecioHora.Text = espacio.precioReserva.ToString();
                LblTotalReserva.Text = (espacio.precioReserva * cantidadHoras).ToString("F2");

                //Si el usuario esta logueado, llena los campos y los pone en lectura
                if (Session["idUsuario"] != null)
                {
                    int idPersona = (int)Session["idUsuario"];
                    var comprador = compraService.buscarComprador(idPersona);
                    if (comprador != null)
                    {
                        txtNombres.Text = comprador.nombres;
                        txtNombres.ReadOnly = true;
                        txtApellidoPaterno.Text = comprador.primerApellido;
                        txtApellidoPaterno.ReadOnly = true;
                        txtApellidoMaterno.Text = comprador.segundoApellido;
                        txtApellidoMaterno.ReadOnly = true;
                        txtDNI.Text = comprador.numDocumento;
                        txtDNI.ReadOnly = true;
                        txtCorreo.Text = comprador.correo;
                        txtCorreo.ReadOnly = true;
                        ddlTipoDocumento.SelectedValue = comprador.tipoDocumento.ToString();
                        ddlTipoDocumento.Enabled = false;
                    }
                }


            }
        }


        protected void btnPagar_Click(object sender, EventArgs e)

        {
            if (!Page.IsValid)
            {
                return;
            }

            string fechaR = Request.QueryString["fecha"];
            string horaIni = Request.QueryString["horaIni"];
            string horaFin = Request.QueryString["horaFin"];
            TimeSpan tIni = TimeSpan.Parse(horaIni);
            TimeSpan tFin = TimeSpan.Parse(horaFin).Add(TimeSpan.FromHours(1));



            // Validar método de pago
            if (string.IsNullOrEmpty(hfMetodoPago.Value))
            {
                string script = "setTimeout(function(){ mostrarModalError('Método de pago faltante.','Debe seleccionar un método de pago.'); }, 300);";
                ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModalError", script, true);
                return;
            }

            string dni = txtDNI.Text.Trim();
            var compradorExistente = compraService.buscarCompradorPorDni(dni);

            int idEspacio = int.Parse(Request.QueryString["idEspacio"]);
            double totalAPagar = double.Parse(LblTotalReserva.Text);

            // ---------- Comprobación de saldo ----------
            if (compradorExistente != null && compradorExistente.monto < totalAPagar)
            {
                string script = "setTimeout(function(){ mostrarModalError('Error en pago.','Saldo insuficiente.'); }, 300);";
                ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModalError", script, true);
                return;
            }

            // ---------- Insertar / actualizar comprador ----------

            comprador compradorFinal = null;
            int identificadorPersona;
            if (compradorExistente == null)
            {
                eTipoDocumento tdoc = (eTipoDocumento)Enum.Parse(typeof(eTipoDocumento), ddlTipoDocumento.SelectedValue, true);
                comprador nuevo = new comprador
                {
                    nombres = txtNombres.Text.Trim(),
                    primerApellido = txtApellidoPaterno.Text.Trim(),
                    //segundoApellido = txtApellidoMaterno.Text.Trim(),  //no obligatorio
                    numDocumento = txtDNI.Text.Trim(),
                    correo = txtCorreo.Text.Trim(),
                    tipoDocumento = tdoc,
                    tipoDocumentoSpecified = true,
                    registrado = 0,
                };
                if (txtApellidoMaterno.Text.Length > 0)
                    nuevo.segundoApellido = txtApellidoMaterno.Text.Trim();

                identificadorPersona = compraService.insertarComprador(nuevo);
                compradorFinal = nuevo; // Guardar el nuevo comprador para usarlo más adelante
            }
            else
            {
                compradorExistente.monto -= totalAPagar;
                compraService.actualizarComprador(compradorExistente);
                compradorFinal = compradorExistente; // Usar el comprador existente
                identificadorPersona = compradorExistente.idPersona; // Obtener el ID del comprador existente
            }

            eMetodoPago mp;

            string metodoPagoSeleccionado = hfMetodoPago.Value;

            bool ok = Enum.TryParse(metodoPagoSeleccionado, ignoreCase: false, out mp);

            if (!ok)
            {
                string script = "setTimeout(function(){ mostrarModalError('Método de pago incorrecto.','Método de pago desconocido.'); }, 300);";
                ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModalError", script, true);
                return;
            }

            int idPersonaSession = Session["idUsuario"] != null ? (int)Session["idUsuario"] : 0;
            //si la cuenta está registrada, aunque lo haya comprado otra persona, se le asigna la compra a la cuenta
            reserva nuevaReserva = new reserva
            {
                // Campos heredados de constancia
                fecha = DateTime.Now, //fecha pago
                fechaSpecified = true,
                metodoPago = mp,
                metodoPagoSpecified = true,
                igv = 0.18,
                detallePago = $"Pago realizado por {txtNombres.Text.Trim()} {txtApellidoPaterno.Text.Trim()} con {ddlTipoDocumento.SelectedValue.ToString()} {txtDNI.Text}",
                total = totalAPagar,

                // Campos propios de reserva
                fechaReserva = DateTime.Parse(fechaR),  // Convertir a DateTime
                fechaReservaSpecified = true,
                iniString = horaIni,
                finString = tFin.ToString(),


                persona = new persona
                {
                    idPersona = idPersonaSession > 0 ? idPersonaSession : identificadorPersona, // Si hay sesión, usar su ID, si no, usar el nuevo ID

                }, // el objeto comprador
                espacio = espacioService.buscarEspacio(idEspacio) // el objeto espacio
            };

            int idConstancia = reservaService.insertarReserva(nuevaReserva);


            string scriptExito =
            "setTimeout(function(){ mostrarModalExito('Pago exitoso.','El pago se ha realizado con éxito.'); }, 300);" +
            "setTimeout(function() {" +
            "  window.location.href = '/Presentacion/Ventas/Reserva/ConstanciaReserva.aspx?idConstancia=" + idConstancia + "';" +
            "}, 1500);"; // 1.5 segundos para que el usuario vea el modal
            // Justo antes de redirigir a ConstanciaReserva.aspx
            Session["MostrarFeedback"] = true; //Solo muestra el feedback si el flujo fue por un pago

            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModalExito", scriptExito, true);
        }

        protected void cvDocumento_ServerValidate(object source, ServerValidateEventArgs args)
        {
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            string tipo = ddlTipoDocumento.SelectedValue;
            string numero = txtDNI.Text.Trim();
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


    }
}