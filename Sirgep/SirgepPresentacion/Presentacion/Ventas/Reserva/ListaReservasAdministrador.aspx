<%@ Page Title="Municipalidad > Reservas" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="ListaReservasAdministrador.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Ventas.Reserva.ListaReservasAdministrador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Contenido" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div class="container-reservas">
        <h1 class="titulo-principal">Municipalidad &gt; Reservas</h1>
        <div class="busqueda-filtros">
            <div class="busqueda" style="display: flex; align-items: center; gap: 10px;">
                <input type="text" id="input_busqueda" runat="server" class="input-busqueda"
                    placeholder="🔍 Buscar"
                    oninput="validarLongitudBusqueda(this)"
                    onkeypress="return buscarOnEnter(event)" />
                <span id="mensajeAdvertencia" class="mensaje-advertencia"
                    style="color: red; visibility: hidden; margin-left: 10px;">La búsqueda no puede superar los 50 caracteres.
                </span>
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar"
                    OnClick="btnBuscar_Click" Style="visibility: hidden;" CssClass="btn btn-dark mx-2" />
                <asp:Button ID="btnVerCalendario" runat="server" Text="Ver Calendario Mensual de Reservas"
                    OnClick="btnVerCalendario_Click" CssClass="btn btn-dark mx-2" />
            </div>

            <div class="filtros">
                <label class="filtros-label">Filtros:</label>
                <asp:DropDownList ID="ddlFiltros" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFiltros_SelectedIndexChanged" CssClass="filtro-select">
                    <asp:ListItem Text="Fecha" Value="fecha" />
                    <asp:ListItem Text="Nombre del distrito" Value="distrito" />
                </asp:DropDownList>
                <asp:DropDownList ID="ddlDistritos" runat="server" CssClass="filtro-select" />
                <asp:TextBox ID="txtFecha" runat="server" CssClass="form-control filtro-fecha" TextMode="Date" />
                <div class="filtro-activos-checkbox">
                    <input type="text" class="input-filtro" value="Filtro por Activos" readonly />
                    <asp:CheckBox ID="chkActivos" runat="server" CssClass="checkbox-filtro" />
                </div>
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-dark mx-2" OnClick="btnFiltrar_Click" />
                <asp:ImageButton ID="btnLimpiarFiltro" runat="server" ImageUrl="/Images/img/trash-solid.svg" CssClass="image-button" OnClick="btnLimpiarFiltro_Click" Visible="false" ToolTip="Limpiar filtros y mostrar todas las reservas" />
            </div>
        </div>

        <asp:GridView ID="gvReservas" runat="server" AutoGenerateColumns="False" CssClass="tabla-reservas" GridLines="None" OnRowDataBound="gvReservas_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="Abrir">
                    <ItemTemplate>
                        <a href='<%# "ConstanciaReserva.aspx?idConstancia=" + Eval("idConstancia") %>'>
                            <img src="/Images/icons/open-link.png" alt="Abrir" class="icono-abrir" />
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="#Reserva">
                    <ItemTemplate>
                        <%# "#" + Convert.ToInt32(Eval("numReserva")).ToString("D3") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="FechaConstancia" HeaderText="Fecha de Constancia" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Distrito" HeaderText="Distrito" />
                <asp:BoundField DataField="Espacio" HeaderText="Espacio" />
                <asp:BoundField DataField="Correo" HeaderText="Correo del usuario" />

                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:Button ID="btnEliminarReserva" runat="server" Text="Eliminar" CssClass="btn btn-danger btn-sm"
                            OnClick="btnEliminarReserva_Click"
                            CommandArgument='<%# Eval("numReserva") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="¿Activo?">
                    <ItemTemplate>
                        <%# Eval("Activo").ToString() == "65" ? "Sí" : "No" %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- Cambio de paginado -->
        <asp:Panel ID="pnlPaginacion" runat="server" CssClass="d-flex justify-content-center my-3">
            <asp:Button ID="btnAnterior" runat="server" Text="Anterior" CssClass="btn btn-outline-dark mx-1" OnClick="btnAnterior_Click" />
            <asp:Label ID="lblPagina" runat="server" CssClass="align-self-center mx-2" />
            <asp:Button ID="btnSiguiente" runat="server" Text="Siguiente" CssClass="btn btn-outline-dark mx-1" OnClick="btnSiguiente_Click" />
        </asp:Panel>
    </div>

    <!-- JavaScript -->
    <script type="text/javascript">
        function validarLongitudBusqueda(input) {
            var mensaje = document.getElementById('mensajeAdvertencia');
            if (input.value.length > 50) {
                mensaje.style.visibility = 'visible';
            } else {
                mensaje.style.visibility = 'hidden';
            }
        }
    </script>
</asp:Content>