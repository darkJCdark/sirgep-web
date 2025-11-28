<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaEspacios.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Infraestructura.Espacio.ListaEspacios" MasterPageFile="~/MainLayout.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Municipalidad &gt; Espacios
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <asp:HiddenField ID="hdnIdAEliminar" runat="server" />
    <!-- Título principal -->
    <h2 class="fw-bold mb-4" >Municipalidad &gt; Espacios</h2>
    <!-- Búsqueda -->
    <div class="mb-3">
        <asp:TextBox OnTextChanged="txtBusqueda_TextChanged" ID="txtBusqueda" runat="server" CssClass="input-busqueda" Placeholder="🔍 Buscar" AutoPostBack="true"/>
    </div>

    <!-- Filtros -->
    <div class="mb-4">
        <div class="d-flex justify-content-between align-items-center flex-wrap gap-2">
            <!-- Etiqueta "Filtros:" y dropdowns -->
            <div class="d-flex align-items-center gap-2 flex-wrap">
                <label class="fw-bold mb-0">Filtros:</label>

                <asp:DropDownList ID="ddlCategoria" runat="server"
                    CssClass="filtro-fecha-espacio">
                    <asp:ListItem Text="Seleccione una categoría" Value="" />
                    <asp:ListItem Text="Teatros" Value="TEATRO" />
                    <asp:ListItem Text="Canchas" Value="CANCHA" />
                    <asp:ListItem Text="Salones" Value="SALON" />
                    <asp:ListItem Text="Parques" Value="PARQUE" />
                </asp:DropDownList>

                <asp:DropDownList ID="ddlDistrito" runat="server"
                    CssClass="filtro-fecha-espacio">
                    <asp:ListItem Text="Seleccione un distrito" Value="" />
                </asp:DropDownList>
            </div>

            <!-- Botones alineados a la derecha -->
            <div class="d-flex gap-2">
                <asp:Button ID="btnConsultar" runat="server"
                    Text="Consultar" CssClass="btn btn-dark fw-semibold" OnClick="btnConsultar_Click" />

                <asp:Button ID="btnAgregarEspacio" runat="server"
                    Text="Añadir Espacio" CssClass="btn btn-success fw-semibold fst-italic"
                    OnClick="btnAgregarEspacio_Click" />
            </div>
        </div>
    </div>



    <!-- Tabla -->
    <div class="table-responsive">
        <table class="tabla-reservas">
            <thead class="table-light fw-bold">
                <tr>
                    <th>Imagen</th>
                    <th>Código</th>
                    <th>Categoría</th>
                    <th>Nombre del Espacio</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptEspacios" runat="server">
                    <ItemTemplate>
                        
                        <tr>
                            <td>
                                <asp:LinkButton
                                    ID="lnkVerImagen"
                                    runat="server"
                                    CommandArgument='<%# Eval("foto") %>'
                                    OnCommand="lnkVerImagen_Command">
                                    <i class="bi bi-box-arrow-up-right text-dark"></i>
                                </asp:LinkButton>
                            </td>
                            <td><%# Eval("IdEspacio") %></td>
                            <td><%# Eval("TipoEspacio") %></td>
                            <td><%# Eval("Nombre") %></td>
                            <td>
                                <asp:Button ID="btnEditar" runat="server" CssClass="btn btn-warning btn-sm fw-bold me-2" Text="Editar" CommandArgument='<%# Eval("IdEspacio") %>' OnClick="btnEditar_Click" />
                                <asp:Button ID="btnEliminar" runat="server" CssClass="btn btn-danger btn-sm fw-bold"
                                Text="Eliminar" CommandArgument='<%# Eval("IdEspacio") %>'
                                OnClientClick='<%# $"mostrarConfEspacio({Eval("IdEspacio")}); return false;" %>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td colspan="5" class="text-center pt-3">
                                <asp:Button ID="btnAnteriorFoot" runat="server" CssClass="btn btn-outline-secondary btn-sm me-2"
                                    Text="Anterior" CommandName="Anterior" OnCommand="Paginar_Click" />

                                <asp:Label ID="lblPaginaFoot" runat="server" CssClass="fw-bold"></asp:Label>

                                <asp:Button ID="btnSiguienteFoot" runat="server" CssClass="btn btn-outline-secondary btn-sm ms-2"
                                    Text="Siguiente" CommandName="Siguiente" OnCommand="Paginar_Click" />
                            </td>
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>

    <!-- Modal Paso 1: Datos del Espacio [ AGREGAR UN ESPACIO ] -->
    <div class="modal fade" id="modalPaso1" tabindex="-1" aria-labelledby="modalPaso1Label" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header" style="background-color: #f10909;">
                    <h2 class="modal-title" id="modalPaso1Label" style="color: white">Añadir - Espacio</h2>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <!--Cuerpo del modal [ AGREGAR UN ESPACIO ] -->
                <div class="modal-body">

                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <label>Nombre del espacio</label>
                            <asp:TextBox ID="txtNombreEspacioAgregar" runat="server" CssClass="form-control" Placeholder="Inserte nombre del espacio" />
                        </div>
                        <div class="mb-3 col-md-6">
                            <label>Tipo de espacio</label>
                            <asp:DropDownList ID="ddlTipoEspacioAgregar" runat="server" CssClass="form-select">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <label>Ubicación</label>
                            <asp:TextBox ID="txtUbicacionAgregar" runat="server" CssClass="form-control" Placeholder="Inserte ubicación" />
                        </div>
                        <div class="mb-3 col-md-6">
                            <label>Departamento</label>
                            <asp:DropDownList ID="ddlDepartamentoAgregar" runat="server" CssClass="form-select" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlDepartamentoAgregar_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <label>Superficie (m²)</label>
                            <asp:TextBox ID="txtSuperficieAgregar" runat="server" CssClass="form-control" TextMode="Number" Placeholder="Inserte la superficie" />
                        </div>
                        <div class="mb-3 col-md-6">
                            <label>Provincia</label>
                            <asp:DropDownList ID="ddlProvinciaAgregar" runat="server" CssClass="form-select" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlProvinciaAgregar_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>


                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <label>Precio de reserva por hora</label>
                            <asp:TextBox ID="txtPrecioReservaAgregar" runat="server" CssClass="form-control" TextMode="Number" Placeholder="Inserte el precio de la reserva por hora" />
                        </div>
                        <div class="mb-3 col-md-6">
                            <label>Distrito</label>
                            <asp:DropDownList ID="ddlDistritoAgregar" runat="server" CssClass="form-select" OnSelectedIndexChanged="ddlDistritoAgregar_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                    
                    <!-- Mensaje de error o éxito -->
                    <div class="mb-3">
                        <asp:Label ID="lblMensaje" runat="server" CssClass="text-danger fw-bold" />
                    </div>

                    <hr class="my-2" />

                    <!-- Horarios -->
                    <div>
                        <h6 class="mb-3 fw-semibold">Añadir horario de atención</h6>
                    </div>

                    <div class="row align-items-end">
                        <div class="col-md-3 mb-3">
                            <label for="txtHoraInicioInsert" class="form-label">Hora Inicio</label>
                            <asp:DropDownList
                                ID="ddlHoraInicioInsert"
                                runat="server"
                                CssClass="form-control"
                                OnTextChanged="txtHoraFinInsert_TextChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-3 mb-3">
                            <label for="txtHoraFinInsert" class="form-label">Hora Fin</label>
                            <asp:DropDownList
                                ID="ddlHoraFinInsert"
                                runat="server"
                                CssClass="form-control"
                                OnTextChanged="txtHoraFinInsert_TextChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </div>

                        <div class="col-md-6 mb-3">
                            <asp:Label ID="lblError" runat="server" CssClass="text-danger fw-semibold"></asp:Label>
                        </div>
                    </div>
                    <!-- Fin de Horarios -->

                    <!-- Caja de los Días de Atención -->
                    <asp:ScriptManager ID="ScriptManager1" runat="server" />
                    <asp:UpdatePanel runat="server" ID="updDias">
                        <ContentTemplate>
                            <div class="card p-3 mb-3">
                                <div class="row">
                                    <div class="col-6 col-md-4">
                                        <asp:CheckBox ID="chkLunes" runat="server" CssClass="form-check-input" />
                                        <label for="<%= chkLunes.ClientID %>">Lunes</label>
                                    </div>
                                    <div class="col-6 col-md-4">
                                        <asp:CheckBox ID="chkMartes" runat="server" CssClass="form-check-input" />
                                        <label for="<%= chkMartes.ClientID %>">Martes</label>
                                    </div>
                                    <div class="col-6 col-md-4">
                                        <asp:CheckBox ID="chkMiercoles" runat="server" CssClass="form-check-input" />
                                        <label for="<%= chkMiercoles.ClientID %>">Miércoles</label>
                                    </div>
                                    <div class="col-6 col-md-4">
                                        <asp:CheckBox ID="chkJueves" runat="server" CssClass="form-check-input" />
                                        <label for="<%= chkJueves.ClientID %>">Jueves</label>
                                    </div>
                                    <div class="col-6 col-md-4">
                                        <asp:CheckBox ID="chkViernes" runat="server" CssClass="form-check-input" />
                                        <label for="<%= chkViernes.ClientID %>">Viernes</label>
                                    </div>
                                    <div class="col-6 col-md-4">
                                        <asp:CheckBox ID="chkSabado" runat="server" CssClass="form-check-input" />
                                        <label for="<%= chkSabado.ClientID %>">Sábado</label>
                                    </div>
                                    <div class="col-6 col-md-4">
                                        <asp:CheckBox ID="chkDomingo" runat="server" CssClass="form-check-input" />
                                        <label for="<%= chkDomingo.ClientID %>">Domingo</label>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <hr class="my-4"/>

                    <!-- Foto -->
                    <div class="row g-3">
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

                    <!-- FIN DE Caja de los Días de Atención -->
                <div class="modal-footer">
                    <input type="hidden" id="diasSeleccionados" name="diasSeleccionados" runat="server" />
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button CssClass="btn btn-success" ID="btnGuardarInsertado" runat="server" Text="Guardar" OnClientClick="guardarDiasSeleccionados(); return true;" OnClick="btnGuardarInsertado_Click"/>
                </div>
            </div>
        </div>
    </div>

                
    <!------------------------------------- Modal de Edición de Espacios ---------------------------------------------------------------------------------->

    <!-- Datos del Espacio [ EDICION ESPACIO ] -->
    <div class="modal fade" id="modalEdicionEspacio" tabindex="-1" aria-labelledby="modalEdicionLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header" style="background-color: #f10909;">
                    <h2 class="modal-title" id="modalEdicionLabel" style="color: white">Editar - Espacio</h2>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <!--Cuerpo del modal [ EDICION ESPACIO ] -->
                <div class="modal-body">

                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <label>Nombre del espacio</label>
                            <asp:TextBox ID="txtNombreEdit" runat="server" CssClass="form-control" Placeholder="Inserte nombre del espacio" />
                        </div>
                        <div class="mb-3 col-md-6">
                            <label>Tipo de espacio</label>
                            <asp:DropDownList ID="ddlTipoEspacioEdit" runat="server" CssClass="form-select">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <label>Ubicación</label>
                            <asp:TextBox ID="txtUbicacionEdit" runat="server" CssClass="form-control" Placeholder="Inserte ubicación" />
                        </div>

                        <div class="mb-3 col-md-6">
                            <label>Departamento</label>
                            <div class="d-flex align-items-center">
                                <asp:DropDownList ID="ddlDepartamentoEdit" runat="server" CssClass="form-select me-2 flex-grow-1" AutoPostBack="true"
                                    OnSelectedIndexChanged="ddlDepartamentoEdit_SelectedIndexChanged">
                                </asp:DropDownList>

                                <asp:LinkButton ID="btnEditUbigeo" runat="server" CssClass="btn btn-success"
                                    ToolTip="Presione para modificar el ubigeo." OnClick="btnEditUbigeo_Click">
                                    <i class="bi bi-pencil-fill"></i>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>


                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <label>Superficie (m²)</label>
                            <asp:TextBox ID="txtSuperficieEdit" runat="server" CssClass="form-control" TextMode="Number" Placeholder="Inserte la superficie" />
                        </div>
                        <div class="mb-3 col-md-6">
                            <label>Provincia</label>
                            <asp:DropDownList ID="ddlProvinciaEdit" runat="server" CssClass="form-select" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlProvinciaEdit_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>


                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <label>Precio de reserva por hora</label>
                            <asp:TextBox ID="txtPrecioEdit" runat="server" CssClass="form-control" TextMode="Number" Placeholder="Inserte el precio de la reserva por hora" />
                        </div>
                        <div class="mb-3 col-md-6">
                            <label>Distrito</label>
                            <asp:DropDownList ID="ddlDistritoEdit" runat="server" CssClass="form-select" OnSelectedIndexChanged="ddlDistritoEdit_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- Horarios -->
                    <div>
                        <h6 class="mb-3 fw-semibold">Editar horario de atención</h6>
                    </div>

                    <div class="row align-items-end">
                        <div class="col-md-3 mb-3">
                            <label for="txtHoraInicioEdit" class="form-label">Hora Inicio</label>
                            <asp:DropDownList
                                ID="ddlHoraInicioEdit"
                                runat="server"
                                TextMode="Time"
                                CssClass="form-control"
                            ></asp:DropDownList>
                        </div>

                        <div class="col-md-3 mb-3">
                            <label for="txtHoraFinEdit" class="form-label">Hora Fin</label>
                            <asp:DropDownList
                                ID="ddlHoraFinEdit"
                                runat="server"
                                CssClass="form-control"
                            ></asp:DropDownList>
                        </div>
                    </div>
                    <!-- Fin de Horarios -->
                    <div>
                        <h6>Visualizar días de atención</h6>
                    </div>

                    <!-- Caja de los Días de Atención -->
                    <div class="card p-3 mb-3">
                        <div class="row">
                            <div class="col-6 col-md-4">
                                <div class="form-check">
                                    <input class="form-check-input" disabled="disabled" runat="server" type="checkbox" id="lunesEdit">
                                    <label class="form-check-label" for="lunes">Lunes</label>
                                </div>
                            </div>
                            <div class="col-6 col-md-4">
                                <div class="form-check">
                                    <input class="form-check-input" disabled="disabled" runat="server" type="checkbox" id="martesEdit">
                                    <label class="form-check-label" for="martes">Martes</label>
                                </div>
                            </div>
                            <div class="col-6 col-md-4">
                                <div class="form-check">
                                    <input class="form-check-input" disabled="disabled" runat="server" type="checkbox" id="miercolesEdit">
                                    <label class="form-check-label" for="miercoles">Miércoles</label>
                                </div>
                            </div>
                            <div class="col-6 col-md-4">
                                <div class="form-check">
                                    <input class="form-check-input" disabled="disabled" runat="server" type="checkbox" id="juevesEdit">
                                    <label class="form-check-label" for="jueves">Jueves</label>
                                </div>
                            </div>
                            <div class="col-6 col-md-4">
                                <div class="form-check">
                                    <input class="form-check-input" disabled="disabled" runat="server" type="checkbox" id="viernesEdit">
                                    <label class="form-check-label" for="viernes">Viernes</label>
                                </div>
                            </div>
                            <div class="col-6 col-md-4">
                                <div class="form-check">
                                    <input class="form-check-input" disabled="disabled" runat="server" type="checkbox" id="sabadoEdit">
                                    <label class="form-check-label" for="sabado">Sábado</label>
                                </div>
                            </div>
                            <div class="col-6 col-md-4">
                                <div class="form-check">
                                    <input class="form-check-input" disabled="disabled" runat="server" type="checkbox" id="domingoEdit">
                                    <label class="form-check-label" for="domingo">Domingo</label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!-- FIN DE Caja de los Días de Atención -->

                    <asp:HiddenField ID="hiddenIdEspacio" runat="server" />
                    <asp:HiddenField ID="hiddenIdDistrito" runat="server" />

                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button CssClass="btn btn-success" ID="btnActualizarEspacioEdit" runat="server" Text="Actualizar" OnClick="btnActualizarEspacioEdit_Click" CommandArgument='<%# Eval("idEspacio") %>'/>
                </div>
            </div>
        </div>
    </div>

    <!---------------------------- FIN DE EDICION MODAL  --------------------------------------------->

    <!-- Modal de Confirmación -->
    <div class="modal fade" id="mostrarConfEspacio" tabindex="-1" aria-labelledby="mostrarConfEspacioLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">

                <div class="modal-header modal-header-rojo text-white">
                    <h5 class="modal-title" id="mostrarConfEspacioLabel">Ventana de confirmación</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>

                <div class="modal-body d-flex align-items-center modal-body-confirmacion">
                    
                    <div class="icono-confirmacion me-3">
                        <div class="icono-circulo">
                            <i class="bi bi-info-lg fs-1"></i>
                        </div>
                    </div>

                    <div id="modalConfirmacionCuerpo" class="fs-5">
                        ¿Está seguro que desea eliminar este registro?
                    </div>
                </div>

                <div class="modal-footer justify-content-center">
                    <button type="button" class="btn btn-dark px-4" data-bs-dismiss="modal"><em>No</em></button>
                    <asp:Button ID="btnConfirmarAccion" runat="server" CssClass="btn btn-dark px-4" Text="Sí" OnClick="btnConfirmarAccion_Click" />
                </div>

            </div>
        </div>
    </div>

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

    <script type="text/javascript">
        function abrirPaso1() {
            // Oculta el modal actual (paso 2)
            var modal2 = bootstrap.Modal.getInstance(document.getElementById('modalPaso2'));
            if (modal2) modal2.hide();

            // Muestra el modal del paso anterior (paso 1)
            var modal1 = new bootstrap.Modal(document.getElementById('modalPaso1'));
            modal1.show();
        }

    </script>

    <!-- Abrir modal de Edición -->
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
        function abrirModalEdicion() {
            var modalEdicion = bootstrap.Modal.getInstance(document.getElementById('modalEdicionEspacio'));
            modalEdicion.show();
        }
        function mostrarConfEspacio(id) {
            // Obtener correctamente el ID generado por ASP.NET
            var hiddenField = document.getElementById('<%= hdnIdAEliminar.ClientID %>');
            if (hiddenField) {
                hiddenField.value = id;
            
            } else {
                    console.error("No se encontró el campo oculto.");
                return; // Importante: salir de la función si no se encuentra el campo oculto
            }

            // Obtener el elemento del modal
            var modalElement = document.getElementById('mostrarConfEspacio');
            if (!modalElement) {
                    console.error("No se encontró el elemento del modal.");
                return; // Importante: salir de la función si no se encuentra el modal
            }

            // Crear una instancia del modal de Bootstrap
            var modalEliminar = new bootstrap.Modal(modalElement);

            // Mostrar el modal
            modalEliminar.show();
        }
        function guardarDiasSeleccionados() {
            const dias = [];
            const ids = {
            lunes: '<%= chkLunes.ClientID %>',
            martes: '<%= chkMartes.ClientID %>',
            miercoles: '<%= chkMiercoles.ClientID %>',
            jueves: '<%= chkJueves.ClientID %>',
            viernes: '<%= chkViernes.ClientID %>',
            sabado: '<%= chkSabado.ClientID %>',
            domingo: '<%= chkDomingo.ClientID %>'
            };

            for (let dia in ids) {
                const chk = document.getElementById(ids[dia]);
                if (chk && chk.checked) {
                    dias.push(dia.toUpperCase()); // o el valor que desees
                }
            }

            // Guardamos en el hidden
            document.getElementById('<%= diasSeleccionados.ClientID %>').value = dias.join(',');
        }
    </script>

</asp:Content>
