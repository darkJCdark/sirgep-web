using DocumentFormat.OpenXml.Math;
using SirgepPresentacion.ReferenciaDisco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SirgepPresentacion.Presentacion.Ventas.Entrada
{
    public partial class CompraEntrada : System.Web.UI.Page
    {
        CompraWSClient compraService;
        FuncionWSClient fWs;
        EntradaWSClient entradaWS;
        PersonaWSClient personaWS;
        EventoWSClient eventoWS;
        protected void Page_Init(object sender, EventArgs e)
        {
            compraService = new CompraWSClient();
            fWs = new FuncionWSClient();
            entradaWS = new EntradaWSClient();
            personaWS = new PersonaWSClient();
            eventoWS = new EventoWSClient();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                // Aseguramos que el session para el feedback esté en false al cargar la página
                Session["MostrarFeedback"] = false;
                int idFuncion = int.Parse(Request.QueryString["idFuncion"]);
                var funcion = fWs.buscarFuncionId(idFuncion); // Simular ID de función
                evento evento = compraService.buscarEventos(funcion.evento.idEvento);

                lblEvento.Text = evento.nombre;
                lblUbicacion.Text = evento.ubicacion;
                lblReferencia.Text = evento.referencia;

                string fecha = funcion.fecha.ToString();
                string horaIni = funcion.horaInicio.ToString();
                string horaFin = funcion.horaFin.ToString();
                string cantidad = "1"; //siempre se compra 1 entrada

                if (!string.IsNullOrEmpty(evento.archivoImagen))
                {
                    imgEvento.ImageUrl = "~/" + evento.archivoImagen;
                    imgEvento.Visible = true;
                }
                else
                {
                    imgEvento.Visible = false;
                }

                lblHorario.Text = $"{horaIni} - {horaFin}";
                lblFecha.Text = DateTime.Parse(fecha).ToString("dd/MM/yyyy");
                lblCantidad.Text = cantidad;
                lblTotal.Text = evento.precioEntrada.ToString();

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

            //documentoNoValido(sender, e);

            // Validar método de pago
            if (string.IsNullOrEmpty(hfMetodoPago.Value))
            {
                string script = "setTimeout(function(){ mostrarModalError('Método de pago faltante.','Debe seleccionar un método de pago.'); }, 300);";
                ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModalError", script, true);
                return;
            }



            string dni = txtDNI.Text.Trim();
            var compradorExistente = compraService.buscarCompradorPorDni(dni);
            int idPersona1;
            int idFuncion = int.Parse(Request.QueryString["idFuncion"]);
            var funcion = fWs.buscarFuncionId(idFuncion); // Simular ID de función
            evento evento = compraService.buscarEventos(funcion.evento.idEvento);
            double precio = evento.precioEntrada;
            double totalAPagar = precio;

            // ---------- Comprobación de saldo ----------
            if (compradorExistente != null && compradorExistente.monto < totalAPagar)
            {
                string script = "setTimeout(function(){ mostrarModalError('Error en pago.','Saldo insuficiente.'); }, 300);";
                ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModalError", script, true);
                return;
            }

            // ---------- Insertar / actualizar comprador ----------
            if (compradorExistente == null)
            {
                eTipoDocumento tdoc = (eTipoDocumento)Enum.Parse(typeof(eTipoDocumento), ddlTipoDocumento.SelectedValue, true);
                comprador nuevo = new comprador
                {
                    nombres = txtNombres.Text.Trim(),
                    primerApellido = txtApellidoPaterno.Text.Trim(),
                    segundoApellido = " ", //no obligatorio
                    numDocumento = txtDNI.Text.Trim(),
                    correo = txtCorreo.Text.Trim(),
                    tipoDocumento = tdoc,
                    tipoDocumentoSpecified = true,
                    registrado = 0, //se guarda un comprador no registrado
                };
                if (txtApellidoMaterno.Text.Length > 0) nuevo.segundoApellido = txtApellidoMaterno.Text.Trim();
                idPersona1 = compraService.insertarComprador(nuevo);
            }
            else
            {
                compradorExistente.monto -= totalAPagar;
                compraService.actualizarComprador(compradorExistente);
                idPersona1 = compradorExistente.idPersona;
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
            
            int numE = evento.cantEntradasVendidas + 1;
            evento.cantEntradasVendidas = numE;

            // Actualizar el evento con la nueva cantidad de entradas vendidas
            eventoWS.actualizarEvento(evento);
            // ---------- Insertar constancia ----------
            //verificar si está en una cuenta registrada
            int idPersonaSession = Session["idUsuario"] != null ? (int)Session["idUsuario"] : 0;
            //si la cuenta está registrada, aunque lo haya comprado otra persona, se le asigna la compra a la cuenta
            entrada nEntrada = new entrada
            {
                numEntrada = numE,
                fecha = DateTime.Now,
                fechaSpecified = true,
                metodoPago = mp,
                metodoPagoSpecified = true,
                detallePago = $"Pago realizado por {txtNombres.Text.Trim()} {txtApellidoPaterno.Text.Trim()} con {ddlTipoDocumento.SelectedValue.ToString()} {txtDNI.Text}",
                total = totalAPagar,
                igv = 0.18,
                persona = new persona
                {
                    idPersona = idPersonaSession > 0 ? idPersonaSession : idPersona1, // Si hay sesión, usar su ID, si no, usar el nuevo ID
                },
                funcion = new funcion
                {
                    idFuncion = int.Parse(Request.QueryString["idFuncion"]),
                    //idFuncion = 1, // Simular ID de función
                },
            };

            //int idConstancia=compraService.insertarConstancia(nueva);
            int idConstancia = entradaWS.insertarEntrada(nEntrada);
            string scriptExito = "setTimeout(function(){ mostrarModalExito('Pago exitoso.','El pago se ha realizado con éxito.'); }, 300);" +
                                 "setTimeout(function() {" +
                                 "  window.location.href = '/Presentacion/Ventas/Entrada/ConstanciaEntrada.aspx?idConstancia=" + idConstancia + "';" +
                                 "}, 1500);"; // 1.5 segundos para que el usuario vea el modal;
            // Justo antes de redirigir a ConstanciaEntrada.aspx
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