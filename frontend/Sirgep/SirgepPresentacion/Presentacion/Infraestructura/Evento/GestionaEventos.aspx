<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionaEventos.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Infraestructura.Evento.GestionaEventos" MasterPageFile="~/MainLayout.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Municipalidad &gt; Eventos
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <!-- Título principal -->
    <h2 class="fw-bold mb-4" >Municipalidad &gt; Eventos</h2>

    <!-- Búsqueda -->
    <div class="row mb-3">
        <div class="col-12 col-md-12">
            <div class="input-group">
                <asp:TextBox OnTextChanged="txtBusqueda_TextChanged" ID="txtBusqueda" runat="server" CssClass="input-busqueda" Placeholder="🔍 Buscar" AutoPostBack="true"/>
            </div>
        </div>
    </div>

    <!-- Filtros -->
    <div class="mb-4">
        <label class="fw-bold d-block" style="font-size: 1.2rem;">Filtro por fecha:</label>

        <div class="row g-3 align-items-end">
            <!-- Mensaje de error -->
            <div class="col-12">
                <asp:Label ID="lblError" runat="server" CssClass="text-danger fw-semibold"></asp:Label>
            </div>

            <!-- Fechas y botón en la misma fila -->
            <div class="col-12 d-flex align-items-center justify-content-between flex-wrap gap-3">

                <!-- Fechas -->
                <div class="d-flex align-items-center flex-wrap gap-3" id="filtrosFechas">
                    <label for="txtFechaInicioFiltro" class="mb-0">Inicio:</label>
                    <asp:TextBox ID="txtFechaInicioFiltro" runat="server"
                        CssClass="form-control form-control-sm mb-0 filtro-fecha"
                        TextMode="Date" Style="width: 150px;" />

                    <label for="txtFechaFinFiltro" class="mb-0">Fin:</label>
                    <asp:TextBox ID="txtFechaFinFiltro" runat="server"
                        CssClass="form-control form-control-sm mb-0 filtro-fecha"
                        TextMode="Date" Style="width: 150px;" />
                </div>

                <!-- Botones: Consultar + Añadir -->
                <div class="d-flex gap-2 flex-wrap">
                    <asp:Button ID="btnConsultar" runat="server" Text="Consultar"
                        CssClass="btn btn-dark px-4 fw-semibold fst-italic"
                        OnClick="btnConsultar_Click" />

                    <asp:Button ID="btnMostrarModalAgregarEvento" runat="server"
                        CssClass="btn btn-success fw-bold fst-italic px-4"
                        Text="Añadir Evento" OnClick="btnMostrarModalAgregarEvento_Click" />
                </div>
            </div>
        </div>
    </div>


    <!-- Tabla -->
    <div class="table-responsive">
        <table class="table table-bordered text-center align-middle tabla-reservas" style="background: #f7f7f7;">
            <thead class="table-light fw-bold">
                <tr>
                    <th>Imagen</th>
                    <th>Código</th>
                    <th>Nombre</th>
                    <th>Disponibles</th>
                    <th>Vendidas</th>
                    <th>Precio S/</th>
                    <th>Inicio</th>
                    <th>Fin</th>
                    <th>Acciones</th>
                    <th>¿Activo?</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptEventos" runat="server" OnItemDataBound="rptEventos_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:LinkButton
                                    ID="lnkVerImagen"
                                    runat="server"
                                    CommandArgument='<%# Eval("archivoImagen") %>'
                                    OnCommand="lnkVerImagen_Command">
                                        <i class="bi bi-box-arrow-up-right text-dark"></i>
                                </asp:LinkButton>

                            </td>
                            <td class="columna-peq"><%# Eval("IdEvento") %></td>
                            <td><%# Eval("Nombre") %></td>
                            <td class="columna-peq"><%# Eval("cantEntradasDispo") %></td>
                            <td class="columna-peq"><%# Eval("cantEntradasVendidas") %></td>
                            <td class="columna-peq"><%# Eval("precioEntrada") %></td>
                            <td><%# Eval("fecha_inicio").ToString().Split('T')[0] %></td>
                            <td><%# Eval("fecha_fin").ToString().Split('T')[0] %></td>
                            <td>
                                <asp:Button ID="btnEditar" runat="server" CssClass="btn btn-warning btn-sm fw-bold me-2" Text="Editar" CommandArgument='<%# Eval("IdEvento") %>' OnClick="btnEditar_Click" />
                                <asp:Button ID="btnEliminar" runat="server" CssClass="btn btn-danger btn-sm fw-bold"
                                    Text="Eliminar" CommandArgument='<%# Eval("IdEvento") %>' OnClick="btnEliminar_Click" />
                            </td>
                            <td><%# Eval("Activo").ToString() == "65" ? "Sí" : "No" %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td colspan="10" class="text-center pt-3">

                                <asp:Button ID="btnAnteriorFootEvento" runat="server" CssClass="btn btn-outline-secondary btn-sm me-4"
                                    Text="Anterior" CommandName="Anterior" OnCommand="Paginar_Click" />

                                <asp:Label ID="lblPaginaFootEvento" runat="server" CssClass="fw-bold"></asp:Label>

                                <asp:Button ID="btnSiguienteFootEvento" runat="server" CssClass="btn btn-outline-secondary btn-sm ms-4"
                                    Text="Siguiente" CommandName="Siguiente" OnCommand="Paginar_Click" />
                            </td>
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>

    <!-- ------------------------------MODAL AGREGAR EVENTO------------------------------ -->
        <div class="modal fade" id="modalAgregarEvento" tabindex="-1" aria-labelledby="modalAgregarEventoLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header" style="background-color: #f10909">
                        <h2 class="modal-title" id="modalAgregarEventoLabel" style="color: white">Añadir - Evento</h2>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                    </div>

                    <div class="modal-body">
                        <!-- Campos del evento -->
                        <div class="row g-3">
                            <!-- Nombre del evento -->
                            <div class="col-md-6">
                                <label class="form-label fw-semibold">Nombre</label>
                                <asp:TextBox ID="txtNomEvent" runat="server" Placeholder="Ingrese el nombre" CssClass="form-control"></asp:TextBox>
                            </div>

                            <!-- DROP DOWN LIST -- FUNCIONES -- -->
                            <div class="col-md-6">
                                <label class="form-label fw-semibold">Ver Funciones</label>
                                <asp:DropDownList ID="ddlFuncionesAgregar" runat="server" CssClass="form-select" onchange="this.selectedIndex = this.dataset.lastSelectedIndex || 0" onfocus="this.dataset.lastSelectedIndex = this.selectedIndex;">
                                    <asp:ListItem Text="Presione para ver las funciones agregadas" Value="" Disabled="true" Selected="True" />
                                </asp:DropDownList>
                            </div>

                            <asp:TextBox ID="txtFechaInicioEvento" runat="server" CssClass="d-none" />
                            <asp:TextBox ID="txtFechaFinEvento" runat="server" CssClass="d-none" />

                        </div>

                        <div class="row g-3 my-2">

                            <!-- Descripción -->
                            <div class="col-6">
                                <label class="form-label fw-semibold">Descripción</label>
                                <asp:TextBox ID="txtDescAgregar" runat="server" Placeholder="Ingrese una descripción" CssClass="form-control"></asp:TextBox>
                            </div>

                            <!-- Departamento -->
                            <div class="col-6">
                                <label class="form-label fw-semibold">Departamento</label>
                                <asp:DropDownList ID="ddlDepaAgregar" runat="server" CssClass="form-select" OnSelectedIndexChanged="ddlDepaAgregar_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="Seleccione un departamento" Value="" Disabled="true" Selected="True" />
                                </asp:DropDownList>
                            </div>

                        </div>

                        <div class="row g-3 my-2">
                            <!-- Ubicación -->
                            <div class="col-6">
                                <lsabel class="form-label fw-semibold">Ubicación</lsabel>
                                <asp:TextBox ID="txtUbicacionAgregar" runat="server" Placeholder="Ingrese la ubicación del evento" CssClass="form-control"></asp:TextBox>
                            </div>

                            <!-- Provincia -->
                            <div class="col-6">
                                <lsabel class="form-label fw-semibold">Provincia</lsabel>
                                <asp:DropDownList ID="ddlProvAgregar" runat="server" CssClass="form-select" OnSelectedIndexChanged="ddlProvAgregar_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="Seleccione una provincia" Value="" Disabled="true" Selected="True" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="row g-3 my-2">
                            <!-- Referencia -->
                            <div class="col-6">
                                <label class="form-label fw-semibold">Referencia</label>
                                <asp:TextBox ID="txtReferencia" runat="server" Placeholder="Inserte una referencia del lugar" CssClass="form-control"></asp:TextBox>
                            </div>

                            <!-- Distrito -->
                            <div class="col-6">
                                <lsabel class="form-label fw-semibold">Distrito</lsabel>
                                <asp:DropDownList ID="ddlDistAgregar" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Seleccione un distrito" Value="" Disabled="true" Selected="True" />
                                </asp:DropDownList>
                            </div>

                        </div>

                        <div class="row g-3 my-2">

                            <!-- Precio Entrada -->
                            <div class="col-4">
                                <label class="form-label fw-semibold">Precio Entrada</label>
                                <asp:TextBox TextMode="Number" ID="txtPrecioEntrada" runat="server" Placeholder="(S/.)" CssClass="form-control"></asp:TextBox>
                            </div>
                            <!-- Disponibles -->
                            <div class="col-4">
                                <label class="form-label fw-semibold">Entradas Disponibles</label>
                                <asp:TextBox TextMode="Number" ID="txtDisponibles" runat="server" Placeholder="# disponibles" CssClass="form-control"></asp:TextBox>
                            </div>
                            <!-- Vendidas -->
                            <div class="col-4">
                                <label class="form-label fw-semibold">Entradas Vendidas</label>
                                <asp:TextBox TextMode="Number" ID="txtVendidas" runat="server" Placeholder="# ventas" CssClass="form-control"></asp:TextBox>
                            </div>

                        </div>

                        <hr class="my-4" />

                    <!-- Lista de funciones -->
                    <h6 class="fw-bold mb-3">Funciones del Evento:</h6>
                    <!-- Añadir función -->
                    <div class="row g-3 align-items-end">
                        <div class="col-md-4">
                            <label class="form-label">Fecha:</label>
                            <asp:TextBox ID="txtFechaFuncion" runat="server" CssClass="form-control" TextMode="Date"/>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Hora Inicio:</label>
                            <asp:TextBox ID="txtHoraIniFuncion" runat="server" CssClass="form-control" TextMode="Time"/>
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Hora Fin:</label>
                            <asp:TextBox ID="txtHoraFinFuncion" runat="server" CssClass="form-control" TextMode="Time"/>
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnAgregarFuncion" runat="server" Text="Añadir" CssClass="btn btn-success" OnClick="btnAgregarFuncion_Click" />
                        </div>
                        <asp:Label ID="lblErrorAgregar" runat="server" CssClass="text-danger fw-semibold"></asp:Label>
                    </div>
                        <!-- Foto -->
                    <div class="row g-3 my-2">
                        <!-- Subir imagen -->
                        <div class="col-md-6">
                            <label class="form-label fw-semibold">Subir foto</label>
                            <asp:FileUpload ID="fuAgregar" runat="server" CssClass="form-control" onchange="mostrarPreview(this, 'imgPreviewAgregar')" />
                        </div>

                        <!-- Vista previa -->
                        <div class="col-md-6 text-center d-flex align-items-end">
                            <img id="imgPreviewAgregar" src="#" alt="Vista previa" style="display: none; max-height: 180px;" class="img-thumbnail w-100" />
                        </div>
                    </div>
                </div>


                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnAgregar" CssClass="btn btn-secondary" runat="server" Text="Aceptar" OnClick="btnAgregar_Click"/>
                </div>
            </div>
        </div>
    </div>
    <!-- ------------------------------FIN MODAL AGREGAR EVENTO------------------------------ -->

    <!-- MODAL DE ELIMINAR EVENTO -- -->

    <asp:HiddenField ID="hdnIdEvento" runat="server" />

    <div runat="server" class="modal fade" id="modalConfirmacionEliminado" tabindex="-1" aria-labelledby="modalConfirmacionLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header modal-header-rojo text-white">
                    <h5 class="modal-title" id="modalConfirmacionLabel">Ventana de confirmación</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <div class="modal-body d-flex align-items-center modal-body-confirmacion">
                    <div class="icono-confirmacion me-3">
                        <div class="icono-circulo">
                            <i class="bi bi-info-lg fs-1"></i>
                        </div>
                    </div>
                    <div id="modalConfirmacionBody" class="fs-5">
                        ¿Está seguro que desea eliminar este registro?
       
                    </div>
                </div>

                <div class="modal-footer justify-content-center">
                    <button type="button" class="btn btn-dark px-4" data-bs-dismiss="modal"><em>No</em></button>
                    <asp:Button ID="btnConfirmarEliminado" runat="server" Text="Sí" OnClick="btnConfirmarEliminado_Click"
                        CssClass="btn btn-danger"/>
                </div>

            </div>
        </div>
    </div>

    <!-- MODAL DE EDITAR EVENTO -- -->

    <!-- tengo un hdnIDevento para editarlo por si acaso -->

    <!-- ------------------------------MODAL EDITAR EVENTO------------------------------ -->
    <div class="modal fade" id="modalEditarEvento" tabindex="-1" aria-labelledby="modalEditarEventoLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header" style="background-color: #f10909">
                    <h2 class="modal-title" id="modalEditarEventoLabel" style="color: white">Editar - Evento</h2>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <div class="modal-body">
                    <!-- Campos del evento -->
                    <div class="row g-3">
                        <!-- Nombre del evento -->
                        <div class="col-md-6">
                            <label class="form-label fw-semibold">Nombre</label>
                            <asp:TextBox ID="txtNombreEditar" runat="server" Placeholder="Ingrese el nombre" CssClass="form-control"></asp:TextBox>
                        </div>
                        
                        <!-- DROP DOWN LIST -- FUNCIONES -- NO SE IMPLEMENTARÁ SU EDICIÓN - -->
                        <div class="col-md-6">
                            <label class="form-label fw-semibold">Ver Funciones</label>
                            <asp:DropDownList ID="ddlFuncEditar" runat="server" CssClass="form-select" onchange="this.selectedIndex = this.dataset.lastSelectedIndex || 0" onfocus="this.dataset.lastSelectedIndex = this.selectedIndex;">
                                <asp:ListItem Text="Presione para ver las funciones agregadas" Value="" Disabled="true" Selected="True" />
                            </asp:DropDownList>
                        </div>
                        
                    </div>

                    <div class="row g-3 my-2">

                        <!-- Descripción -->
                        <div class="col-6">
                            <label class="form-label fw-semibold">Descripción</label>
                            <asp:TextBox ID="txtDescEditar" runat="server" Placeholder="Ingrese una descripción" CssClass="form-control"></asp:TextBox>
                        </div>

                        <!-- Departamento -->
                        <div class="col-6">
                            <label class="form-label fw-semibold">Departamento</label>

                            <div class="d-flex align-items-center gap-2">
                                <asp:DropDownList ID="ddlDepasEditar" runat="server" CssClass="form-select flex-grow-1"
                                    OnSelectedIndexChanged="ddlDepasEditar_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="Seleccione un departamento" Value="" Disabled="true" Selected="True" />
                                </asp:DropDownList>

                                <asp:LinkButton ID="btnEditUbigeo" runat="server" CssClass="btn btn-success" ToolTip="Presione para modificar el ubigeo." OnClick="btnEditUbigeo_Click">
                                    <i class="bi bi-pencil-fill"></i>
                                </asp:LinkButton>
                            </div>
                        </div>


                    </div>

                    <div class="row g-3 my-2">
                        <!-- Ubicación -->
                        <div class="col-6">
                            <lsabel class="form-label fw-semibold">Ubicación</lsabel>
                            <asp:TextBox ID="txtUbiEditar" runat="server" Placeholder="Ingrese la ubicación del evento" CssClass="form-control"></asp:TextBox>
                        </div>

                        <!-- Provincia -->
                        <div class="col-6">
                            <lsabel class="form-label fw-semibold">Provincia</lsabel>
                            <asp:DropDownList ID="ddlProvEditar" runat="server" CssClass="form-select" OnSelectedIndexChanged="ddlProvEditar_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Text="Seleccione una provincia" Value="" Disabled="true" Selected="True" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row g-3 my-2">
                        <!-- Referencia -->
                        <div class="col-6">
                            <label class="form-label fw-semibold">Referencia</label>
                            <asp:TextBox ID="txtRefEditar" runat="server" Placeholder="Inserte una referencia del lugar" CssClass="form-control"></asp:TextBox>
                        </div>

                        <!-- Distrito -->
                        <div class="col-6">
                            <lsabel class="form-label fw-semibold">Distrito</lsabel>
                            <asp:DropDownList ID="ddlDistEditar" runat="server" CssClass="form-select" OnSelectedIndexChanged="ddlDistEditar_SelectedIndexChanged">
                                <asp:ListItem Text="Seleccione un distrito" Value="" Disabled="true" Selected="True" />
                            </asp:DropDownList>
                        </div>

                    </div>

                    <div class="row g-3 my-2">

                        <!-- Precio Entrada -->
                        <div class="col-4">
                            <label class="form-label fw-semibold">Precio Entrada</label>
                            <asp:TextBox ID="txtPrecioEditar" TextMode="Number" runat="server" Placeholder="(S/.)" CssClass="form-control"></asp:TextBox>
                        </div>
                        <!-- Disponibles -->
                        <div class="col-4">
                            <label class="form-label fw-semibold">Entradas Disponibles</label>
                            <asp:TextBox ID="txtDispoEditar" TextMode="Number" runat="server" Placeholder="# disponibles" CssClass="form-control"></asp:TextBox>
                        </div>
                        <!-- Vendidas -->
                        <div class="col-4">
                            <label class="form-label fw-semibold">Entradas Vendidas</label>
                            <asp:TextBox ID="txtVendEditar" TextMode="Number" runat="server" Placeholder="# ventas" CssClass="form-control"></asp:TextBox>
                        </div>

                    </div>

                    <hr class="my-4" />

                    <!-- Lista de funciones -->
                    <h6 class="fw-bold mb-3">Funciones del Evento:</h6>
                    <!-- Añadir función -->
                    <div class="row g-3 align-items-end">
                        <div class="col-md-4">
                            <label class="form-label">Fecha:</label>
                            <asp:TextBox ID="txtFechaEditar" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Hora Inicio:</label>
                            <asp:TextBox ID="txtHoraIniEditar" runat="server" CssClass="form-control" TextMode="Time" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Hora Fin:</label>
                            <asp:TextBox ID="txtHoraFinEditar" runat="server" CssClass="form-control" TextMode="Time" />
                        </div>
                        <div class="col-md-2">
                            <asp:Button ID="btnAgregarFuncionEditar" runat="server" Text="Añadir" CssClass="btn btn-success" OnClick="btnAgregarFuncionEditar_Click" />
                        </div>
                    </div>

                    <div class="col-md-6 mb-4">
                        <asp:Label ID="lblErrorEditar" runat="server" CssClass="text-danger fw-semibold"></asp:Label>
                    </div>

                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnAceptarEditar" CssClass="btn btn-success" runat="server" Text="Aceptar" OnClick="btnAceptarEditar_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- fin MODAL EDITAR EVENTO -- -->

    <!-- Modal Ver Foto -->
    <div class="modal fade" id="modalPreview" tabindex="-1" aria-labelledby="modalPreviewLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-lg">
            <div class="modal-content">
                <!-- Encabezado completamente rojo -->
                <div class="modal-header" style="background-color: #f10909; color: white;">
                    <h5 class="modal-title" id="modalPreviewLabel">Vista previa de imagen</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>
                <!-- Cuerpo del modal -->
                <div class="modal-body text-center">
                    <img id="imgPreviewModal" src="#" style="max-width: 100%;" class="img-fluid" />
                </div>
            </div>
        </div>
    </div>



    <!-- SCRIPTS -->

    <script type="text/javascript">
        function mostrarPreview(input, idImg) {
            const file = input.files[0];
            const img = document.getElementById(idImg);

            if (file) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    img.src = e.target.result;
                    img.style.display = 'block';
                };
                reader.readAsDataURL(file);
            } else {
                img.src = '#';
                img.style.display = 'none';
            }
        }
        function toggleFechas() {
            var check = document.getElementById('chkFiltroFechas');
            var filtros = document.getElementById('filtrosFechas');
            filtros.style.display = check.checked ? 'block' : 'none';
        }
        function agregarFuncion() {
            const fecha = document.getElementById('fechaFuncion').value;
            const horaInicio = document.getElementById('horaInicioFuncion').value;
            const horaFin = document.getElementById('horaFinFuncion').value;

            // Validar campos vacíos
            if (!fecha || !horaInicio || !horaFin) {
                alert("Por favor complete todos los campos de la función.");
                return;
            }

            const hoy = new Date();
            const fechaSeleccionada = new Date(fecha + "T00:00");

            // Validar año actual
            if (fechaSeleccionada.getFullYear() !== hoy.getFullYear()) {
                alert("La fecha debe estar en el año actual.");
                return;
            }

            // Validar que no sea fecha pasada
            const hoySinHora = new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate());
            if (fechaSeleccionada < hoySinHora) {
                alert("La fecha no puede ser anterior a hoy.");
                return;
            }

            // Validar hora inicio < hora fin
            const horaInicioDate = new Date(`1970-01-01T${horaInicio}`);
            const horaFinDate = new Date(`1970-01-01T${horaFin}`);
            if (horaInicioDate >= horaFinDate) {
                alert("La hora de Inicio debe ser menor a la hora de Fin.");
                return;
            }

            // Verificar duplicados en el DropDownList de ASP.NET
            const ddl = document.getElementById('<%= ddlFuncionesAgregar.ClientID %>');
            const valorFuncion = `${fecha}_${horaInicio}_${horaFin}`;

            for (let i = 0; i < ddl.options.length; i++) {
                if (ddl.options[i].value === valorFuncion) {
                    alert("Esa función ya ha sido agregada.");
                    return;
                }
            }

            // ACTUALIZAR FECHAS MÍNIMA Y MÁXIMA
            if (fechaMinima === null || new Date(fecha) < new Date(fechaMinima)) {
                fechaMinima = fecha;
            }
            if (fechaMaxima === null || new Date(fecha) > new Date(fechaMaxima)) {
                fechaMaxima = fecha;
            }
            // los ponemos en mi txts ocultos
            document.getElementById('<%= txtFechaInicioEvento.ClientID %>').value = fechaMinima;
            document.getElementById('<%= txtFechaFinEvento.ClientID %>').value = fechaMaxima;

            // agregar al Drop Down List 
            const option = document.createElement("option");
            option.value = `${fecha}_${horaInicio}_${horaFin}`;
            option.textContent = `${fecha} - ${horaInicio} a ${horaFin}`;
            ddl.appendChild(option);

            // Limpiar inputs
            document.getElementById('fechaFuncion').value = '';
            document.getElementById('horaInicioFuncion').value = '';
            document.getElementById('horaFinFuncion').value = '';
        }
        function modalConfirmacionEliminado(id) {
            // Mostrar el modal (Bootstrap 5)
            var modal = new bootstrap.Modal(document.getElementById('modalConfirmacionEliminado'));
            modal.show();
        }
    </script>
</asp:Content>
