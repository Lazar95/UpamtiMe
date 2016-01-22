$('.levels').on('click', '.circle-display', function() {
  var level = $(this).parent();
  var index = level.index() + 1;
  if (level.hasClass('active')) {
    $(level.removeClass('active'));
  } else {
    if (level.parent().children().hasClass('active')) {
      // Menja se kroz istu grupu
      // alert()
      level.parent().find('.more-info-outer').css('height', '240px').css('max-height', '240px').css('min-height', '240px');
      level.parent().children().removeClass('active');
      level.addClass('active');
      setTimeout(function() { level.parent().find('.more-info-outer').removeAttr('style') }, 500);
    } else {
      // Manje se medju grupama
      $('.level').removeClass('active');
      level.addClass('active');
    }
  }
});

$(document).ready(function() {
});
