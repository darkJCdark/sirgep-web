<%@ Page Title="" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Inicio.SignIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server" />
<asp:Content ID="Content2" ContentPlaceHolderID="Encabezado" runat="server" />
<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <div class="row justify-content-center align-items-center" style="min-height: 80vh;">
        <div class="col-md-10 shadow-lg rounded p-5 bg-white">
            <div class="row">
                <!--izquierda-->
                <div class="col-md-6 d-flex flex-column justify-content-center align-items-center bg-dark text-white py-4 rounded-start">
                    <!-- Contenedor de imagen con tamaño fijo -->
                    <div class="w-100 d-flex justify-content-center mb-3" style="max-height: 450px; overflow: hidden;">
                        <img src="/Images/img/ImagenSI1.JPG" class="img-fluid rounded-3 shadow" style="max-height: 450px; object-fit: cover;" alt="Imagen 1" />
                    </div>
                    <div class="w-100 d-flex justify-content-center mb-3" style="max-height: 450px; overflow: hidden;">
                        <img src="/Images/img/ImagenSI2.JPG" class="img-fluid rounded-3 shadow" style="max-height: 450px; object-fit: cover;" alt="Imagen 2" />
                    </div>
                    <div class="w-100 d-flex justify-content-center" style="max-height: 450px; overflow: hidden;">
                        <img src="/Images/img/ImagenSI3.JPG" class="img-fluid rounded-3 shadow" style="max-height: 450px; object-fit: cover;" alt="Imagen 3" />
                    </div>
                </div>
                <!--derecha-->
                <div class="col-md-6 px-4">
                    <div class ="mt-4">
                        <div class="w-100 text-center">
                         <h3 class="fw-bold mb-3">Registrarse</h3>
                            <div class="mb-4">
                                ¿Tienes una cuenta?
                                <asp:LinkButton ID="lnkIniciarSesion" runat="server" CausesValidation="false" OnClick="lnkIniciarSesion_Click">
                                    Inicia sesión
                                </asp:LinkButton>
                            </div>
                        </div>

                    <div class="row g-3">

                        <!-- Grupo: Datos personales -->
                        <div class="p-3 border rounded bg-light mb-3">
                            <h5 class="fw-semibold">Datos personales</h5>
                            <p class="mb-2">Ingrese aquí su nombre, primer apellido y, si aplica, segundo apellido.</p>

                            <div class="mb-2">
                                <!-- Nombres -->
                                <asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" placeholder="Nombres" MaxLength="45"></asp:TextBox>
                                <span id="msgNombresMax" class="text-danger" style="display:none;">*Máximo 45 caracteres</span>
                                <asp:RequiredFieldValidator ControlToValidate="txtNombres" runat="server" ErrorMessage="*Campo obligatorio" CssClass="text-danger" Display="Dynamic" />
                                <asp:RegularExpressionValidator ControlToValidate="txtNombres" runat="server"
                                ErrorMessage="Solo se permiten letras en el nombre"
                                CssClass="text-danger" Display="Dynamic"
                                ValidationExpression="^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$" />
                            </div>
                            <!-- Primer Apellido -->
                            <div class="mb-2">
                                <asp:TextBox ID="txtApellidoPaterno" runat="server" CssClass="form-control" placeholder="Primer Apellido" MaxLength="45"></asp:TextBox>
                                <span id="msgApellidoPaternoMax" class="text-danger" style="display:none;">*Máximo 45 caracteres</span>
                                <asp:RequiredFieldValidator ControlToValidate="txtApellidoPaterno" runat="server" ErrorMessage="*Campo obligatorio" CssClass="text-danger" Display="Dynamic" />
                                <asp:RegularExpressionValidator ControlToValidate="txtApellidoPaterno" runat="server"
                                ErrorMessage="Solo se permiten letras en el apellido paterno"
                                CssClass="text-danger" Display="Dynamic"
                                ValidationExpression="^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$" />

                            </div>
                            <!-- Segundo Apellido (opcional) -->
                            <div class="mb-2">
                                <asp:TextBox ID="txtApellidoMaterno" runat="server" CssClass="form-control" placeholder="Segundo Apellido (opcional)" MaxLength="45"></asp:TextBox>
                                <span id="msgApellidoMaternoMax" class="text-danger" style="display:none;">*Máximo 45 caracteres</span>
                                <asp:RegularExpressionValidator ControlToValidate="txtApellidoMaterno" runat="server"
                                ErrorMessage="Solo se permiten letras en el apellido materno"
                                CssClass="text-danger" Display="Dynamic"
                                ValidationExpression="^([a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]*)$" />
                            </div>
                        </div>

                        <!-- Grupo: Documento -->
                        <div class="p-3 border rounded bg-light mb-3">
                            <h5 class="fw-semibold">Documento de Identidad</h5>
                            <p class="mb-2">Complete los datos de su documento de identificación.</p>

                            <div class="mb-2">
                                <asp:DropDownList ID="ddlTipoDocumento" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Seleccione tipo de documento" Value="" />
                                    <asp:ListItem Text="DNI" Value="DNI" />
                                    <asp:ListItem Text="Carnet de Extranjería" Value="CARNETEXTRANJERIA" />
                                    <asp:ListItem Text="Pasaporte" Value="PASAPORTE" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ControlToValidate="ddlTipoDocumento" InitialValue="" runat="server" ErrorMessage="*Campo obligatorio" CssClass="text-danger" Display="Dynamic" />
                            </div>
                            <div class="mb-2">
                                <asp:TextBox ID="txtNumeroDocumento" runat="server" CssClass="form-control" placeholder="Número de Documento"></asp:TextBox>
                                <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="txtNumeroDocumento" CssClass="text-danger" Display="Dynamic" OnServerValidate="cvDocumento_ServerValidate" ValidateEmptyText="true" />
                                <asp:RequiredFieldValidator ControlToValidate="txtNumeroDocumento" runat="server" ErrorMessage="*Campo obligatorio" CssClass="text-danger" Display="Dynamic" />
                            </div>
                        </div>

                        <!-- Grupo: Ubicación -->
                        <div class="p-3 border rounded bg-light mb-3">
                            <h5 class="fw-semibold">Ubicación</h5>
                            <p class="mb-2">Elija aquí el distrito en el que vive o del que busca eventos.</p>
                            <div class="row">
                                <div class="col-md-4 mb-2">
                                    <asp:DropDownList ID="ddlDepartamento" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartamento_SelectedIndexChanged" />
                                </div>
                                <div class="col-md-4 mb-2">
                                    <asp:DropDownList ID="ddlProvincia" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlProvincia_SelectedIndexChanged" />
                                </div>
                                <div class="col-md-4 mb-2">
                                    <asp:DropDownList ID="ddlDistrito" runat="server" CssClass="form-select" />
                                </div>
                            </div>
                        </div>

                        <!-- Grupo: Cuenta -->
                        <div class="p-3 border rounded bg-light mb-3">
                            <h5 class="fw-semibold">Cuenta</h5>
                            <p class="mb-2">Ingrese las credenciales y datos de su nueva cuenta.</p>

                            <!-- Usuario -->
                            <div class="mb-2">
                                <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Nombre de usuario" MaxLength="45"></asp:TextBox>
                                <span id="msgUsuarioMax" class="text-danger" style="display:none;">*Máximo 45 caracteres</span>
                                <asp:RequiredFieldValidator ControlToValidate="txtUsuario" runat="server" ErrorMessage="*Campo obligatorio" CssClass="text-danger" Display="Dynamic" />

                                </div>
                            <div class="mb-2">
                                <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" placeholder="Correo electrónico" MaxLength="45"></asp:TextBox>
                                <span id="msgCorreoMax" class="text-danger" style="display:none;">*Máximo 45 caracteres</span>

                                <!-- Validación de campo vacío -->
                                <asp:RequiredFieldValidator ControlToValidate="txtCorreo" runat="server"
                                    ErrorMessage="*Campo obligatorio" CssClass="text-danger" Display="Dynamic" />

                                <!-- Validación de formato -->
                                 <asp:RegularExpressionValidator ControlToValidate="txtCorreo" runat="server"
                                     ErrorMessage="*Correo inválido" CssClass="text-danger" Display="Dynamic"
                                     ValidationExpression="^[\w\.\-]+@[\w\-]+(\.[\w\-]+)+$" />

                                <!-- Validación de existencia (custom) -->
                                <asp:CustomValidator ID="cvCorreo" runat="server"
                                    ControlToValidate="txtCorreo"
                                    OnServerValidate="cvCorreo_ServerValidate"
                                    ErrorMessage="Este correo ya está registrado"
                                    CssClass="text-danger" Display="Dynamic" ValidateEmptyText="true" />
                            </div>
                            <!-- Contraseña -->
                            <div class="mb-2">
                                
                                <asp:TextBox ID="txtContraseña" runat="server" CssClass="form-control" TextMode="Password" placeholder="Contraseña" MaxLength="16"></asp:TextBox>
                                <span id="msgContraseñaMax" class="text-danger" style="display:none;">*Máximo 16 caracteres</span>
                                <asp:RequiredFieldValidator ControlToValidate="txtContraseña" runat="server" ErrorMessage="*Campo obligatorio" CssClass="text-danger" Display="Dynamic" />
                            </div>
                            <div class="mb-2">
                                <asp:TextBox ID="txtMonto" runat="server" CssClass="form-control" placeholder="Monto a agregar (mínimo S/ 50, múltiplos de 10)"></asp:TextBox>
                                <asp:CustomValidator ID="cvMonto" runat="server" ErrorMessage="*Monto inválido" CssClass="text-danger" Display="Dynamic" OnServerValidate="cvMonto_ServerValidate" />
                            </div>
                        </div>

                        <!-- Botón -->
                        <div class="col-12 text-center mt-3">
                            <asp:Button ID="btnCrearCuenta" runat="server" CssClass="btn btn-dark px-4" Text="Crear cuenta" OnClick="btnCrearCuenta_Click" />
                            <asp:Label ID="lblCorreoError" runat="server" CssClass="text-danger" />
                        </div>

                    </div>
                </div>
                </div>
            </div>
        </div>
    </div>
    <!--Sript para que muestre el mensaje de error si se intentan escribir más caracteres de los establecidos -->
    <script type="text/javascript">
        function addMaxLengthMsg(inputId, msgId, max) {
            var txt = document.getElementById(inputId);
            var msg = document.getElementById(msgId);
            if (!txt || !msg) return;
            txt.addEventListener('input', function() {
                if (txt.value.length === max) {
                    msg.style.display = 'inline';
                } else {
                    msg.style.display = 'none';
                }
            });
        }
        document.addEventListener('DOMContentLoaded', function() {
            addMaxLengthMsg('<%= txtNombres.ClientID %>', 'msgNombresMax', 45);
            addMaxLengthMsg('<%= txtApellidoPaterno.ClientID %>', 'msgApellidoPaternoMax', 45);
            addMaxLengthMsg('<%= txtApellidoMaterno.ClientID %>', 'msgApellidoMaternoMax', 45);
            addMaxLengthMsg('<%= txtUsuario.ClientID %>', 'msgUsuarioMax', 45);
            addMaxLengthMsg('<%= txtCorreo.ClientID %>', 'msgCorreoMax', 45);
            addMaxLengthMsg('<%= txtContraseña.ClientID %>', 'msgContraseñaMax', 16);
        });
    </script>

</asp:Content>