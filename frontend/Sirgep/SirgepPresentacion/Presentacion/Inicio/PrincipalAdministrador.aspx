<%@ Page Title="Principal Administrador" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="PrincipalAdministrador.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Inicio.PrincipalAdministrador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Contenido" runat="server">
    <div class="container principal-wrapper">
        

        <div class="row w-100 align-items-center">
            <!-- Columna izquierda: bloques de gestión -->
            <div class="col-md-6">
            <h2 class="mb-3 principal-title">Bienvenido al Espacio Administrativo !!</h2>
                <h5 class="fw-semibold mb-3">Gestionar Espacios</h5>
                <div class="row mb-4">
                    <div class="col-6 mb-2">
                        <asp:Button ID="btnManejarEspacios" runat="server" CssClass="btn btn-dark w-100" Text="Manejar Espacios" OnClick="btnManejarEspacios_Click" OnClientClick="mostrarModalCarga('Cargando...', 'Redireccionando. Espere un momento.');"/>
                    </div>
                    <div class="col-6 mb-2">
                        <asp:Button ID="btnConsultarReservas" runat="server" CssClass="btn btn-dark w-100" Text="Consultar Reservas" OnClick="btnConsultarReservas_Click" OnClientClick="mostrarModalCarga('Cargando...', 'Redireccionando. Espere un momento.');"/>
                    </div>
                </div>

                <h5 class="fw-semibold mb-3">Gestionar Eventos</h5>
                <div class="row mb-4">
                    <div class="col-6 mb-2">
                        <asp:Button ID="btnManejarEventos" runat="server" CssClass="btn btn-dark w-100" Text="Manejar Eventos" OnClick="btnManejarEventos_Click" OnClientClick="mostrarModalCarga('Cargando...', 'Redireccionando. Espere un momento.');"/>
                    </div>
                    <div class="col-6 mb-2">
                        <asp:Button ID="btnConsultarEntradas" runat="server" CssClass="btn btn-dark w-100" Text="Consultar Entradas" OnClick="btnConsultarEntradas_Click" OnClientClick="mostrarModalCarga('Cargando...', 'Redireccionando. Espere un momento.');"/>
                    </div>
                </div>

                <h5 class="fw-semibold mb-3">Gestionar Cuentas de Compradores</h5>
                <div class="row mb-4">
                    <div class="col-6 mb-2">
                        <asp:Button ID="btnAdministrarCompradores" runat="server"
                            CssClass="btn btn-dark w-100"
                            Text="Administrar Compradores"
                            OnClick="btnAdministrarCompradores_Click" 
                            OnClientClick="mostrarModalCarga('Cargando...', 'Redireccionando. Espere un momento.');"/>
                    </div>
                </div>

                <h5 class="fw-semibold mb-3">Analizar Estadísticas de Servicios</h5>
                <div class="row mb-4">
                    <div class="col-6 mb-2">
                        <asp:Button ID="btnStats" runat="server"
                            CssClass="btn btn-dark w-100"
                            Text="Ver Estadísticas"
                            OnClick="btnStats_click" 
                            OnClientClick="mostrarModalCarga('Cargando...', 'Redireccionando. Espere un momento.');"/>
                    </div>
                </div>

            </div>
            <!-- Columna derecha: imagen -->
            <div class="col-md-6 d-flex justify-content-center align-items-start">
                <img src="/Images/principal/lima_evento.jpg" class="img-fluid rounded" alt="Evento Lima" style="max-width: 80%;" />
            </div>
        </div>
    </div>
</asp:Content>