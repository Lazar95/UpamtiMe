/**
 * Tooltipovi
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
  var size = document.getElementsByTagName('main')[0].getAttribute('data-size');
  if (size == 'lg' || size == 'md' || size == 'sm') {
    var direction = $(this).is('[data-tooltip-direction]') ? $(this).attr('data-tooltip-direction') : 'top';
    var margin = $(this).is('[data-tooltip-margin]') ? $(this).attr('data-tooltip-margin') : 12;
    showTooltip($(this), direction, margin);
  }
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


/**
 * Stil material-card za kurseve
 */
 $('#search-results-outer').on('click', '.show-more', function() {
   $(this).closest('.result').toggleClass('expanded');
 })
 $('#user-courses').on('click', '.show-more', function() {
   $(this).closest('.course-card').toggleClass('expanded');
 })

 /**
  * Upis trenutog lg/md/sm itd u main data-size
  */
 var responsiveMain = function() {
   w = $(window).width();
   var xs = 480;
   var sm = 960;
   var md = 1224;
   var lg = 1572;
   if (w > lg) $('main').attr('data-size', 'lg');
   else if (w > md) $('main').attr('data-size', 'md');
   else if (w > sm) $('main').attr('data-size', 'sm');
   else if (w > xs) $('main').attr('data-size', 'xs');
   else $('main').attr('data-size', 'xxs');
 }
 $(window).bind('resize', function() {
   responsiveMain();
 });
 $(window).bind('load', function() {
   responsiveMain();
 })

 /**
  * Canvas resize to fit
  */
/*
var canvasResize = function() {
  $('canvas').each(function() {
    var parentSize = this.parentNode.getBoundingClientRect();
    var h = Math.floor(parentSize.height);
    var w = Math.floor(parentSize.width);
    this.width = w;
    this.height = h;
    $(this).removeAttr('style');
  });
}
$(window).bind('load', function() {
  canvasResize();
});
var resizeTimer;
$(window).bind('resize', function() {
  clearTimeout(resizeTimer);
  resizeTimer = setTimeout(function() {
    canvasResize();
  }, 250);
});*/
//TODO: na resize ce da ispituje breakpointe i u zavisnosti od toga da uzima neke predefinisane vrednosti za sirinu
