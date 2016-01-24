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
    maintainAspectRatio: true,

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
var colorPink = "rgb(233, 30, 99)";
var colorPinkTransp = "rgba(233, 30, 99, .3)";
var colorPink200 = "#f48fb1";
var colorBlueGrey700 = "rgb(69, 90, 100)";
var colorBlueGrey700Transp = "rgba(69, 90, 100, .3)";
var ColorBlueGrey300 = "#90a4ae";

var statsLearningHistoryMonth;
var statsCardsBreakdown;

// Get the context of the canvas element we want to select
var loadStatsLearningHistoryMonth = function() {
  var element = document.getElementById('stats-learning-history-month');
  var dataLearned = element.getAttribute('data-learned').split('|');
  var dataReviewed = element.getAttribute('data-reviewed').split('|');
  var numberOfPiecesOfData = Math.min(dataLearned.length, dataReviewed.length);
  var dateLabels = [];
  for (var i = 0 ; i <= numberOfPiecesOfData-1; i++) { dateLabels[numberOfPiecesOfData-1-i] = i.toString(); }
  var data = {
    labels: dateLabels,
    datasets: [
      {
        label: "Učeno",
        fillColor: "rgba(233, 30, 99, .3)",
        strokeColor: "rgba(233, 30, 99, 1)",
        pointColor: "rgba(233, 30, 99, 1)",
        pointStrokeColor: "#fff",
        pointHighlightFill: "#fff",
        pointHighlightStroke: "rgba(233, 30, 99, 1)",
        data: dataLearned,
      },
      {
        label: "Obnovljeno",
        fillColor: "rgba(69, 90, 100, .3)",
        strokeColor: "rgba(69, 90, 100, 1)",
        pointColor: "rgba(69, 90, 100, 1)",
        pointStrokeColor: "#fff",
        pointHighlightFill: "#fff",
        pointHighlightStroke: "rgba(69, 90, 100, 1)",
        data: dataReviewed,
      }
    ]
  }
  statsLearningHistoryMonth = new Chart(element.getContext('2d')).Line(data);
  var legend = statsLearningHistoryMonth.generateLegend();
  $('#stats-learning-history-month-wrapper').append(legend);
}

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

var loadStatsPoints = function() {
  var element = document.getElementById('stats-points-week');
  var dataPoints = element.getAttribute('data-points').split('|');
  var numberOfPiecesOfData = dataPoints.length;
  var dateLabels = [];
  for (var i = 0 ; i <= numberOfPiecesOfData-1; i++) { dateLabels[numberOfPiecesOfData-1-i] = i.toString(); }
  var data = {
    labels: dateLabels,
    datasets: [
      {
        label: "Poeni",
        fillColor: colorPinkTransp,
        strokeColor: colorPink,
        pointColor: colorPink,
        pointStrokeColor: "#fff",
        pointHighlightFill: "#fff",
        pointHighlightStroke: colorPink,
        data: dataPoints,
      }
    ]
  }
  statsPoints = new Chart(element.getContext('2d')).Line(data);
  var legend = statsPoints.generateLegend();
  $('#stats-points-week-wrapper').append(legend);
}

var loadStatsTime = function() {
  var element = document.getElementById('stats-time-week');
  var dataPoints = element.getAttribute('data-time').split('|');
  var numberOfPiecesOfData = dataPoints.length;
  var dateLabels = [];
  for (var i = 0 ; i <= numberOfPiecesOfData-1; i++) { dateLabels[numberOfPiecesOfData-1-i] = i.toString(); }
  var data = {
    labels: dateLabels,
    datasets: [
      {
        label: "Vreme",
        fillColor: colorBlueGrey700Transp,
        strokeColor: colorBlueGrey700,
        pointColor: colorBlueGrey700,
        pointStrokeColor: "#fff",
        pointHighlightFill: "#fff",
        pointHighlightStroke: colorBlueGrey700,
        data: dataPoints,
      }
    ]
  }
  statsPoints = new Chart(element.getContext('2d')).Line(data);
  var legend = statsPoints.generateLegend();
  $('#stats-time-week-wrapper').append(legend);
}

$(document).ready(function() {
  loadStatsLearningHistoryMonth();
  loadStatsCardsBreakdown();
  loadStatsPoints();
  loadStatsTime();
});
