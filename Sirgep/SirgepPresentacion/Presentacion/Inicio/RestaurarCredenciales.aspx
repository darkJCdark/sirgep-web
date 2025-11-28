<%@ Page Title="Recuperar Contraseña" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="RestaurarCredenciales.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Inicio.RestaurarCredenciales" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Recuperar Contraseña
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Encabezado" runat="server">
    <!-- Encabezado vacío para mantener consistencia -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <div class="d-flex justify-content-center align-items-center" style="min-height: 80vh;">
        <div class="recuperar-form-block">
            <h3 class="text-center mb-3 fw-bold">¿Olvidaste tu contraseña?</h3>
            <p class="text-center mb-4">Envíanos tu correo y te ayudaremos a reestablecerla</p>
            <asp:Label ID="lblMensaje" runat="server" CssClass="text-danger fw-bold d-block mb-2" />
            <div class="mb-3">
                <asp:Label AssociatedControlID="txtCorreo" runat="server" CssClass="form-label" Text="Correo electrónico registrado" />
                <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" placeholder="ejemplo@correo.com"></asp:TextBox>
                <!-- Validación de campo vacío -->
                <asp:RequiredFieldValidator ControlToValidate="txtCorreo" runat="server"
                    ErrorMessage="*Campo obligatorio" CssClass="text-danger" Display="Dynamic" />
                <!-- Validación de formato -->
                <asp:RegularExpressionValidator ControlToValidate="txtCorreo" runat="server"
                    ErrorMessage="*Correo inválido" CssClass="text-danger" Display="Dynamic"
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
            </div>
            <asp:Button ID="btnEnviar" runat="server" CssClass="btn btn-dark w-100" Text="Enviar" OnClick="btnEnviar_Click" OnClientClick="if (Page_ClientValidate()) { mostrarModalCarga('Cargando...', 'La solicitud se está procesando.'); } else { return false; }"/>
            <div class="text-center mt-3">
                <a href="LogIn.aspx" class="text-decoration-none">Volver a iniciar sesión</a>
            </div>
            <small class="d-block mt-2 text-muted text-center">Pronto un encargado te podrá ayudar por correo.</small>
        </div>
    </div>
</asp:Content>
