<%@ Page Title="" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="ListaEntradasComprador.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Ventas.Entrada.ListaEntradasComprador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Encabezado" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <!-- Título principal -->
    <h2 class="fw-bold mb-4">Entradas</h2>
    <!-- Búsqueda y Filtros -->
    <div class="busqueda-filtros mb-4">
        <div class="filtros d-flex align-items-center flex-wrap gap-3">
            <label class="fw-bold me-2 mb-0">Filtros:</label>
            <!-- Fecha Desde -->
            <div class="d-flex align-items-center">
                <label class="me-1 mb-0">Desde:</label>
                <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <!-- Fecha Hasta -->
            <div class="d-flex align-items-center">
                <label class="me-1 mb-0">Hasta:</label>
                <asp:TextBox ID="txtFechaFin" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <!-- RadioButtonList para Estado con estilos en línea -->
            <div class="d-flex align-items-center">
                <asp:Label runat="server" AssociatedControlID="rblEstados" Style="margin-right: 10px; font-weight: normal; color: black;">Estado:</asp:Label>
                <asp:RadioButtonList ID="rblEstados" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                    Style="display: flex; gap: 20px; color: black; font-weight: normal;">
                    <asp:ListItem Text="Vigentes" Value="Vigentes" />
                    <asp:ListItem Text="Finalizadas" Value="Finalizadas" />
                </asp:RadioButtonList>
            </div>
            <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-dark" OnClientClick="mostrarModalCarga('Filtrando...', 'Estamos buscando tus entradas.');" OnClick="btnFiltrar_Click" />
            <asp:Button ID="btnMostrarTodos" runat="server" Text="Mostrar Todos" CssClass="btn btn-dark" OnClientClick="mostrarModalCarga('Mostrando...', 'Estamos buscando todas tus entradas.');" OnClick="btnMostrarTodos_Click" />
        </div>
    </div>
    <!-- Descarga -->
    <div class="container">
        <div class="d-flex justify-content-between align-items-center mb-3">
            <!-- Mensaje alineado a la izquierda, sin negrita -->
            <asp:Label ID="lblMensaje" runat="server" CssClass="mb-0 text-dark" />
            <!-- Botón alineado a la derecha -->
            <asp:Button ID="btnDescargarMostrarModal" runat="server"
                Text="Descargar Lista de Entradas"
                OnClick="btnDescargar_Click"
                OnClientClick="mostrarModalCarga('Descargando', 'Estamos generando el archivo Excel. Esto puede tardar unos segundos...');"
                CssClass="btn btn-dark" />
        </div>
    </div>
    <!-- GridView -->
    <asp:GridView ID="GvListaEntradasComprador" runat="server" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true"
        AllowPaging="true" PageSize="5" OnPageIndexChanging="GvListaEntradasComprador_PageIndexChanging"
        CssClass="table table-striped table-responsive table-hover text-center" HeaderStyle-HorizontalAlign="Center">
        <Columns>
            <asp:TemplateField HeaderText="Abrir">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" CssClass="text-center" />
                <ItemTemplate>
                    <asp:LinkButton ID="BtnAbrir" runat="server"
                        CommandArgument='<%# Eval("IdConstancia") %>'
                        OnClick="BtnAbrir_Click"
                        CssClass="btn btn-link"
                        OnClientClick="mostrarModalCarga('Abriendo...', 'Estamos abriendo tu entrada.');"
                        ToolTip="Abrir entrada">
                    <i class="bi bi-box-arrow-up-right"></i>
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="NumEntrada" HeaderText="Nro Entrada">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" CssClass="text-center" />
            </asp:BoundField>

            <asp:BoundField DataField="NombreEvento" HeaderText="Evento">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" CssClass="text-center" />
            </asp:BoundField>

            <asp:BoundField DataField="Ubicacion" HeaderText="Ubicación">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" CssClass="text-center" />
            </asp:BoundField>

            <asp:BoundField DataField="NombreDistrito" HeaderText="Distrito">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" CssClass="text-center" />
            </asp:BoundField>
            <asp:BoundField DataField="FechaFuncion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" CssClass="text-center" />
            </asp:BoundField>
            <asp:BoundField DataField="HoraInicio" HeaderText="Hora Inicio" DataFormatString="{0:HH:mm}" HtmlEncode="false">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" CssClass="text-center" />
            </asp:BoundField>
            <asp:BoundField DataField="HoraFin" HeaderText="Hora Fin" DataFormatString="{0:HH:mm}" HtmlEncode="false">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" CssClass="text-center" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>
</asp:Content>