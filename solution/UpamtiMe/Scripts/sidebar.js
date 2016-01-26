/**
 * Grafici
 */
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
}

/**
 * Expandovanje favorites
 */
$('.acordeon-title').click(function() {
  $(this).parent().parent().children().removeClass('expanded');
  $(this).parent().addClass('expanded');
});

 $(window).bind('load', function() {
  var temp = [
    { dataName: 'data-dates',    color: "",               label: "",              },
    { dataName: 'data-points',   color: colorBlueGrey700, label: 'Poeni',         }
  ];
  loadGraphLine($('#sidebar-graph-points'), temp, 7);
});
