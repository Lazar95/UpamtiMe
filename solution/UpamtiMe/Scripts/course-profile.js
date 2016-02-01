$('.levels').on('click', '.circle-display', function() {
  var $level = $(this).parent();
  var index = $level.index() + 1;
  if ($level.hasClass('active')) {
    $($level.removeClass('active'));
  } else {
    var size = $('main').attr('data-size');
    var $okolni = $level.nextUntil('.clearfix-' + size);
    var $okolni = $okolni.add($level.prevUntil('.clearfix-' + size));
    var $okolni = $okolni.not('.clearfix');
    console.log($okolni.length);
    if ($okolni.hasClass('active')) {
      // Menja se kroz istu grupu
      console.log('ista grupa');
      $okolni.find('.more-info-outer').css('height', '240px').css('max-height', '240px').css('min-height', '240px');
      $okolni.removeClass('active');
      $level.addClass('active');
      setTimeout(function() { $level.parent().find('.more-info-outer').removeAttr('style') }, 500);
    } else {
      // Manje se medju grupama
      console.log('nije ista');
      $('.level').removeClass('active');
      $level.addClass('active');
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
$(document).on('click', '.button-rect .count', function(e) {
  e.preventDefault(); // da se ne ode na link
  $(this).siblings('ul').toggleClass('show');
});

$(document).ready(function() {
});
