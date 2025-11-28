<%@ Page Title="" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="ListaComprador.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Usuarios.Comprador.ListaComprador" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Usuarios > Listado
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hdnIdAEliminar" runat="server" />

    <h2 class="fw-bold mb-4">Usuarios</h2>

    <!-- Filtros -->
    <div class="mb-4">
        <label class="fw-bold">Buscar:</label>
        <asp:TextBox ID="txtBusqueda" runat="server"
            CssClass="form-control d-inline-block mx-2"
            Style="width: 400px;"
            Placeholder="🔍 Buscar" />
        <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-dark" OnClick="btnBuscar_Click" />
    </div>
    <!-- Aquí va el GridView -->
    <asp:GridView ID="GvListaCompradores" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered text-center">
        <Columns>
            <asp:BoundField DataField="idComprador" HeaderText="ID" />
            <asp:BoundField DataField="nombres" HeaderText="Nombre" />
            <asp:BoundField DataField="primerApellido" HeaderText="Apellido Paterno" />
            <asp:BoundField DataField="segundoApellido" HeaderText="Apellido Materno" />
            <asp:BoundField DataField="tipoDocumento" HeaderText="Tipo Documento" />
            <asp:BoundField DataField="numeroDocumento" HeaderText="N° Documento" />
            <asp:BoundField DataField="correo" HeaderText="Correo" />
            <asp:BoundField DataField="fechaUltimaCompra" HeaderText="Última Compra" />
            <asp:TemplateField HeaderText="Acciones">
                <ItemTemplate>
                    <asp:Button ID="btnEliminar" runat="server" CssClass='<%# (bool)Eval("PuedeEliminar") ? "btn btn-danger btn-sm fw-bold" : "btn btn-danger btn-sm fw-bold disabled opacity-50" %>'
                        CommandName="Eliminar"
                        CommandArgument='<%# Eval("idComprador") %>'
                        Text="Eliminar"
                        OnClientClick='<%# (bool)Eval("PuedeEliminar") ? $"mostrarConfUsuario({Eval("idComprador")}); return false;" : "alert(\"No se puede eliminar: no han pasado 3 años desde la última compra.\"); return false;" %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <div class="d-flex justify-content-center align-items-center gap-3 mt-4 mb-4">
    <asp:Button ID="btnAnterior" runat="server" Text="Anterior" CssClass="btn btn-outline-dark" OnClick="btnAnterior_Click" />
    <asp:Label ID="lblPaginaActual" runat="server" CssClass="fw-bold fs-5 m-0 px-2"></asp:Label>
    <asp:Button ID="btnSiguiente" runat="server" Text="Siguiente" CssClass="btn btn-outline-dark" OnClick="btnSiguiente_Click" />
    </div>

    <!-- Modal de Confirmación -->
    <div class="modal fade" id="mostrarConfUsuario" tabindex="-1" aria-labelledby="mostrarConfUsuarioLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header modal-header-rojo text-white">
                    <h5 class="modal-title" id="mostrarConfUsuarioLabel">Ventana de confirmación</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>
                <div class="modal-body d-flex align-items-center modal-body-confirmacion">
                    <div class="icono-confirmacion me-3">
                        <div class="icono-circulo">
                            <i class="bi bi-info-lg fs-1"></i>
                        </div>
                    </div>
                    <div id="modalConfirmacionCuerpo" class="fs-5">
                        ¿Está seguro que desea eliminar esta cuenta?
                    </div>
                </div>
                <div class="modal-footer justify-content-center">
                    <button type="button" class="btn btn-dark px-4" data-bs-dismiss="modal"><em>No</em></button>
                    <asp:Button ID="btnConfirmarAccion" runat="server" CssClass="btn btn-dark px-4" Text="Sí" OnClick="btnConfirmarAccion_Click" />
                </div>
            </div>
        </div>
    </div>
    
    
    
    <script type="text/javascript">
        function mostrarConfUsuario(id) {
            var hiddenField = document.getElementById('<%= hdnIdAEliminar.ClientID %>');
            if (hiddenField) hiddenField.value = id;

            var modalElement = document.getElementById('mostrarConfUsuario');
            var modalEliminar = new bootstrap.Modal(modalElement);
            modalEliminar.show();
        }

    </script>
</asp:Content>