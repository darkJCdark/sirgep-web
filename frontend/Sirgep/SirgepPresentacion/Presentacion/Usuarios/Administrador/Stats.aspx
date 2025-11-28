<%@ Page Title="" Language="C#" MasterPageFile="~/MainLayout.Master" AutoEventWireup="true" CodeBehind="Stats.aspx.cs" Inherits="SirgepPresentacion.Presentacion.Usuarios.Administrador.Stats" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Estadísticas
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Encabezado" runat="server">
    <style type="text/css">
        .chart-container {
            width: 100%;
            height: 500px;
            position: relative;
        }
    </style>
    <style type="text/css">
        .chart-container3 {
            width: 100%;
            height:340px;
            position: relative;
            display: flex;
            justify-content: center; /* centrado horizontal */
            align-items: center;     /* opcional: centrado vertical */
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <div class="container">
        <h2 class ="mb-4 text-center">Estadísticas del Servicio Web</h2>
        <div class="row">
            <!-- Column izquierda (2 pies) -->
            <div class="col-md-6">
                <div class="mb-4">
                    <h4 class="text-center">Espacios Más Reservados en <%= DateTime.Now.ToString("MMMM") %></h4>
                    <div class="chart-container3">
                        <canvas id="pieChart"></canvas>
                    </div>
                </div>
                <div class="mb-4">
                    <h4 class="text-center">Eventos Más Vendidos en <%= DateTime.Now.ToString("MMMM") %></h4>
                    <div class="chart-container3">
                        <canvas id="pieChart2"></canvas>
                    </div>
                </div>
            </div>

            <!-- Column derecha (línea) -->
            <div class="col-md-6">
                <h4 class="text-center">Reservas y Entradas en <%= DateTime.Now.Year %></h4>
                <div class="chart-container">
                    <canvas id="lineChart"></canvas>
                </div>
            </div>
        </div>
    </div>

    <!-- Chart.js CDN -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <script type="text/javascript">

        var dataLineChart = JSON.parse('<%= DataLineChartJson %>');
        var dataPieChart = JSON.parse('<%= DataPieChartJson %>');
        var dataPieChart2 = JSON.parse('<%= DataPieChart2Json %>');

        // Escalado automático
        function calcularEscalaMaxima(data) {
            let max = Math.max(...data);
            if (max <= 10) return 12;
            if (max <= 20) return 24;
            if (max <= 30) return 36;
            return Math.ceil(max * 1.2);
        }

        const ctxLine = document.getElementById('lineChart').getContext('2d');
        const lineChart = new Chart(ctxLine, {
            type: 'line',
            data: {
                labels: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
                datasets: [{
                    label: 'Reservas',
                    data: dataLineChart.reservas,
                    borderColor: '#f10909',
                    backgroundColor: 'rgba(241, 9, 9, 0.2)',
                    fill: true,
                    tension: 0.3
                }, {
                    label: 'Entradas',
                    data: dataLineChart.entradas,
                    borderColor: '#1da7db',
                    backgroundColor: 'rgba(29, 167, 219, 0.2)',
                    fill: true,
                    tension: 0.3
                }
                ]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true,
                        max: calcularEscalaMaxima([
                            ...dataLineChart.reservas,
                            ...dataLineChart.entradas
                        ])
                    }
                }
            }
        });

        const ctxPie = document.getElementById('pieChart').getContext('2d');
        const pieChart = new Chart(ctxPie, {
            type: 'pie',
            data: {
                labels: dataPieChart.map(e => e.nombre),
                datasets: [{
                    data: dataPieChart.map(e => e.cantidad),
                    backgroundColor: ['#1da7db', '#ffc107', '#198754', '#0d6efd', '#6f42c1']
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
        const ctxPie2 = document.getElementById('pieChart2').getContext('2d');
        const pieChart2 = new Chart(ctxPie2, {
            type: 'pie',
            data: {
                labels: dataPieChart2.map(e => e.nombre),
                datasets: [{
                    data: dataPieChart2.map(e => e.cantidad),
                    backgroundColor: ['#ffc107', '#198754', '#dc3545', '#6610f2', '#20c997']
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });

    </script>
</asp:Content>