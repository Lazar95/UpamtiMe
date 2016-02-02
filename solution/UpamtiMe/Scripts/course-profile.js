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
