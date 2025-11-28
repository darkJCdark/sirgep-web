<%@ Page Title="" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="EligeDistrito.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Ubicacion.Distrito.EligeDistrito" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ¿De donde Buscamos?
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Encabezado" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <div class="container">
        <h2 class ="mb-3">¿De donde buscamos?</h2>
        <p class ="text-muted mb-4">Elige el departamento, provincia y distrito del cual buscas eventos.</p>

        <div class="row mb-3 align-items-end">
            <div class="col-md-4">
                <label for="ddlDepartamento" class="form-label">Departamento</label>
                <asp:DropDownList ID="ddlDepartamento" runat="server" CssClass="form-select mb-3" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartamento_SelectedIndexChanged">
                    <asp:ListItem Text="Seleccione un departamento" Value="" />
                </asp:DropDownList>
            </div>

            <!-- ddl de provincia dependiente de la selcción depa -->
            <div class="col-md-4">
                <label for="ddlProvincia" class="form-label">Provincia</label>
                <asp:DropDownList ID="ddlProvincia" runat="server" CssClass="form-select mb-3" AutoPostBack="true" OnSelectedIndexChanged="ddlProvincia_SelectedIndexChanged">
                    <asp:ListItem Text="Seleccione una provincia" Value="" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label for="ddlDistrito" class="form-label">Distrito</label>
                <asp:DropDownList ID="ddlDistrito" runat="server" CssClass="form-select mb-3">
                    <asp:ListItem Text="Seleccione un distrito" Value="" />
                </asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                <asp:Label ID="lblError" runat="server" CssClass="text-danger fw-bold" Visible="false"></asp:Label>
            </div>
        </div>
        <div class="text-end">
                <asp:Button ID="btnAceptar" runat="server" Text="Buscar Eventos" CssClass="btn btn-primary mt-3" OnClick="btnAceptar_Click" />
            </div>
        </div>
</asp:Content>
