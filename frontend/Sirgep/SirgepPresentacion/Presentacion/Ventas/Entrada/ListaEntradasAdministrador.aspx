<%@ Page Title="" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="ListaEntradasAdministrador.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Ventas.Entrada.ListaEntradasAdministrador" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Encabezado" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hdnConstancia" runat="server"/>
    <asp:HiddenField ID="hdnEntrada" runat="server"/>

    <!-- Título principal -->
    <h1 class="fw-bold mb-4">Municipalidad &gt; Entradas</h1>

    <!-- Búsqueda -->
    <div class="mb-3">
        <asp:TextBox OnTextChanged="txtBusqueda_TextChanged" ID="txtBusqueda" runat="server" CssClass="input-busqueda" Placeholder="🔍 Buscar" AutoPostBack="true" />
    </div>

    <!-- Tabla -->
    <div class="table-responsive">
        <table class="table table-bordered text-center tabla-reservas">
            <thead class="table-light fw-bold">
                <tr>
                    <th>Abrir</th>
                    <th>Código</th>
                    <th>Nombre</th>
                    <th>Distrito</th>
                    <th>Fecha Constancia</th>
                    <th>Hora de Inicio</th>
                    <th>Hora de Fin</th>
                    <th>Acciones</th>
                    <th>¿Activa?</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptEntradas" runat="server" OnItemDataBound="rptEntradas_ItemDataBound">
                    <ItemTemplate>

                        <tr>
                            <td>
                                <a href='<%# "ConstanciaEntrada.aspx?idConstancia=" + Eval("idConstancia") %>'>
                                    <img src="/Images/icons/open-link.png" alt="Abrir" style="width: 24px;" />
                                </a>
                            </td>
                            <td><%# Eval("numEntrada") %></td>
                            <td><%# Eval("nombreEvento") %></td>
                            <td><%# Eval("nombreDistrito") %></td>
                            <td><%# String.Format("{0:yyyy-MM-dd}", Eval("fechaConstancia")) %></td>
                            <td><%# String.Format("{0:HH:mm}", Eval("horaInicio")) %></td>
                            <td><%# String.Format("{0:HH:mm}", Eval("horaFin")) %></td>
                            <td>
                                <asp:Button ID="btnEliminar" runat="server" CssClass="btn btn-danger btn-sm fw-bold"
                                    Text="Eliminar" CommandArgument='<%# String.Format("{0:yyyy-MM-dd}", Eval("fechaConstancia")) + "|" + Eval("numEntrada") %>'
                                    OnClick="btnEliminar_Click"/>
                            </td>
                            <td><%# Eval("Estado").ToString() == "65" ? "Sí" : "No" %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>

    <!-- Controles de paginación -->
    <div class="d-flex justify-content-center align-items-center mt-4 gap-3">
        <asp:Button ID="btnAnterior" runat="server"
            CssClass="btn btn-outline-secondary btn-sm me-4"
            Text="Anterior" OnClick="btnAnterior_Click" />

        <asp:Label ID="lblPaginaActual" runat="server"
            CssClass="fw-bold text-dark mx-2" />

        <asp:Button ID="btnSiguiente" runat="server"
            CssClass="btn btn-outline-secondary btn-sm ms-4"
            Text="Siguiente" OnClick="btnSiguiente_Click" />
    </div>
    <div class="mb-3"></div>
    <!-- Modal de Confirmación -->
    <div class="modal fade" id="modalConfEntrada" tabindex="-1" aria-labelledby="modalConfEntradaLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header modal-header-rojo text-white">
                    <h5 class="modal-title" id="modalConfEntradaLabel">Ventana de confirmación</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <div class="modal-body d-flex align-items-center modal-body-confirmacion">
                    <div class="icono-confirmacion me-3">
                        <div class="icono-circulo">
                            <i class="bi bi-info-lg fs-1"></i>
                        </div>
                    </div>
                    <div id="modalConfirmacionEntrada" class="fs-5">
                        ¿Está seguro que desea eliminar esta Entrada?
       
                    </div>
                </div>

                <div class="modal-footer justify-content-center">
                    <button type="button" class="btn btn-dark px-4" data-bs-dismiss="modal"><em>No</em></button>
                    <asp:Button ID="btnEliminarConfirmado" CssClass="btn btn-danger" runat="server" Text="Sí"
                        
                        OnClick="btnEliminarConfirmado_Click" />
                </div>

            </div>
        </div>
    </div>
    
    <script>
        function mostrarModalConfEntrada() {
            var modalElement = document.getElementById('modalConfEntrada');
            var modalEliminar = new bootstrap.Modal(modalElement);
            modalEliminar.show();
        }

    </script>

</asp:Content>
