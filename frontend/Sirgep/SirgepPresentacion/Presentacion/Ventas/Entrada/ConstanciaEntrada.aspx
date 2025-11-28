<%@ Page Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="ConstanciaEntrada.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Ventas.Entrada.ConstanciaEntrada" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div class="container">
        <h1 class="mb-4">
            Constancia de Entrada #<asp:Label ID="lblNumEntrada" runat="server" Text="000" />
        </h1>

        <!-- DATOS DEL EVENTO -->
        <div class="card mb-3">
            <div class="card-header modal-header-rojo text-white">Datos del Evento</div>
            <div class="card-body row g-3">
                <div class="col-12 col-md-4">
                    <strong>Nombre:</strong> <asp:Label ID="lblEvento" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Ubicación:</strong> <asp:Label ID="lblUbicacion" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Distrito:</strong> <asp:Label ID="lblDistrito" runat="server" CssClass="ms-2" />
                </div>
            </div>
        </div>

        <!-- DATOS DE LA FUNCIÓN -->
        <div class="card mb-3">
            <div class="card-header modal-header-rojo text-white">Datos de la Función</div>
            <div class="card-body row g-3">
                <div class="col-12 col-md-4">
                    <strong>Fecha:</strong> <asp:Label ID="lblFechaFuncion" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Hora Inicio:</strong> <asp:Label ID="lblHoraInicio" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Hora Fin:</strong> <asp:Label ID="lblHoraFin" runat="server" CssClass="ms-2" />
                </div>
            </div>
        </div>

        <!-- DATOS DEL COMPRADOR -->
        <div class="card mb-3">
            <div class="card-header modal-header-rojo text-white">Datos del Comprador</div>
            <div class="card-body row g-3">
                <!-- Fila con dos columnas -->
                <div class="col-12 col-md-4">
                    <strong>Nombres:</strong> <asp:Label ID="lblNombres" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Apellidos:</strong> <asp:Label ID="lblApellidos" runat="server" CssClass="ms-2" />
                </div>
                <!-- Fila con tres columnas -->
                <div class="col-12 col-md-4">
                    <strong>Tipo de Documento:</strong> <asp:Label ID="lblTipoDocumento" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Número de Documento:</strong> <asp:Label ID="lblTNumDocumento" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Correo:</strong> <asp:Label ID="lblCorreo" runat="server" CssClass="ms-2" />
                </div>
            </div>
        </div>

        <!-- DATOS DE LA CONSTANCIA -->
        <div class="card mb-3">
            <div class="card-header modal-header-rojo text-white">Datos de la Constancia del Pago</div>
            <div class="card-body row g-3">
                <div class="col-12 col-md-4">
                    <strong>Fecha:</strong> <asp:Label ID="lblFechaConstancia" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Método de pago:</strong> <asp:Label ID="lblMetodoPago" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12 col-md-4">
                    <strong>Total:</strong> <asp:Label ID="lblTotal" runat="server" CssClass="ms-2" />
                </div>
                <div class="col-12">
                    <strong>Detalle de pago:</strong> <asp:Label ID="lblDetallePago" runat="server" CssClass="ms-2" />
                </div>
            </div>
        </div>

        <!-- BOTONES -->
        <div class="d-flex justify-content-between mb-3">
            <asp:Button ID="btnVolver" runat="server" Text="Volver" CssClass="btn btn-dark" OnClick="btnVolver_Click" />
            <asp:Button ID="btnDescargarMostrarModal" runat="server" Text="Descargar Constancia" OnClientClick="setTimeout(function() { mostrarModalExito('Descarga exitosa', 'La constancia de la reserva fue descargada correctamente.'); }, 1000);" OnClick="btnDescargar_Click" CssClass="btn btn-dark" />
        </div>
    </div>
</asp:Content>