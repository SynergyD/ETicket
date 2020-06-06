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

var ChartScale = {
    ByDays : 1,
    ByMonths : 2,
    ByYears : 3
}

var chartScale = ChartScale.ByDays;

function yyyy_mm_dd(date) {
    //var now = new Date();
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return '' + y + "-" + (m < 10 ? '0' : '') + m + "-" + (d < 10 ? '0' : '') + d;
}

function yyyy_mm(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    return '' + y + "-" + (m < 10 ? '0' : '') + m;
}

$(document).ready(function () {
    var dateStart = new Date(new Date().setMonth(new Date().getMonth() - 1));
    var dateEnd = new Date();
    $('#passengers-by-time-start').val(yyyy_mm_dd(dateStart));
    $('#passengers-by-time-end').val(yyyy_mm_dd(dateEnd));
    refreshChart();
});

$('#passengers-by-time-start').change(function () {
    refreshChart();
})
$('#passengers-by-time-end').change(function () {
    refreshChart();
})
$('#passengers-by-time-by-days').change(function () {
    ToDatePicker();
    refreshChart();
})
$('#passengers-by-time-by-months').change(function () {
    ToMonthPicker();
    refreshChart();
})
$('#passengers-by-time-by-years').change(function () {
    ToYearPicker();
    refreshChart();
})

var isMonthDatePick = false;
function ToDatePicker() {
    var start = new Date($('#passengers-by-time-start').val());
    var end = new Date($('#passengers-by-time-end').val());

    if (chartScale == ChartScale.ByMonths) {
        end = new Date(end.getFullYear(), end.getMonth() + 1, 0) > new Date() ? new Date() : new Date(end.getFullYear(), end.getMonth() + 1, 0);
    }

    if (chartScale == ChartScale.ByYears) {
        start = new Date(start.getFullYear(), 0, 1);
        end = new Date(end.getFullYear() + 1, 0, 0) > new Date() ? new Date() : new Date(end.getFullYear() + 1, 0, 0);
    }

    $('#passengers-by-time-start').attr('type', 'date');
    $('#passengers-by-time-end').attr('type', 'date');
    $('#passengers-by-time-start').attr('max', yyyy_mm_dd(new Date()));
    $('#passengers-by-time-end').attr('max', yyyy_mm_dd(new Date()));

    $('#passengers-by-time-start').val(yyyy_mm_dd(start));
    $('#passengers-by-time-end').val(yyyy_mm_dd(end));

    chartScale = ChartScale.ByDays;
}

function ToMonthPicker() {
    var start = new Date($('#passengers-by-time-start').val());
    var end = new Date($('#passengers-by-time-end').val());

    if (chartScale == ChartScale.ByYears) {
        start = new Date(start.getFullYear(), 0, 1);
        end = new Date(end.getFullYear() + 1, 0, 0) > new Date() ? new Date() : new Date(end.getFullYear(), end.getMonth() + 1, 0);; 
    }

    $('#passengers-by-time-start').attr('type', 'month');
    $('#passengers-by-time-end').attr('type', 'month');
    $('#passengers-by-time-start').attr('max', new Date());
    $('#passengers-by-time-end').attr('max', new Date());

    $('#passengers-by-time-start').val(yyyy_mm(start));
    $('#passengers-by-time-end').val(yyyy_mm(end));

    chartScale = ChartScale.ByMonths;
}

function ToYearPicker() {
    var start = new Date($('#passengers-by-time-start').val());
    var end = new Date($('#passengers-by-time-end').val());

    $('#passengers-by-time-start').attr('type', 'number');
    $('#passengers-by-time-end').attr('type', 'number');
    $('#passengers-by-time-start').attr('min', '2000');
    $('#passengers-by-time-start').attr('max', new Date().getFullYear());
    $('#passengers-by-time-end').attr('min', '2000');
    $('#passengers-by-time-end').attr('max', new Date().getFullYear());

    $('#passengers-by-time-start').val(start.getFullYear());
    $('#passengers-by-time-end').val(end.getFullYear());

    chartScale = ChartScale.ByYears;
}

var selectedRoutesPassengerByTimeChart = [];

$(document).ready(function () {
    var slimSelectPassengerByTime = new SlimSelect({
        select: '#routes-for-passengers-by-time',
        searchingText: 'Searching...', // Optional - Will show during ajax request
        hideSelectedOption: true,
        placeholder: 'Select routes',
        onChange: (info) => {
            selectedRoutesPassengerByTimeChart = [];
            for (let i = 0; i < info.length; i++) {
                selectedRoutesPassengerByTimeChart.push(info[i].value);
            }
            refreshChart();
        }
    });

    var actionUrl = '/routes/GetRoutesList';
    $.getJSON(actionUrl, function (response) {
        let responseResult = [];
        if (response != null) {
            for (let i = 0; i < response.length; i++) {
                responseResult.push({ value: response[i].Id, text: response[i].Number })
            }
        }

        slimSelectPassengerByTime.setData(responseResult);
    });
})


var passengerByTimeChart = null;

function refreshChart() {
    var start = new Date($('#passengers-by-time-start').val());
    var end = new Date($('#passengers-by-time-end').val());
    if (isNaN(start.valueOf()) || isNaN(end.valueOf())) {
        return;
    }
    if (chartScale == ChartScale.ByMonths) {
        end = new Date(end.getFullYear(), end.getMonth() + 1, 0);
    }
    if (chartScale == ChartScale.ByYears) {
        end = new Date(end.getFullYear() + 1, 0, 0);
    }
   
    var actionUrl = '/metrics/PassengersByTime' + "?startPeriod=" + start.toISOString() + "&endPeriod=" + end.toISOString() + "&scale=" + chartScale;
    $.getJSON(actionUrl, function (response) {
        if (response != null) {
            chartData = response;
            var maxY = Math.max.apply(null, chartData.Data);

            var ctx = document.getElementById("passengers-by-time");

            if (passengerByTimeChart != null) {
                passengerByTimeChart.destroy();
            }

            if (chartData.ErrorMessage) {
                $("#passengers-by-time-error").html(chartData.ErrorMessage);
            }
            else {
                $("#passengers-by-time-error").html("");
            }

            passengerByTimeChart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: chartData.Labels,
                    datasets: [{
                        label: "passengers",
                        lineTension: 0.3,
                        backgroundColor: "rgba(78, 115, 223, 0.05)",
                        borderColor: "rgba(78, 115, 223, 1)",
                        pointRadius: 3,
                        pointBackgroundColor: "rgba(78, 115, 223, 1)",
                        pointBorderColor: "rgba(78, 115, 223, 1)",
                        pointHoverRadius: 3,
                        pointHoverBackgroundColor: "rgba(78, 115, 223, 1)",
                        pointHoverBorderColor: "rgba(78, 115, 223, 1)",
                        pointHitRadius: 10,
                        pointBorderWidth: 2,
                        data: chartData.Data,
                    }],
                },
                options: {
                    maintainAspectRatio: false,
                    layout: {
                        padding: {
                            left: 10,
                            right: 25,
                            top: 25,
                            bottom: 0
                        }
                    },
                    scales: {
                        xAxes: [{
                            time: {
                                unit: 'date'
                            },
                            gridLines: {
                                display: false,
                                drawBorder: false
                            },
                            ticks: {
                                maxTicksLimit: 10
                            }
                        }],
                        yAxes: [{
                            ticks: {
                                suggestedMin: 0,
                                maxTicksLimit: 8,
                                padding: 10,
                                // Include a dollar sign in the ticks
                                callback: function (value, index, values) {
                                    return number_format(value);
                                }
                            },
                            gridLines: {
                                color: "rgb(234, 236, 244)",
                                zeroLineColor: "rgb(234, 236, 244)",
                                drawBorder: false,
                                borderDash: [2],
                                zeroLineBorderDash: [2]
                            }
                        }],
                    },
                    legend: {
                        display: false
                    },
                    tooltips: {
                        backgroundColor: "rgb(255,255,255)",
                        bodyFontColor: "#858796",
                        titleMarginBottom: 10,
                        titleFontColor: '#6e707e',
                        titleFontSize: 14,
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        xPadding: 15,
                        yPadding: 15,
                        displayColors: false,
                        intersect: false,
                        mode: 'index',
                        caretPadding: 10,
                        callbacks: {
                            label: function (tooltipItem, chart) {
                                var datasetLabel = chart.datasets[tooltipItem.datasetIndex].label || '';
                                return datasetLabel + ': ' + number_format(tooltipItem.yLabel);
                            }
                        }
                    },
                }
            });
        }
    });
}


