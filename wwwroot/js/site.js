// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
var selectedCourses = [];

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
            //allowPointSelect: true,
            //selected: false,
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
            },
            point: {
                //selected: false,
                events: {
                    click: function (e) {

                        var course = {
                            name: this.series.name, code: this.name, color: this.color
                        };
                        var len = selectedCourses.length;
                        //if its not selected, select it
                        if (this.color != 'white') {
                            selectedCourses[len] = course;
                            this.update({ color: 'white' });
                            //this.update({ selected: 'true' });
                            console.log(
                                "Selected course: " + selectedCourses[len].name + selectedCourses[len].code
                                + " tot:" + (len + 1));
                        } else { // if its selected, deselect it
                            var temp = [];
                            var t = 0;
                            for (var i = 0; i < len; i++) {
                                if (selectedCourses[i].name == course.name && selectedCourses[i].code == course.code) {

                                    //this.update({ selected: 'false' });
                                    this.update({ color: selectedCourses[i].color });
                                    console.log("Deselecting");
                                    // skipping this one

                                } else {
                                    temp[t] = selectedCourses[i];
                                    t++;
                                }
                            }
                            selectedCourses = temp;
                        }

                        console.log("\nShowing the selected courses:\n");
                        if (selectedCourses.length > 0) {
                            for (var i = 0; i < selectedCourses.length; i++) {
                                console.log(selectedCourses[i].name + selectedCourses[i].code);
                            }
                        }

                    }
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
