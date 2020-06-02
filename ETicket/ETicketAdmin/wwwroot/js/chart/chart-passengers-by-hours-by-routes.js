﻿// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';

function number_format(number, decimals, dec_point, thousands_sep) {
    // *     example: number_format(1234.56, 2, ',', ' ');
    // *     return: '1 234,56'
    number = (number + '').replace(',', '').replace(' ', '');
    var n = !isFinite(+number) ? 0 : +number,
        prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
        sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
        dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
        s = '',
        toFixedFix = function (n, prec) {
            var k = Math.pow(10, prec);
            return '' + Math.round(n * k) / k;
        };
    // Fix for IE parseFloat(0.55).toFixed(0) = 0;
    s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
    if (s[0].length > 3) {
        s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
    }
    if ((s[1] || '').length < prec) {
        s[1] = s[1] || '';
        s[1] += new Array(prec - s[1].length + 1).join('0');
    }
    return s.join(dec);
}

function yyyy_mm_dd(date) {
    //var now = new Date();
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return '' + y + "-" + (m < 10 ? '0' : '') + m + "-" + (d < 10 ? '0' : '') + d;
}

function dynamicColors() {
    var r = Math.floor(Math.random() * 255);
    var g = Math.floor(Math.random() * 255);
    var b = Math.floor(Math.random() * 255);
    return "rgba(" + r + "," + g + "," + b + ", 0.5)";
}

function poolColors(a) {
    var pool = [];
    for (i = 0; i < a; i++) {
        pool.push(dynamicColors());
    }
    return pool;
}

$(document).ready(function () {
    var selectedDate = new Date(new Date().setMonth(new Date().getMonth() - 1));
    $('#passengers-by-hours-by-routes-date').val(yyyy_mm_dd(selectedDate));
    refreshPassengersByRoutesByHoursChart();
});

$('#passengers-by-hours-by-routes-date').change(function () {
    refreshPassengersByRoutesByHoursChart();
})

var PassengersByRoutesByHoursChart = null;
var chartData;
function refreshPassengersByRoutesByHoursChart() {
    var start = new Date($('#passengers-by-hours-by-routes-date').val());

    if (isNaN(start.valueOf())) {
        return;
    }

    var actionUrl = '/metrics/GetPassengersByHoursByRoutes' + "?selectedDate=" + start.toISOString() + '&selectedRoutesId=' + selectedRoutesPassengersByRoutesByHoursChart;
    $.getJSON(actionUrl, function (response) {
        if (response != null) {
            rawData = response;

            var chartData = new Array(24);
            var datasetsChart = new Array(24);
            var chartColors = poolColors(rawData.Labels.length);

            for (var i = 0; i < chartData.length; i++) {
                chartData[i] = new Array(rawData.Data[i].length);
                for (var j = 0; j < chartData[i].length; j++) {
                    chartData[i][j] = [i, i + (rawData.Data[i][j] / rawData.MaxPassengersByRoute)];
                }
                var temp = {
                    label: i + ' hour',
                    data: chartData[i], //routes data
                    backgroundColor: chartColors
                }

                datasetsChart[i] = temp
            }

            var ctx = document.getElementById("passengers-by-hours-by-routes");

            if (PassengersByRoutesByHoursChart != null) {
                PassengersByRoutesByHoursChart.destroy();
            }

            if (chartData.errorMessage) {
                $("#passengers-by-hours-by-routes-error").html(chartData.errorMessage);
            }
            else {
                $("#passengers-by-hours-by-routes-error").html("");
            }

            PassengersByRoutesByHoursChart = new Chart(ctx, {
                type: 'horizontalBar',
                data: {
                    labels: rawData.Labels,
                    datasets: datasetsChart
                },
                options: {
                    maintainAspectRatio: false,
                    scales: {
                        xAxes: [{
                            scaleLabel: {
                                display: true,
                                labelString: 'hours'
                            },
                            stacked: false,
                            time: {
                                unit: 'hour'
                            },
                            gridLines: {
                                display: true,
                                drawBorder: false
                            },
                            ticks: {
                                maxTicksLimit: 25,
                                stepSize: 1
                            },
                        }],
                        yAxes: [{
                            stacked: true,
                        }]
                    },
                    legend: {
                        display: false,
                    },
                    title: {
                        display: false
                    }
                }
            });
            PassengersByRoutesByHoursChart.canvas.parentNode.style.height = "" + (rawData.Labels.length * 30) + "px";
        }
    })
};