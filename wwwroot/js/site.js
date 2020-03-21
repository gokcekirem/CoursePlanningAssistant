// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

Highcharts.chart('container', {
    chart: {
        type: 'packedbubble',
        height: '100%'
    },
    title: {
        text: 'Spring 2020'
    },
    tooltip: {
        useHTML: true,
        pointFormat: '<b>{point.name}:</b> {point.value} students</sub>'
    },
    plotOptions: {
        packedbubble: {
            minSize: '0%',
            maxSize: '200%',
            zMin: 0,
            zMax: 1000,
            layoutAlgorithm: {
                gravitationalConstant: 0.05,
                splitSeries: true,
                seriesInteraction: false,
                dragBetweenSeries: false,
                parentNodeLimit: true
            },
            dataLabels: {
                enabled: true,
                format: '{point.name}',
                style: {
                    color: 'black',
                    textOutline: 'none',
                    fontWeight: 'normal'
                }
            }
        }
    },
    series: [{
        name: 'HUMS',
        data: [{
            name: '118',
            value: 40
        }, {
            name: '110',
            value: 24
        },
        {
            name: "104",
            value: 30
        }]
    }, {
        name: 'COMP',
        data: [{
            name: "131",
            value: 60
        },
        {
            name: "437",
            value: 20
        },
        {
            name: "132",
            value: 60
        },
        {
            name: "106",
            value: 40
        },
        {
            name: "200",
            value: 60
        },
        {
            name: "202",
            value: 50
        }]
    }, {
        name: 'ASIU',
        data: [{
            name: "100",
            value: 40
        },
        {
            name: "110",
            value: 30
        },
        {
            name: "104",
            value: 30
        }]
    }, {
        name: 'ENGR',
        data: [{
            name: "200",
            value: 80
        },
        {
            name: "401",
            value: 40
        }]
    }]
});
