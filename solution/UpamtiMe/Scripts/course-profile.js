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

// Otvaranje padajuce liste za sesije
// TODO refaktorisi ovo moze svuda
$('.button-rect .count').click(function(e) {
  e.preventDefault(); // da se ne ode na link
  $(this).siblings('ul').toggleClass('show');
});

$(document).ready(function() {
});
