<%@ Page Title="" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="CalendarioReservas.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Usuarios.Administrador.CalendarioReservas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Calendario de Reservas
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Encabezado" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/index.global.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/fullcalendar@6.1.8/locales-all.global.min.js"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <div class="container mt-4">
        <h3 class="mb-4">Calendario de Reservas</h3>

        <div class="export-buttons mb-3">
            <asp:Button ID="btnExportarPDF" CssClass="btn btn-dark" runat="server" Text="Exportar PDF" OnClick="btnExportarPDF_Click" OnClientClick="setTimeout(function() { mostrarModalExito('Descarga exitosa', 'El calendario se ha descargado en PDF correctamente.'); }, 1000);" Enabled="false" />
            <asp:Button ID="btnExportarExcel" CssClass="btn btn-dark" runat="server" Text="Exportar Excel" OnClick="btnExportarExcel_Click" OnClientClick="setTimeout(function() { mostrarModalExito('Descarga exitosa', 'El calendario se ha descargado en EXCEL correctamente.'); }, 1000);" Enabled="false" />
        </div>

        <asp:Label ID="lblSinReservas" runat="server" Text="No hay reservas para el mes aún"
                   ForeColor="Red" Font-Bold="true" Visible="false"
                   Style="font-size:1.1em; margin-bottom:10px; display:block;" />

        <div id="calendar-block">
            <div id="calendar"></div>
        </div>

        <asp:HiddenField ID="hdnReservasJson" runat="server" />
        <asp:HiddenField ID="hdnMes" runat="server" />
        <asp:HiddenField ID="hdnAnio" runat="server" />
    </div>

    <!-- Modal Detalle Reserva -->
    <div class="modal fade" id="detalleReservaModal" tabindex="-1" role="dialog" aria-labelledby="detalleReservaLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header modal-header-rojo text-white">
                    <h5 class="modal-title" id="detalleReservaLabel">Detalle de Reserva</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Cerrar"></button>
                </div>
                <div class="modal-body">
                    <p><strong>N° Reserva:</strong> <span id="reservaNum"></span></p>
                    <p><strong>Espacio:</strong> <span id="reservaEspacio"></span></p>
                    <p><strong>Fecha:</strong> <span id="reservaFecha"></span></p>
                    <p><strong>Inicio:</strong> <span id="reservaInicio"></span></p>
                    <p><strong>Fin:</strong> <span id="reservaFin"></span></p>
                    <p><strong>Nombres:</strong> <span id="reservaNombres"></span></p>
                    <p><strong>Apellidos:</strong> <span id="reservaApellidos"></span></p>
                    <p><strong>Tipo de Documento:</strong> <span id="reservaTipoDoc"></span></p>
                    <p><strong>N° Documento:</strong> <span id="reservaNumDoc"></span></p>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-dark" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const reservasJson = document.getElementById('<%= hdnReservasJson.ClientID %>').value;
        const eventos = JSON.parse(reservasJson || "[]");

        // Lee mes y año de los campos ocultos
        const mes = parseInt(document.getElementById('<%= hdnMes.ClientID %>').value, 10) - 1; // JS: 0-based
        const anio = parseInt(document.getElementById('<%= hdnAnio.ClientID %>').value, 10);

        const calendarEl = document.getElementById('calendar');
        const calendar = new FullCalendar.Calendar(calendarEl, {
            initialView: 'dayGridMonth',
            locale: 'es',
            height: 'auto',
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,dayGridWeek,dayGridDay,listMonth'
            },
            buttonText: {
                today: 'hoy',
                month: 'mes',
                week: 'semana',
                day: 'día',
                list: 'lista'
            },
            initialDate: new Date(anio, mes, 1),
            events: eventos,
            datesSet: function (info) {
                // Obtén la fecha central de la vista (lo que el usuario ve en el título)
                const currentDate = calendar.getDate();
                const newMonth = currentDate.getMonth() + 1; // 1-12
                const newYear = currentDate.getFullYear();

                // Lee los valores iniciales de los campos ocultos
                const currentMonth = parseInt(document.getElementById('<%= hdnMes.ClientID %>').value, 10);
                const currentYear = parseInt(document.getElementById('<%= hdnAnio.ClientID %>').value, 10);

                // Solo redirigir si el mes o año realmente cambió respecto a los valores iniciales
                if (newMonth !== currentMonth || newYear !== currentYear) {
                    const url = window.location.pathname + '?mes=' + newMonth + '&anio=' + newYear;
                    window.location.href = url;
                }
            },
            eventClick: function (info) {
                mostrarDetalleReserva(info.event.extendedProps);
            }
        });

        calendar.render();
    });

        function mostrarDetalleReserva(reserva) {
            document.getElementById('reservaNum').textContent = reserva.numReserva;
            document.getElementById('reservaEspacio').textContent = reserva.espacio;
            document.getElementById('reservaFecha').textContent = reserva.fecha;
            document.getElementById('reservaInicio').textContent = reserva.horaInicio;
            document.getElementById('reservaFin').textContent = reserva.horaFin;
            document.getElementById('reservaNombres').textContent = reserva.nombres;
            document.getElementById('reservaApellidos').textContent = reserva.apellidos;
            document.getElementById('reservaTipoDoc').textContent = reserva.tipoDoc;
            document.getElementById('reservaNumDoc').textContent = reserva.numDoc;

            const modal = new bootstrap.Modal(document.getElementById('detalleReservaModal'));
            modal.show();
        }
    </script>
</asp:Content>
