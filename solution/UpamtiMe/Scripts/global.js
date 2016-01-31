/**
 * Tooltipovi //TODO refaktorisi da se vidi svuda
 */

var TOOLTIP_TIMEOUT = 0;

var showTooltip = function($obj, direction, margin) {
  if ($obj.is('[data-tooltip-shown]')) return;
  $obj.attr('data-tooltip-shown', '');
  TOOLTIP_TIMEOUT = setTimeout(function() {
    // Sve ovo pokrecemo tek nakon sto istekne delay.

    var objSize = $obj.get(0).getBoundingClientRect();
    var $tt = $('<div class="tooltip">' + $obj.attr('data-tooltip-text') + '</div>');
    $('body').append($tt);
    var tooltipSize = $tt.get(0).getBoundingClientRect();

    switch (direction) {
      case 'top':
        var top = objSize.top - margin - tooltipSize.height;
        var left = objSize.left + objSize.width/2 - tooltipSize.width/2;
        break;
      case 'bottom':
        var top = objSize.top + objSize.height + margin;
        var left = objSize.left + objSize.width/2 - tooltipSize.width/2;
        break;
      case 'left':
        var top = objSize.top + objSize.height/2 - tooltipSize.height/2;
        var left = objSize.left - margin - tooltipSize.width;
        break;
      case 'right':
        var top = objSize.top + objSize.height/2 - tooltipSize.height/2;
        var left = objSize.left + objSize.width + margin;
        break;
    }

    $tt.css('top', top + 'px').css('left', left + 'px');
  }, $obj.attr('data-tooltip-delay') == undefined ? 350 : $obj.attr('data-tooltip-delay'));
}

$(document).on('mouseover', '.tooltippable', function() {
  var direction = $(this).is('[data-tooltip-direction]') ? $(this).attr('data-tooltip-direction') : 'top';
  var margin = $(this).is('[data-tooltip-margin]') ? $(this).attr('data-tooltip-margin') : 12;
  showTooltip($(this), direction, margin);
});

$(document).on('mouseleave', '.tooltippable', function() {
  if (!$(this).is('[data-tooltip-shown]')) return;
  clearTimeout(TOOLTIP_TIMEOUT);
  $(this).removeAttr('data-tooltip-shown');
  $('.tooltip').fadeOut(50);
  setTimeout(function() {
    $('.tooltip').remove();
  }, 50);
});
