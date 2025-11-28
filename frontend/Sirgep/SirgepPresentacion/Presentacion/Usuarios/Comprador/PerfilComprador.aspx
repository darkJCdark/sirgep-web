<%@ Page Title="" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="PerfilComprador.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Usuarios.Comprador.PerfilComprador" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Encabezado" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <div class="container">
       <div class="d-flex justify-content-between align-items-center mb-4">
            <h1 class="mb-0">Informacion Registrada</h1>
            <asp:Button ID="btnEliminarCuenta" runat="server"
            Text="Eliminar cuenta"
            CssClass="btn btn-danger btn-sm fw-bold"
            OnClick="btnEliminarCuenta_Click"
            OnClientClick="mostrarModalConfirmacion('Eliminar cuenta', '¿Estás seguro de que deseas eliminar tu cuenta? Esta acción no se puede deshacer.'); return false;" />
       </div>
        <!-- DATOS DEL PERFIL-->
        <div class="card mb-3">
            <div class="card-header modal-header-rojo text-white d-flex justify-content-between align-items-center">
                <span>Datos del Perfil</span>
                
            </div>
            <div class="card-body row g-3">
                <div class="col-12 col-md-4">
                    <strong>Nombres:</strong> <asp:Label ID="lblNombres" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Primer Apellido:</strong> <asp:Label ID="lblPrimerApellido" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Segundo Apellido:</strong> <asp:Label ID="lblSegundoApellido" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Tipo Documento:</strong> <asp:Label ID="lblTipoDocumento" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Numero Documento:</strong> <asp:Label ID="lblNumeroDocumento" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Monto Billetera:</strong> <asp:Label ID="lblMontoBilletera" runat="server" CssClass="ms-2" />
                </div>
            </div>
        </div>
        <!-- DATOS DE UBICACION-->
        <div class="card mb-3">
            <div class="card-header modal-header-rojo text-white">Datos de Ubicación</div>
            <div class="card-body row g-3">
                <!-- Departamento -->
                <div class="col-12 col-md-4 d-flex align-items-center">
                    <strong>Departamento:</strong>
                    <asp:Label ID="lblDepartamento" runat="server" CssClass="ms-2" />
                </div>
                <!-- Provincia -->
                <div class="col-12 col-md-4 d-flex align-items-center">
                    <strong>Provincia:</strong>
                    <asp:Label ID="lblProvincia" runat="server" CssClass="ms-2" />
                </div>
                <!-- Distrito -->
                <div class="col-12 col-md-4">
                    <div class="d-flex align-items-center gap-2">
                        <strong>Distrito:</strong>
                        <asp:TextBox ID="txtDistrito" runat="server" CssClass="form-control form-control-sm text-dark" Style="width: 120px;" />
                        <asp:Button ID="btnGuardarDistrito" runat="server" Text="Actualizar" CssClass="btn btn-dark btn-sm" OnClick="btnGuardarDistrito_Click" />
                    </div>
                </div>
            </div>
        </div>
        <!-- DATOS DE CUENTA-->
        <div class="card mb-3">
            <div class="card-header modal-header-rojo text-white">Datos de Cuenta</div>
            <div class="card-body row g-3">
                <div class="col-12 col-md-4">
                    <strong>Correo:</strong> <asp:Label ID="lblCorreo" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Contraseña:</strong> <asp:Label ID="lblContrasenia" runat="server" CssClass="ms-2" />
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            var btnConfirmar = document.getElementById('btnConfirmarAccion');
            if (btnConfirmar) {
                btnConfirmar.onclick = function () {
                    __doPostBack('<%= btnEliminarCuenta.UniqueID %>', '');
                };
            }
        });
    </script>
</asp:Content>