<%@ Page Title="" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="PrincipalInvitado.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Inicio.PrincipalInvitado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Contenido" runat="server">
    <div class="d-flex align-items-center justify-content-center ">
        <div class="container principal-wrapper">
            <div class="row w-100 align-items-center">
                <!-- Columna izquierda: bloques de gestión -->
                <div class="col-md-6 text-center text-md-start">
                    <h2 class="mb-3 principal-title">Sistema Integral de Reservas y Gestión de Espacios Públicos</h2>
                    <p class="principal-subtitle">Reserva espacios públicos municipales de manera sencilla</p>
                    <div class="row mb-4 justify-content-center justify-content-md-start">
                        <div class="col-10 col-md-6 mb-2">
                            <asp:Button ID="btnReservar" runat="server" CssClass="btn btn-dark w-100" Text="Reservar" OnClick="btnReservar_Click" OnClientClick="mostrarModalCarga('Cargando...', 'Redireccionando a Formulario de Reservas.');" />
                        </div>
                        <div class="col-10 col-md-6 mb-2">
                            <asp:Button ID="btnVerEventos" runat="server" CssClass="btn btn-dark w-100" Text="Ver Eventos" OnClick="btnVerEventos_Click" OnClientClick="mostrarModalCarga('Cargando...', 'Redireccionando a Buscador de Eventos.');"/>
                        </div>
                    </div>
                </div>
                <!-- Columna derecha: imagen -->
                <div class="col-md-6 d-flex justify-content-center align-items-center">
                    <img src="/Images/principal/lima_evento.jpg" class="img-fluid rounded" alt="Evento Lima" style="max-width: 90%;" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
