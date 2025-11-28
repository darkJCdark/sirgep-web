<%@ Page Title="" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="FormularioEspacio.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Ventas.Reserva.FormularioEspacio" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Reservar un espacio
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <h2 class="fw-bold mb-4">Reserva un espacio</h2>
    <p class="mb-3">Llena los datos del espacio que quieres reservar. Tenga en cuenta que se considerará su reserva hasta el final de la última hora seleccionada.
        Si elige reservar de 10:00 a 12:00, se considerará que la reserva es hasta las 12:59. Si desea reservar de 10:00 a 11:00, debe seleccionar únicamente la hora 10:00.
    </p>

    <!-- Departamento / Provincia / Distrito -->
    <div class="row mb-3">
        <div class="col-md-4">
            <asp:DropDownList ID="ddlDepartamento" runat="server" CssClass="form-select" 
                AutoPostBack="true" OnSelectedIndexChanged="ddlDepartamento_SelectedIndexChanged" />
        </div>
        <div class="col-md-4">
            <asp:DropDownList ID="ddlProvincia" runat="server" CssClass="form-select" 
                AutoPostBack="true" OnSelectedIndexChanged="ddlProvincia_SelectedIndexChanged" />
        </div>
        <div class="col-md-4">
            <asp:DropDownList ID="ddlDistrito" runat="server" CssClass="form-select" 
                AutoPostBack="true" OnSelectedIndexChanged="ddlDistrito_SelectedIndexChanged" />
        </div>
    </div>

    <!-- Categoría / Espacio / Fecha -->
    <div class="row mb-3">
        <div class="col-md-4">
            <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select"
                AutoPostBack="true" OnSelectedIndexChanged="ddlCategoria_SelectedIndexChanged" />
        </div>
        <div class="col-md-4">
            <asp:DropDownList ID="ddlEspacio" runat="server" CssClass="form-select"
                AutoPostBack="true" OnSelectedIndexChanged="ddlEspacio_SelectedIndexChanged" />
        </div>
        <div class="col-md-4 d-flex align-items-center">
            <label class="form-label me-2">Fecha:</label>
            <asp:TextBox ID="txtFecha" runat="server" CssClass="form-control" TextMode="Date" />
        </div>
    </div>

    <!-- Consultar horarios -->
    <asp:Button ID="btnConsultarHorarios" runat="server" CssClass="btn btn-dark mb-3"
        Text="Consultar Horarios" OnClick="btnConsultarHorarios_Click" />

    <!-- Horarios disponibles -->
    <div class="border rounded p-3">
        <asp:Repeater ID="rptHorarios" runat="server" ClientIDMode="Static">
            <ItemTemplate>
                 <div class="form-check form-check-inline">
                    <input type="checkbox"
                    class="form-check-input horario-check"
                    data-disponible='<%# Eval("disponible", "{0}") %>'
                    data-hora='<%# Eval("horaIni", "{0:HH:mm}") %>'
                    <%# !(bool)Eval("disponible") ? "disabled" : "" %> />

                    <label class="form-check-label">
                        <%# Eval("horaIni", "{0:HH:mm}") %>
                    </label>
                </div>
            </ItemTemplate>

        </asp:Repeater>
    </div>

    <asp:Label ID="lblPrecioHora" runat="server" CssClass="d-block mt-3 fw-bold" />
    <asp:HiddenField ID="hdnPrecioHora" runat="server" />
    
    <asp:Label ID="lblPrecioTotal" runat="server" CssClass="d-block fw-bold" />
    <asp:Label ID="lblError" runat="server" CssClass="text-danger fw-bold" Style="min-height:1.5em; display:block;" />

    <asp:Button ID="btnReservar" runat="server" Text="Reservar" CssClass="btn btn-primary mt-3"
        OnClick="btnReservar_Click" />
    <asp:HiddenField ID="hdnHoraInicioSeleccionada" runat="server" />
    <asp:HiddenField ID="hdnHoraFinSeleccionada" runat="server" />
    <!-- JavaScript -->
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const checks = Array.from(document.querySelectorAll('.horario-check'));
            const precioPorHora = parseFloat(document.getElementById('<%= hdnPrecioHora.ClientID %>')?.value?.replace(",", ".") || "0");;
            const lblPrecioTotal = document.getElementById('<%= lblPrecioTotal.ClientID %>');
            const hdnHoraInicioSeleccionada = document.getElementById('<%= hdnHoraInicioSeleccionada.ClientID %>');
            const hdnHoraFinSeleccionada = document.getElementById('<%= hdnHoraFinSeleccionada.ClientID %>');
            function actualizarEstado() {
                const seleccionados = checks.map((chk, idx) => chk.checked ? idx : -1).filter(idx => idx !== -1);
                const horasSeleccionadas = checks
                    .filter(chk => chk.checked)
                    .map(chk => chk.getAttribute('data-hora'))
                    .sort(); // HH:mm es lexicográficamente ordenable

                // Guardar en hiddenFields
                if (horasSeleccionadas.length > 0) {
                    hdnHoraInicioSeleccionada.value = horasSeleccionadas[0];
                    hdnHoraFinSeleccionada.value = horasSeleccionadas[horasSeleccionadas.length - 1];
                } else {
                    hdnHoraInicioSeleccionada.value = "";
                    hdnHoraFinSeleccionada.value = "";
                }


                // Habilitar todo inicialmente
                checks.forEach(chk => {
                    if (!chk.disabled || chk.getAttribute("data-disponible") === "true") {
                        chk.disabled = false;
                    }
                });

                if (seleccionados.length > 0) {
                    const min = Math.min(...seleccionados);
                    const max = Math.max(...seleccionados);

                    checks.forEach((chk, idx) => {
                        const disponible = chk.getAttribute("data-disponible") === "true";

                        // Solo habilita los inmediatamente adyacentes
                        if (
                            idx !== min - 1 && idx !== max + 1 &&
                            !chk.checked
                        ) {
                            chk.disabled = true;
                        }

                        // Evitar reactivar los que no son disponibles
                        if (!disponible) {
                            chk.disabled = true;
                        }
                    });
                }

                // Actualizar precio
                const cantidadSeleccionada = seleccionados.length;
                const total = cantidadSeleccionada * precioPorHora;
                lblPrecioTotal.textContent = `Precio total seleccionado: S/ ${total.toFixed(2)}`;
            }

            // Marcar cuáles están disponibles para recuperación posterior
            checks.forEach((chk) => {
                chk.setAttribute("data-disponible", !chk.disabled);
            });

        // Añadir evento a cada checkbox
        checks.forEach(chk => {
            chk.addEventListener('change', actualizarEstado);
        });

        // Inicializa el estado si viene con preselección
        actualizarEstado();
    });
    </script>
</asp:Content>