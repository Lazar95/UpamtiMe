// http://www.chartjs.org/docs/
Chart.defaults.global = {
    // Boolean - Whether to animate the chart
    animation: true,

    // Number - Number of animation steps
    animationSteps: 60,

    // String - Animation easing effect
    // Possible effects are:
    // [easeInOutQuart, linear, easeOutBounce, easeInBack, easeInOutQuad,
    //  easeOutQuart, easeOutQuad, easeInOutBounce, easeOutSine, easeInOutCubic,
    //  easeInExpo, easeInOutBack, easeInCirc, easeInOutElastic, easeOutBack,
    //  easeInQuad, easeInOutExpo, easeInQuart, easeOutQuint, easeInOutCirc,
    //  easeInSine, easeOutExpo, easeOutCirc, easeOutCubic, easeInQuint,
    //  easeInElastic, easeInOutSine, easeInOutQuint, easeInBounce,
    //  easeOutElastic, easeInCubic]
    animationEasing: "easeOutQuart",

    // Boolean - If we should show the scale at all
    showScale: true,

    // Boolean - If we want to override with a hard coded scale
    scaleOverride: false,

    // ** Required if scaleOverride is true **
    // Number - The number of steps in a hard coded scale
    scaleSteps: null,
    // Number - The value jump in the hard coded scale
    scaleStepWidth: null,
    // Number - The scale starting value
    scaleStartValue: null,

    // String - Colour of the scale line
    scaleLineColor: "rgba(0,0,0,.1)",

    // Number - Pixel width of the scale line
    scaleLineWidth: 1,

    // Boolean - Whether to show labels on the scale
    scaleShowLabels: true,

    // Interpolated JS string - can access value
    scaleLabel: "<%=value%>",

    // Boolean - Whether the scale should stick to integers, not floats even if drawing space is there
    scaleIntegersOnly: true,

    // Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
    scaleBeginAtZero: false,

    // String - Scale label font declaration for the scale label
    scaleFontFamily: "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif",

    // Number - Scale label font size in pixels
    scaleFontSize: 10,

    // String - Scale label font weight style
    scaleFontStyle: "normal",

    // String - Scale label font colour
    scaleFontColor: "#666",

    // Boolean - whether or not the chart should be responsive and resize when the browser does.
    responsive: false,

    // Boolean - whether to maintain the starting aspect ratio or not when responsive, if set to false, will take up entire container
    maintainAspectRatio: false,

    // Boolean - Determines whether to draw tooltips on the canvas or not
    showTooltips: true,

    // Function - Determines whether to execute the customTooltips function instead of drawing the built in tooltips (See [Advanced - External Tooltips](#advanced-usage-custom-tooltips))
    customTooltips: false,

    // Array - Array of string names to attach tooltip events
    tooltipEvents: ["mousemove", "touchstart", "touchmove"],

    // String - Tooltip background colour
    tooltipFillColor: "rgba(0,0,0,0.6)",

    // String - Tooltip label font declaration for the scale label
    tooltipFontFamily: "'Roboto', 'Helvetica Neue', 'Helvetica', 'Arial', sans-serif",

    // Number - Tooltip label font size in pixels
    tooltipFontSize: 10,

    // String - Tooltip font weight style
    tooltipFontStyle: "normal",

    // String - Tooltip label font colour
    tooltipFontColor: "#fff",

    // String - Tooltip title font declaration for the scale label
    tooltipTitleFontFamily: "'Roboto', 'Helvetica Neue', 'Helvetica', 'Arial', sans-serif",

    // Number - Tooltip title font size in pixels
    tooltipTitleFontSize: 12,

    // String - Tooltip title font weight style
    tooltipTitleFontStyle: "bold",

    // String - Tooltip title font colour
    tooltipTitleFontColor: "#fff",

    // Number - pixel width of padding around tooltip text
    tooltipYPadding: 6,

    // Number - pixel width of padding around tooltip text
    tooltipXPadding: 6,

    // Number - Size of the caret on the tooltip
    tooltipCaretSize: 8,

    // Number - Pixel radius of the tooltip border
    tooltipCornerRadius: 6,

    // Number - Pixel offset from point x to tooltip edge
    tooltipXOffset: 10,

    // String - Template string for single tooltips
    tooltipTemplate: "<%if (label){%><%=label%>: <%}%><%= value %>",

    // String - Template string for multiple tooltips
    multiTooltipTemplate: "<%= value %>",

    // Function - Will fire on animation progression.
    onAnimationProgress: function(){},

    // Function - Will fire on animation completion.
    onAnimationComplete: function(){}
}

Chart.defaults.Line =  {

  ///Boolean - Whether grid lines are shown across the chart
  scaleShowGridLines : true,

  //String - Colour of the grid lines
  scaleGridLineColor : "rgba(0,0,0,.03)",

  //Number - Width of the grid lines
  scaleGridLineWidth : 1,

  //Boolean - Whether to show horizontal lines (except X axis)
  scaleShowHorizontalLines: true,

  //Boolean - Whether to show vertical lines (except Y axis)
  scaleShowVerticalLines: false,

  //Boolean - Whether the line is curved between points
  bezierCurve : true,

  //Number - Tension of the bezier curve between points
  bezierCurveTension : 0.4,

  //Boolean - Whether to show a dot for each point
  pointDot : true,

  //Number - Radius of each point dot in pixels
  pointDotRadius : 4,

  //Number - Pixel width of point dot stroke
  pointDotStrokeWidth : 2,

  //Number - amount extra to add to the radius to cater for hit detection outside the drawn point
  pointHitDetectionRadius : 20,

  //Boolean - Whether to show a stroke for datasets
  datasetStroke : true,

  //Number - Pixel width of dataset stroke
  datasetStrokeWidth : 2,

  //Boolean - Whether to fill the dataset with a colour
  datasetFill : true,

  //String - A legend template
  legendTemplate : "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span class=\"legend-color\" style=\"background-color:<%=datasets[i].strokeColor%>\"></span><span class=\"legend-name\"><%if(datasets[i].label){%><%=datasets[i].label%><%}%></span></li><%}%></ul>"
};

// Boje
var colorPink = "rgba(233, 30, 99, 1)";
var colorPinkTransp = "rgba(233, 30, 99, .3)";
var colorPink200 = "rgba(244, 143, 177, 1)";
var colorBlueGrey700 = "rgba(69, 90, 100, 1)";
var colorBlueGrey700Transp = "rgba(69, 90, 100, .3)";
var ColorBlueGrey300 = "#90a4ae";

var statsLearningHistoryMonth;
var statsCardsBreakdown;

var loadGraphLine = function($canvas, dataInfo, len) {
  // prosledjeno:
  var dataAttr = [];
  var dataAttrLength = [];
  for (var i = 0; i < dataInfo.length; i++) {
    dataAttr[i] = $canvas.attr(dataInfo[i].dataName).split('|').reverse();
    dataAttrLength[i] = dataAttr[i].length;
  }
  var numberOfPiecesOfData = Math.min(Math.min.apply(null, dataAttrLength), len);
  for (var i = 0; i < dataAttr.length; i++) {
    dataAttr[i] = dataAttr[i].splice(0, numberOfPiecesOfData).reverse();
  }

  // formiranje dataseta (ds)
  var ds = [];
  for (var i = 1; i < dataAttr.length; i++) {
    var temp = {
      label: dataInfo[i].label,
      fillColor: dataInfo[i].color.replace('1)', '.3)'),
      strokeColor: dataInfo[i].color,
      pointColor: dataInfo[i].color,
      pointStrokeColor: "#fff",
      pointHighlightFill: "#fff",
      pointHighlightStroke: dataInfo[i].color,
      data: dataAttr[i],
    };
    ds.push(temp);
  }

  var data = {
    labels: dataAttr[0],
    datasets: ds,
  }

  // bibliotecki pozivi
  var chart = new Chart($canvas.get(0).getContext('2d')).Line(data);
  var legend = chart.generateLegend();
  $canvas.append(legend);
  return chart;
}

// Pie - user
var loadStatsCardsBreakdown = function() {
  var element = document.getElementById('stats-cards-breakdown');
  var learn = element.getAttribute('data-learned');
  var review = element.getAttribute('data-review');
  var unseen = element.getAttribute('data-unseen');
  var data = [
    {
      value: learn,
      color: colorPink,
      highlight: "#FFF",
      label: "Naučene",
    },
    {
      value: review,
      color: colorPink200,
      highlight: "#FFF",
      label: "Za obnavljanje",
    },
    {
      value: unseen,
      color: colorBlueGrey700,
      highlight: "#FFF",
      label: "Nevidjene",
    },
  ];
  var options = {
    animationEasing: "easeOutQuart",
  }
  statsCardsBreakdown = new Chart(element.getContext('2d')).Pie(data, options);
}

// Pie - user
var loadStatsCourseTotalBreakdown = function($element) {
  var learn = $element.attr('data-learned');
  var review = $element.attr('data-review');
  var unseen = $element.attr('data-unseen');
  var data = [
    {
      value: learn,
      color: colorPink,
      highlight: "#FFF",
      label: "Naučene",
    },
    {
      value: review,
      color: colorPink200,
      highlight: "#FFF",
      label: "Za obnavljanje",
    },
    {
      value: unseen,
      color: colorBlueGrey700,
      highlight: "#FFF",
      label: "Nevidjene",
    },
  ];
  var options = {
    animationEasing: "easeOutQuart",
  }
  statsCardsBreakdown = new Chart($element.get(0).getContext('2d')).Pie(data, options);
}

var statsLearningHistoryMonthData = [];
var statsPointsWeekData = [];
var statsTimeWeekData = [];

var statsLearningHistoryMonthGraph;
var statsPointsWeekGraph;
var statsTimeWeekGraph;

$(document).ready(function() {

  // za korisnika
  statsLearningHistoryMonthData = [
    { dataName: 'data-dates',    color: "",               label: "",              },
    { dataName: 'data-learned',  color: colorPink,        label: 'Naučeno',       },
    { dataName: 'data-reviewed', color: colorBlueGrey700, label: 'Obnovljeno',    }
  ];
  statsLearningHistoryMonthGraph = loadGraphLine($('#stats-learning-history-month'), statsLearningHistoryMonthData, 30);

  statsPointsWeekData = [
    { dataName: 'data-dates',    color: "",               label: "",              },
    { dataName: 'data-points',   color: colorBlueGrey700, label: 'Poeni',         }
  ];
  statsPointsWeekGraph = loadGraphLine($('#stats-points-week'), statsPointsWeekData, 7);

  statsTimeWeekData = [
    { dataName: 'data-dates',    color: "",               label: "",              },
    { dataName: 'data-time',     color: colorBlueGrey700, label: 'Vreme',         }
  ];
  statsTimeWeekGraph = loadGraphLine($('#stats-time-week'), statsTimeWeekData, 7);

  loadStatsCardsBreakdown();

  // za svaki  kurs
  var temp = [
    { dataName: 'data-dates',    color: "",               label: "",              },
    { dataName: 'data-points',   color: colorBlueGrey700, label: 'Poeni',         }
  ];
  var elements = $('.stats-points-per-course');
  elements.each(function() {
    loadGraphLine($(this), temp, 7);
  });

  // Pie - trenutno nema
  $('.course-total-breakdown').each(function() {
    loadStatsCourseTotalBreakdown($(this));
  });
});

//TODO Refaktorisi
// npr. moze da se doda data-attr u #leaderboard-tabs
// i da se na osnovu toga gadja koji da se otvori
//TODO Refaktorisi
// Ovo treba da ide u neki globalni,
// trenutno je kopirano u course-profile.js i u users-index.js
$('#leaderboard-tabs').on('click', 'li', function() {
  $(this).siblings().removeClass('active');
  $(this).addClass('active');
  if ($(this).attr('id') == 'leaderboard-tabs-all') {
    $('#leaderboard-all-time-score').addClass('current');
    $('#leaderboard-week-score').removeClass('current');
    $('#leaderboard-month-score').removeClass('current');
  } else if ($(this).attr('id') == 'leaderboard-tabs-week') {
    $('#leaderboard-all-time-score').removeClass('current');
    $('#leaderboard-week-score').addClass('current');
    $('#leaderboard-month-score').removeClass('current');
  } else if ($(this).attr('id') == 'leaderboard-tabs-month') {
    $('#leaderboard-all-time-score').removeClass('current');
    $('#leaderboard-week-score').removeClass('current');
    $('#leaderboard-month-score').addClass('current');
  }
});

/**
 * MORE => FULL SCREEN
 */

$('.more').click(function() {
  $(this).siblings('.full-screen').addClass('visible');
});

$('.less').click(function() {
  $(this).closest('.full-screen').removeClass('visible');
})

/**
 * Canvas refit
 */

var canvasResize = function() {
  statsLearningHistoryMonthGraph.destroy();
  statsPointsWeekGraph.destroy();
  statsTimeWeekGraph.destroy();
  var size = document.getElementsByTagName('main')[0].getAttribute('data-size');
  var smAndBelow = (size == 'sm' || size == 'xs' || size == 'xxs' || size == 'xxxs')
  $('canvas').each(function() {
    var context = this.getContext('2d');
    context.clearRect(0, 0, this.width, this.height);
    var parentSize = this.parentNode.getBoundingClientRect();
    var h = Math.floor(parentSize.height);
    var w = Math.floor(parentSize.width);
    this.width = w;
    this.height = h;
  });
  loadGraphLine($('#stats-learning-history-month'), statsLearningHistoryMonthData, smAndBelow ? 7 : 30);
  loadGraphLine($('#stats-points-week'), statsPointsWeekData, 7);
  loadGraphLine($('#stats-time-week'), statsTimeWeekData, 7);
  loadStatsCardsBreakdown();
}
$(window).bind('load', function() {
  setTimeout(function() {
    canvasResize();
  }, 200);
});
var resizeTimer;
$(window).bind('resize', function() {
  clearTimeout(resizeTimer);
  resizeTimer = setTimeout(function() {
    canvasResize();
  }, 500);
});
