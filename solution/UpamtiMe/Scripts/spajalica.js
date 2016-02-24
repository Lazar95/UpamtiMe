/**
 * Globalne promenljive pocinju donjom crtom, _camelCase.
 * Globalne konstante koje u sustini sakrivaju implementaciju su CAPITALS.
 */

/**
 * Randomize array element order in-place.
 * Using Durstenfeld shuffle algorithm.
 * http://stackoverflow.com/a/12646864
 */
function shuffleArray(array) {
   for (var i = array.length - 1; i > 0; i--) {
       var j = Math.floor(Math.random() * (i + 1));
       var temp = array[i];
       array[i] = array[j];
       array[j] = temp;
   }
   return array;
}


// Returns a random number between min (inclusive) and max (exclusive)
function getRandomArbitrary(min, max) {
  var r = Math.floor(Math.random() * (max - min) + min);
  //console.log('Random: ' + r);
  return r;
}

// JSON za pitanja i odgovore
var _qa = [];
var ALLOW_GAME = true;

/**
 * //TODO
 *
 * Ovaj niz mora da se filtrira tako da se izbaci sve sto je duplikat ili po
 * pitanju ili po odgovoru, jer se tacnost odredjuje na osnovu ID-ja.
 * Ali ovo bi moralo da se radi na bekendu zbog broja kartica koje ucestvuju
 * u igri.
 */

$(window).bind('load', function() {

  parseTableOfGod();
  shuffleArray(_qa);

  var nums = [1, 2, 3, 4, 5, 6, 7, 8, 9];
  shuffleArray(nums);
  putNew(0, 'q', nums[0]);
  putNew(1, 'q', nums[1]);
  putNew(2, 'q', nums[2]);
  putNew(3, 'q', nums[3]);
  putNew(4, 'q', nums[4]);
  putNew(5, 'q', nums[5]);
  putNew(0, 'a', nums[6]);
  putNew(1, 'a', nums[7]);
  putNew(2, 'a', nums[8]);

  _nextChip = 6;

  _stack.push(3);
  _stack.push(4);
  _stack.push(5);

  _interval = setInterval( function(){ updateTimer(_timeRemaining); }, _timerUpdateInterval);

});


 // JSON za pitanja i odgovore
var parseTableOfGod = function() {
  table = $('#table-of-god');
  numberOfEntries = table.children('tbody').children('tr').length;

  for ( var i = 1; i <= numberOfEntries; i++ ) {
    var curr = table.children('tbody').children('tr:nth-child(' + i + ')');
    //console.log(curr);
    _qa.push( {
      "status": -1,
      "question" : curr.children('[data-type="question"]').text().trim(),
      "answer" : curr.children('[data-type="answer"]').text().trim(),
    } );
  }

  //console.log(_qa);
}


var _nextChip = 1;
var _stack = [];
var _grid = [];
var _selected = 0;

// Za skor
var _correctCount = 0;
var _incorrectCount = 0;
var _currentCombo = 0;
var _maxCombo = 0;
var _lives = 3;

var _baseScore = 1;
var _bonusTime = 0;
var _bonusCombo = 0;
var _score = 0;

// Za tajmer
const _maxTimeRemaining = 800000;
var _timeRemaining = _maxTimeRemaining;
const _dangerThreshold = 2000;
var _timerUpdateInterval = 10;
var _interval;

/**
 * Apdejtuje tajmer, i algoritamski i vizuelno.
 */
var updateTimer = function(timeRemaining) {
  // Izdvoji sekunde i milisekunde u posebne promenljive; pretvori ih u stringove
  var seconds = Math.floor(timeRemaining / 1000);
  var milliseconds = Math.floor((timeRemaining % 1000) / 10);
  seconds = '' + seconds;
  milliseconds = '' + milliseconds;

  // Dodaj vodecu nulu u sekundama i pratecu nulu u milisekundama ako je potrebno
  if (seconds.length == 1) seconds = '0' + seconds;
  if (milliseconds.length == 1) milliseconds = milliseconds + '0';

  var timer = $('.timer div');
  timer.css('height', (timeRemaining / _maxTimeRemaining * 100)  + '%')
    .removeClass('danger')
    .children('span')
      .html(seconds + '"');

  if (_timeRemaining < _dangerThreshold)
    timer.addClass('danger');

  _timeRemaining -= _timerUpdateInterval;

  if (_timeRemaining < 0) {
    clearInterval(_interval);
    timeOut();
  }
}

/**
 * Ispituje da li je igra gotova (normalno)
 */
var isGameOver = function() {
  for(var i = 1; i < _grid.length; i++) { // Da, od jedan, ne pitaj me zasto.
    if (_grid[i] != _qa.length)
      return false;
  }
  return true;
}

/**
 * Sta ce da se desi uvek, kako god da se zavrsi igra.
 * Ovu funkciju zovu timeOut, noMoreLives i goodJob u prvoj liniji.
 */
var gameOver = function() {
  ALLOW_GAME = false;
  clearInterval(_interval);

  var livesBonusPercentage = (_lives * 10);
  var correctnessBonusPercentage = ((_correctCount / (_correctCount + _incorrectCount)) * 10).toFixed(1);
  var comboBonusPercentage = (_maxCombo / 10.0).toFixed(1);

  var livesBonus = Math.ceil(livesBonusPercentage * _score / 100);
  var correctnessBonus = Math.ceil(correctnessBonusPercentage * _score / 100);
  var comboBonus = Math.ceil(comboBonusPercentage * _score / 100);

  $('#cover-linky .total-score span').html(_score);
  $('#cover-remaining-lives span').html('+' + livesBonusPercentage + '%');
  $('#cover-correctness span').html('+' + correctnessBonusPercentage + '%');
  $('#cover-max-combo span').html('+' + comboBonusPercentage + '%');

  $('#cover-linky .total-score').attr('data-init-points', _score);
  $('#cover-remaining-lives').attr('data-points', livesBonus);
  $('#cover-correctness').attr('data-points', correctnessBonus);
  $('#cover-max-combo').attr('data-points', comboBonus);

  $('#cover-linky').addClass('show');

  var dataToSend = {
    "score": _score,
	  "courseID" : $('#table-of-god').attr('data-course-id'),
  };

  $.ajax({
    url: "/Courses/Linky", // /kontroler/akcija (klasa/funkcija u klasi)
    method: "POST",
    data: dataToSend,
    success: function (res) {
      if (res.success) {
        //$('body').append('Waai uspesno!');
      } else {
        //$('body').append('Nije uspelo!');
      }
    }
  });
}

/**
 * Sta ce da se desi kada igracu istekne vreme.
 */
var timeOut = function() {
  gameOver();
  $('#cover-linky .additional').hide();
  $('#cover-linky .title').html('Vreme isteklo!');
  $('#cover-linky .subtitle').html('Predugo si se razmišljao. Probaj opet.');
}

/**
 * Sta ce da se desi kada postris sve zivote.
 */
var noMoreLives = function() {
  gameOver();
  $('#cover-linky .additional').hide();
  $('#cover-linky .title').html('Nemaš više života!');
  $('#cover-linky .subtitle').html('Napravio si 3 greške. Probaj opet.');
}

/**
 * Sta ce da se desi kad zavrsis igricu kako treba.
 */
var goodJob = function() {
  gameOver();
  $('#cover-linky .title').html('Uspešno odigrano!');
  $('#cover-linky .subtitle').html('Svaka čast!');

  var $score = $('#cover-linky .total-score');
  var $scoreSpan = $score.children('span');
  var score = parseInt($scoreSpan.text().trim());

  var livesBonus = parseInt($('#cover-remaining-lives').attr('data-points').trim());
  var correctBonus = parseInt($('#cover-correctness').attr('data-points').trim());
  var comboBonus = parseInt($('#cover-max-combo').attr('data-points').trim());

  setTimeout( function() { // Cekaj 500 ms pre nego sto krenes sa animacijama, da bi stigo da se pojavi #cover
    $("#cover-linky .additional > ul").animate({top: '-=48px'}, 1000, 'linear', function() {
      $score.addClass('boom');
      setTimeout( function() { // Cekaj 200ms (da se izvrsi animacija za boom; vidi css)
        $scoreSpan.text(score + livesBonus);
        $score.removeClass('boom');
      }, 200);
      setTimeout(function() { // Kad se prvi dodatni score pomeri, nemoj odmah da kreces dalje. Cekaj 300 ms.
        $("#cover-linky .additional > ul").animate({top: '-=48px'}, 1000, 'linear', function() {
          $score.addClass('boom');
          setTimeout( function() {
            $scoreSpan.text(score + livesBonus + correctBonus);
            $score.removeClass('boom');
          }, 200);
          setTimeout(function() {
            $("#cover-linky .additional > ul").animate({top: '-=48px'}, 1000, 'linear', function() {
              $score.addClass('boom');
              setTimeout( function() {
                $scoreSpan.text(score + livesBonus + correctBonus + comboBonus);
                $score.removeClass('boom');
              }, 200);
            });
          }, 300);
        });
      }, 300);
    });
  }, 500); // cekanje za #cover
}

/**
 * Kada se da tacan odgovor, poziva se ova funkcija da bi se povecalo vreme.
 */
var addTime = function() {
  _timeRemaining += 3000;

  // Ne dozvoljavamo da ima preko _maxTimeRemaining.
  if (_timeRemaining > _maxTimeRemaining) {
    _timeRemaining = _maxTimeRemaining;
  }
}

/**
 * Upisuje u ovo levo
 */
var updateScoreView = function() {
  var total = _incorrectCount + _correctCount
  var percentage = Math.floor(_correctCount * 100 / total);

  $('#cards-current-total').html(total);
  $('#accuracy').html(percentage + '%');
  $('#lives').html(_lives);
  $('#session-combo').html(_currentCombo);

  $('#score').html(_score);

  $('#base-score').html(_baseScore);
  $('#combo-bonus').html('+' + _bonusCombo);
  $('#time-bonus').html('+' + _bonusTime);
}

$('body').keypress(function(e) {

  // Saseci ga u startu ako igra nije dozvoljena.
  if (!ALLOW_GAME) return;

  var magic = 48;
  if (e.keyCode > magic && e.keyCode < magic + 10)
    //console.log('Numpad ' + (e.keyCode - magic) + ' pressed!');
  switch (e.keyCode) {
    case (magic + 1): selectChip(7); break;
    case (magic + 2): selectChip(8); break;
    case (magic + 3): selectChip(9); break;
    case (magic + 4): selectChip(4); break;
    case (magic + 5): selectChip(5); break;
    case (magic + 6): selectChip(6); break;
    case (magic + 7): selectChip(1); break;
    case (magic + 8): selectChip(2); break;
    case (magic + 9): selectChip(3); break;
  }
});

$('body').on('click', '.workspace .cards ul > li', function() {

  // Saseci ga u startu ako igra nije dozvoljena.
  if (!ALLOW_GAME) return;

  var index = $(this).index();

  switch (index) {
    case (1-1): selectChip(1); break;
    case (2-1): selectChip(2); break;
    case (3-1): selectChip(3); break;
    case (4-1): selectChip(4); break;
    case (5-1): selectChip(5); break;
    case (6-1): selectChip(6); break;
    case (7-1): selectChip(7); break;
    case (8-1): selectChip(8); break;
    case (9-1): selectChip(9); break;
  }
});

var getChipsLeft = function() {
  return (_qa.length - _nextChip);
}

var incrementChip = function() {
  _nextChip++;
  if (_nextChip >= _qa.length)
    _nextChip = _qa.length;
}

var pushNextChip = function(pos) {
  putNew(_nextChip, 'q', pos);
  _stack.push(_nextChip);
  incrementChip();
}

var selectChip = function(pos) {

  var chip = $('.cards ul li:nth-child(' + pos + ') .chip');

  if (chip.attr('data-id') == -1) return;

  if (_selected == 0) {
    // Dosad nije nista selektirano
    _selected = pos;
    chip.addClass('selected');
  } else {
    // Selektirano vec nesto, oceni ga itd
    if (_selected != pos) {
      // Nije opet isti selektiran
      var prevChip = $('.cards ul li:nth-child(' + _selected + ') .chip');
      if (prevChip.attr('data-id') == chip.attr('data-id')) {
        // Tacan odgovor
        //console.log("Tacan odgovor!");
        doSomething(_selected, pos);
        _correctCount++;
        _currentCombo++;
        if (_currentCombo > _maxCombo)
          _maxCombo = _currentCombo;
        _baseScore = 1;
        _bonusTime = Math.floor(_timeRemaining / 1000);
        _bonusCombo = Math.floor(_currentCombo / 5);
        _score += (_baseScore + _bonusTime + _bonusCombo);
        addTime();
        // Efekat
        chip.addClass('correct'); prevChip.addClass('correct');
        setTimeout(function() {
          chip.removeClass('correct');
          prevChip.removeClass('correct');
        }, 200);
      } else {
        // Netacan odgovor
        //console.log("Netacan odgovor!");
        _incorrectCount++;
        _currentCombo = 0;
        _baseScore = -1;
        _bonusTime = 0;
        _bonusCombo = 0;
        _score += (_baseScore + _bonusTime + _bonusCombo);
        if (--_lives == 0) {
          noMoreLives();
          return;
        }
        // Efekat
        chip.addClass('incorrect'); prevChip.addClass('incorrect');
        setTimeout(function() {
          chip.removeClass('incorrect');
          prevChip.removeClass('incorrect');
        }, 500);
      }
    }
    // Svakako (tacno ili ne, samo da je ocenjen)
    // makni sve selekte i apdejtuj
    _selected = 0;
    $('.cards ul li .chip').removeClass('selected');
    updateScoreView();
  }

  dump();
  if (isGameOver()) {
    goodJob();
  }
}

var putNew = function(qID, qa, pos) {
  try {
    _grid[pos] = qID;
    var thingToPut = (qa == 'q') ? _qa[qID].question : _qa[qID].answer;
    var chip = $('.cards ul li:nth-child(' + pos + ') .chip');
    chip.attr('data-id', qID).attr('data-qa', qa);
    chip.children('.old').remove();
    chip.children('.new').removeClass('new').addClass('old');
    chip.append('<div class="new"><span>' + thingToPut + '</span></div>');
  }
  catch (e) {
    putNothing(pos);
  }
}

var putNothing = function(pos) {
  var chip = $('.cards ul li:nth-child(' + pos + ') .chip');
  chip.attr('data-id', -1).attr('data-qa', 'q');

  chip.children('.old').remove();
  chip.children('.new').removeClass('new').addClass('old');
  chip.append('<div class="new"><span>' + '</span></div>'); // nista
}

var doMeh = function(pos1, pos2) {
  //console.log('Doing METH...');
  putNew(_stack.shift(), 'a', pos1);
  pushNextChip(pos2);
}

var doOhYes = function(pos1, pos2) {
  //console.log('Doing OHYES...');
  putNew(_stack.shift(), 'a', pos1);
  putNew(_stack.shift(), 'a', pos2);
}

var doOhNo = function(pos1, pos2) {
  //console.log('Doing OHNO...');
  pushNextChip(pos1);
  pushNextChip(pos2);
}

var doSomething = function(pos1, pos2) {
  var rand = getRandomArbitrary(0, 2);
  if (rand == 0) {
    doMeh(pos1, pos2);
  } else {
    rand = getRandomArbitrary(0, 2);
    if (rand == 0 && _stack.length <= 5 && getChipsLeft() >= 2) {
      doOhNo(pos1, pos2);
    } else if (rand == 1 && _stack.length >= 2) {
      doOhYes(pos1, pos2);
    } else {
      doMeh(pos1, pos2);
    }
  }
}

var dump = function() {
  //console.log(_grid);
  //console.log(_stack);
  //console.log("Selected: " + _selected);
  //console.log("Next chip: " + _nextChip);
}
