$('.levels').on('click', '.circle-display', function() {
  var level = $(this).parent();
  if (level.hasClass('active')) {
    $(level.removeClass('active'));
  } else {
    $('.level').removeClass('active');
    level.addClass('active');
  }
});

$(document).ready(function() {
});
