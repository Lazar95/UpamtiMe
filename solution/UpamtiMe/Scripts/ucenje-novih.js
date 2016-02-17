/**
 * Globalne promenljive pocinju donjom crtom, _camelCase.
 * Globalne promenljive koje u sustini sakrivaju implementaciju su CAPITALS.
 *
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

/**
 * http://stackoverflow.com/questions/17386774/
 * Ispravljeno da radi
 */
 function longestWord(string) {
    var str = string.split(" ");
    var longest = 0;
    var word = null;
    for (var i = 0; i < str.length; i++) {
        if (longest < str[i].length) {
            longest = str[i].length;
            word = str[i];
        }
    }
    return word;
}

// JSON za pitanja i odgovore
var parseTableOfGod = function() {
  table = $('#table-of-god');
  numberOfEntries = table.children('tbody').children('tr').length;

  for ( var i = 1; i <= numberOfEntries; i++ ) {
    var curr = table.children('tbody').children('tr:nth-child(' + i + ')');
    console.log(curr);
    _qa.push( {
      "status": -1,
      //TODO svasta nesto gledaj dole
      "cardID": curr.children('[data-type="card-id"]').text().trim(), // novo
      "question" : curr.children('[data-type="question"]').text().trim(),
      "answer" : curr.children('[data-type="answer"]').text().trim(),
      "description": curr.children('[data-type="description"]').text().trim(),
      "nextSeeMinutes": 0, // nije isto ako si u startu reko da znas rec
      "correctAnswers": 0, // uvek 0, vracam samo nov broj, odnosno akrtice radjene tokom ove sesije
      "wrongAnswers": 0, // uvek 0
      "combo": 0,
      "goodness": 0, // ja treba da sracunam goodness
    } );
  }

  console.log(_qa);
}

var _qa = [];
var LENGTH = _qa.length;
var ALREADY_PUNISHED = false;

$(window).bind('load', function() {
	
  sessionTimer(); // startujemo tajmer sesije 1s nakon loadovanja strane
  parseTableOfGod();
  LENGTH = _qa.length;
  setLevels();
  createProgressBar();


  //console.log('Broj pitanja: ', LENGTH);
  loadQuestion();

});

const UNOPENED = -1;
const SHOWN = 0;
const MULTIPLE_CHOICE_DONE = 1;
const HANGMAN_DONE = 2;
const SCRABBLE_DONE = 3;
const CORRECT_FIRST = 4;
const CORRECT_SECOND = 5;

const IGNORED = 7; // TODO

var _currentLevel = 0; // 0, 1, 2
var _questionPointer = 0; // pokazuje na pitanje u trenutnoj lekciji

var _currentPoints = 0;
var _lastPoints = 0;
var _currentCombo = 0;
var _maxComboReached = 0;

var _correctAnswers = 0;
var _wrongAnswers = 0;

const _partLength = 3;
const _sectionLength = 2 * _partLength;
var _levels = [[], [], []];

var setLevels = function() {
  for (var section = 0; section < Math.ceil(2 * LENGTH / _sectionLength); section++) {
    var sectionStart = section * _sectionLength;
    console.log("Section #" + section + " starts at index " + sectionStart + ".");
    for (var part = 0; part < 2; part++) {
      var partStart = sectionStart + part * _partLength;
      console.log("In section #" + section + ", part #" + part + " starts at index " + partStart + ".");
      for (var offset = 0; offset < _partLength; offset++)
        _levels[0].push(_partLength * section + offset);
    }
  }
  var numberOfSections = Math.ceil(2 * LENGTH / _sectionLength);
  var indexOfLastSession = (numberOfSections - 1) * _sectionLength;
  var expectedNumberOfQuestions = _partLength * numberOfSections;
  var removeLength = expectedNumberOfQuestions - LENGTH;
  _levels[0].splice(indexOfLastSession + _partLength + _partLength - removeLength, removeLength);
  _levels[0].splice(indexOfLastSession + _partLength - removeLength, removeLength);

  for (var i = 0; i < 2*LENGTH; i++) {
    _levels[1][i] = i % LENGTH;
    _levels[2][i] = i % LENGTH;
  }

  shuffleArray(_levels[1]);
  shuffleArray(_levels[2]);
}

// Za tajmer sesije
var seconds = 0, minutes = 0, t; // t je kao neki ID za setTimeout

var updateSessionTimer = function() {
    seconds++;
    if (seconds >= 60) {
        seconds = 0;
        minutes++;
        if (minutes >= 60) {
            minutes = 0;
        }
    }
    
    $('#time').html((minutes ? (minutes > 9 ? minutes : "0" + minutes) : "00") + "'" + (seconds > 9 ? seconds : "0" + seconds) + '"');
    sessionTimer();
}

// Pozivamo updateSessionTimer na svakih 1000ms
var sessionTimer = function() {
    t = setTimeout(updateSessionTimer, 1000);
}

// Zaustavljamo tajmer sesije
var stopSessionTimer = function() {
	clearTimeout(t);
}

var _currentQuestion = function() {
  return _levels[_currentLevel][_questionPointer];
}

var isLevelDone = function() {
  return _questionPointer == _levels[_currentLevel].length;
}

var isCorrect = function(questionNumber, givenAnswer) {
  return (_qa[questionNumber].answer.toUpperCase() == givenAnswer.toUpperCase());
}

var calculateSessionComboMultiplier = function(sessionCombo) {
  if (sessionCombo <= 4) return 1; // level-1
  else if (sessionCombo <= 9) return 1.25; // level-2
  else if (sessionCombo <= 14) return 1.5; //level-3
  else if (sessionCombo <= 19) return 1.75; //level-4
  else return 2; //level-5 (perfect!)
}

var evaluateStatsCorrect = function() {
  _correctAnswers++;
  _currentCombo++;
  if (_currentCombo > _maxComboReached)
    _maxComboReached = _currentCombo;

  var temp = $('#cards-current-total').attr('data-current');
  temp = parseInt(temp);
  $('#cards-current-total').attr('data-current', temp + 1);
  $('#cards-current-total').html((temp + 1) + '/' + $('#cards-current-total').attr('data-total'));
  temp = (100 * _correctAnswers) / (_correctAnswers + _wrongAnswers);
  $('#accuracy').html(temp.toFixed(0) + '%');
  $('#session-combo').html(_currentCombo);

  $('#base-score').html('1');
  _lastPoints = calculateSessionComboMultiplier(_currentCombo);
  $('#session-combo-multiplier').html(_lastPoints);
  $('#last-score').html(_lastPoints > 0 ? '+' + _lastPoints.toFixed(2) : _lastPoints.toFixed(2));
  _currentPoints += _lastPoints;
  $('#score').html(_currentPoints.toFixed(2));
}

var evaluateStatsWrong = function() {
  _wrongAnswers++;
  _currentCombo = 0;

  temp = (100 * _correctAnswers) / (_correctAnswers + _wrongAnswers);
  $('#accuracy').html(temp.toFixed(0) + '%');
  $('#session-combo').html(_currentCombo);

  $('#base-score').html('0');
  _lastPoints = 0;
  $('#session-combo-multiplier').html('1');
  $('#last-score').html('&pm; 0.00');
}

var setVariablesForNextQuestion = function() {
  _questionPointer++;
  if (isLevelDone()) {
    _currentLevel++;
    if (_currentLevel == 3) {
      sessionCompleted();
    }
    _questionPointer = 0;
  }
}

var scheduleAgain = function() {
  if (!ALREADY_PUNISHED)
    _levels[_currentLevel].push(_currentQuestion());
  ALREADY_PUNISHED = true;
}

var loadQuestion = function() {

  updateProgressBarCurrent();

  ALREADY_PUNISHED = false;

  // INITAL SHOW


  // MULTIPLE CHOICE
  // Mogu da budu ili (a) razlicite reci iz sesije ili (b) pretumbana slova.
  // (a) u slucaju kada je rec kratka (manje od 7 karaktera)
  // (b) u slucaju kada je rec dugacka (7 ili vise karaktera)

  if (longestWord(_qa[_currentQuestion()].answer).length < 7)
  {
    // (a)
    temp = [];
    for (var i = 0; i < LENGTH; i++) temp.push(_qa[i].answer);
    shuffleArray(temp);
    temp.splice(temp.indexOf(_qa[_currentQuestion()].answer), 1); // Iz niza pomesanih odgovora uklanjam tacan
    temp.unshift(_qa[_currentQuestion()].answer); // Dodajem tacan odgovor na pocetak niza
    var newTemp = temp.splice(0, 4); // Secem niz tako da ima samo 4 odgovora, medju njima je prvi tacan
    shuffleArray(newTemp); // Mesamo cetiri odgovora
    var multipleChoiceArray = newTemp;
  } else {
    // (b)
    // TODO jajac
    var multipleChoiceArray = shuffleArray([ _qa[_currentQuestion()].answer, 'lel', 'lel', 'lel' ]);
  }

  // HANGMAN
  $('.hangman ul').html('');
  for (var i = 0; i < _qa[_currentQuestion()].answer.length; i++)
    $('.hangman ul').append('<li><input type="text" maxlength="1"></li>');
  // Dodajem prvo slovo:
  $('.hangman ul li:nth-child(1) input')
    .attr('placeholder', _qa[_currentQuestion()].answer[0]);
  // Dodajem spejsove:
  for (var i = 0; i < _qa[_currentQuestion()].answer.length; i++) {
    if (_qa[_currentQuestion()].answer[i] == ' ')
      $('.hangman ul li:nth-child(' + (i+1) + ') input')
        .attr('placeholder','_');
  }

  // SCRABBLE
  // sklanjamo spejseve, zareze, itd: //TODO
  var ans = _qa[_currentQuestion()].answer;
  ans = ans.replace(/\s/g, '');
  ans = ans.toUpperCase();
  //console.log(ans);
  var chars = ans.split('');
  // Ostavi samo jednu kopiju svakog karaktera, ostavi prvu
  // http://stackoverflow.com/a/9229821/2131286
  var uniqueChars = chars.filter(function(item, pos) {
    return (chars.indexOf(item) == pos);
  });
  shuffleArray(uniqueChars);
  // Dodaj celu abecedu na kraj stringa
  for (var c = 65; c <= 90; c++) {
    uniqueChars.push(String.fromCharCode(c));
  }
  // Opet ostavi samo jednu kupiju
  var uniqueChars = uniqueChars.filter(function(item, pos) {
    return (uniqueChars.indexOf(item) == pos);
  });
  // Uzmi samo prvih 13 slova
  uniqueChars = uniqueChars.slice(0, 13);
  shuffleArray(uniqueChars);
  //console.log(uniqueChars);
  // Brojimo koliko nam kog slova treba i upisujemo u niz counts
  var count = [];
  for (var i = 0; i < 13; count[i++] = 0);
  for (var i = 0; i < ans.length; i++) {
    for (var j = 0; j < uniqueChars.length; j++)
      if (ans[i] == uniqueChars[j])
        count[j]++;
  }
  // Tu gde je ostalo 0 (slova koja ne postoje u reci), dodaj random broj;
  // najcesce je 1 ali moze da bude i 2 ili 3.
  for (var i = 0; i < count.length; i++) {
    if (count[i] == 0) {
      var temp = Math.floor((Math.random() * 10) + 1);
      switch (temp) {
        case 1: case 2: count[i] = 3; break;
        case 3: case 4: case 5: count[i] = 2; break;
        default: count[i] = 1;
      }
    }
  }
  //console.log(count);

  // REAL DEAL - CISTO KUCANJE


  // UBACIVANJE DOBIJENIH REZULTATA:
  // Standard
  $('.question span').html(_qa[_currentQuestion()].question);
  $('.correct-answer span').html(_qa[_currentQuestion()].answer);

  // Multiple Choice
  for (var i = 0; i < 4; i++)
    $('.multiple-choice li:nth-child(' + (i + 1) + ')').html(multipleChoiceArray[i]);

  // Scrabble
  $('#scrabble-typing').val('');
  $('#scrabble-typed').attr('data-plain-text', '');
  $('#scrabble-typed').html('');
  $('#scrabble-letters').children('li').remove();
  for (var i = 0; i < 13; i++) {
    // Lista slova sa ostalim neophodnim stvarima unutra:
    var string = '<li><div class="letter">' + uniqueChars[i] + '</div><div class="count"><div class="count-' + count[i] + '" data-count="' + count[i] + '" data-init-count="' +  count[i] + '">';
    for (var j = 1; j <= count[i]; j++)
      string += '<span>' + j + '</span>';
    string += '</div></div></li>';
    $('#scrabble-letters').append(string);
  }

  // Real Deal
  $('#type-answer').val('');

  switch (_qa[_currentQuestion()].status) {
    case UNOPENED:
      if (_currentLevel == 0) { displayShow(); } else { goToNextQuestion(); }
      break;
    case SHOWN:
      if (_currentLevel == 0) { displayMultipleChoice(); } else { goToNextQuestion(); }
      break;
    case MULTIPLE_CHOICE_DONE:
      if (_currentLevel == 1) { displayHangman(); } else { goToNextQuestion(); }
      break;
    case HANGMAN_DONE:
      if (_currentLevel == 1) { displayScrabble(); } else { goToNextQuestion(); }
      break;
    case SCRABBLE_DONE:
      if (_currentLevel == 2) { displayQuestionAndPrompt(); } else { goToNextQuestion(); }
      break;
    case CORRECT_FIRST:
      if (_currentLevel == 2) { displayQuestionAndPrompt(); } else { goToNextQuestion(); }
      break;
    default:
      goToNextQuestion();

  }


}

var goToNextQuestion = function() {
  setVariablesForNextQuestion();
  loadQuestion();
}

var hideAll = function() {
  $('.card-content .question').hide();
  $('.card-content .prompt').hide();
  $('.card-content .multiple-choice').hide();
  $('.card-content .multiple-choice li').removeClass();
  $('.card-content #virtual-typing').val('').removeClass().val('').hide();
  $('.card-content .hangman').hide();
  $('.card-content #scrabble').hide();
  $('.card-content .given-answer').hide();
  $('.card-content .correct-answer').hide();
  $('.card-content #initial-choice').hide();
  $('.card-content .next').hide();
}

var displayShow = function() {
  hideAll();
  $('.card-content .question').show();
  $('.card-content .correct-answer').show();
  $('.card-content #initial-choice').show().find('#yes button').focus();
}

var displayMultipleChoice = function() {
  hideAll();
  $('.card-content .question').show();
  $('.card-content .multiple-choice').show();
}

var displayHangman = function() {
  hideAll();
  $('.card-content .question').show();
  $('.card-content .hangman').show();
  $('.card-content .hangman ul li:first-child input').focus();
}

var displayScrabble = function() {
  hideAll();
  $('.card-content .question').show();
  $('#scrabble').show().children('input').focus();
}

var displayQuestionAndPrompt = function() {
  hideAll();
  $('.card-content .question').show();
  $('.card-content .prompt').show().children('input').focus();
}

var displayQuestionAndAnswer = function() {
  hideAll();
  $('.card-content .question').show();
  $('.card-content .given-answer').show();
  $('.card-content .correct-answer').show();
  $('.card-content .next').show().focus();
}

// Hvata globalne precice sa tastature
$(document).keyup(function(e) {

  // Ako se kuca u inputu, nemoj odavde da uhvatis keyCode. Na primer, ako se
  // ukljuci Virtuelno kucanje, kad se ukuca broj 1 to nece da selektira nista.
  if ($(e.target).is('input') || $(e.target).is('button'))
    return true;

  // Pali virtuelno kucanje
  if (e.ctrlKey && e.keyCode == 32) // CTRL + Space
    $('#virtual-typing').show().focus();

  switch (e.keyCode) {

    // Numbers 1-4 on numberrow or numpad => selecting the answer during
    // the Multiple Choice question
    case 49: case 97: // 1
      multipleChoiceSelect(1); break;
    case 50: case 98: // 2
      multipleChoiceSelect(2); break;
    case 51: case 99: // 3
      multipleChoiceSelect(3); break;
    case 52: case 100: // 4
      multipleChoiceSelect(4); break;

  }

});

$('button.next').click(goToNextQuestion);

/*****************************************************************************/
/*****************************************************************************/
                              /* INITIAL SHOW */
/*****************************************************************************/
/*****************************************************************************/

var iWantToLearn = function() {
  _qa[_currentQuestion()].status = SHOWN;
  goToNextQuestion();
}

var iDontWantToLearn = function() {
  _qa[_currentQuestion()].status = CORRECT_FIRST;
  updateProgressBarCorrect();
  goToNextQuestion();
}

$('#yes button').click(iWantToLearn);
$('#no button').click(iDontWantToLearn);


/*****************************************************************************/
/*****************************************************************************/
                             /* MULTIPLE CHOICE */
/*****************************************************************************/
/*****************************************************************************/


var multipleChoiceSelect = function(n) {
  var selected = $('.multiple-choice li:nth-child(' + n + ')');
  //selected.addClass('selected');
  //console.log('Izabran odgovor ' + n);
  if (isCorrect(_currentQuestion(), selected.html())) {
    // Izabran tacan odgovor:
    evaluateStatsCorrect();
    //console.log('Tacan odgovor!');
    if (!ALREADY_PUNISHED) {
      _qa[_currentQuestion()].status = MULTIPLE_CHOICE_DONE;
    }
    selected.addClass('correct');
    updateProgressBarCorrect();
    $('.card-content .next').show().focus();
  } else {
    // Izabran netacan odgovor.
    evaluateStatsWrong();
    //console.log('Netacan odgovor!');
    scheduleAgain();
    selected.addClass('wrong');
    updateProgressBarWrong();
    $('.multiple-choice #virtual-typing').val('');
  }
}

$('.multiple-choice li').click(function() {

  // Ako se klikne na odgovor na koji je vec kliknuto ranije (znaci bio je
  // netacan), ne dozvoli nista da se desi; smatra se kao misclick, nema
  // kaznenih poena.
  if ($(this).hasClass('wrong')) return;

  // Ako se klikne na odgovor a vec je selektiran tacan odgovor (ceka se
  // klik na dugme NEXT), nista nemoj da radis.
  if ($('.multiple-choice').find('li.correct').length != 0) return;

  index = $(this).index() + 1; // 1 2 3 4
  multipleChoiceSelect(index);
});

// Dok se kuca u #virtual-typing, selektiraju se slova u ponudjenim odgovirma.
$('.multiple-choice').on('keyup', '#virtual-typing', function(e) {

  if (e.keyCode == 13) return;

  var typed = $(this).val();
  var matches = [0, 0, 0, 0];
  $('.multiple-choice').children('li').removeClass('selected');
  for (var i = 1; i <= 4; i++) {
    var ans = $(this).parent().children('li:nth-child(' + i + ')').html();
    //console.log("Predimo " + typed + " sa " + ans + "...");
    if (typed.length > 0 && ans.substring(0, typed.length) == typed) {
      matches[i - 1] = 1;
      $('.multiple-choice li:nth-child(' + i + ')').removeClass().addClass("selected");
      //console.log('Match: ' + i);
    }

  }
  var sum = 0;
  for (var i = 0; i < matches.length; i++) {
    if (matches[i] == 1)
      sum++;
  }
  if (sum == 0) {
    $(this).removeClass().addClass("impossible");
  } else if (sum == 1) {
    $(this).removeClass().addClass("enough");
  } else {
    $(this).removeClass().addClass("incomplete");
  }
});

// Kad se pritisne Enter u #virtual-typing, ukoliko je selektiran samo jedan
// odgovor, taj odgovor se bira.
// Kad se pritisne Esc, virtuelno kucanje se gasi.
$('.multiple-choice').on('keypress', '#virtual-typing', function(e) {
  if (e.keyCode == 13 && $('.multiple-choice').find('li.selected').length == 1) {
    var index = $('.multiple-choice').find('li.selected').index() + 1;
    //alert(index);
    //console.log("Answer chosen through virtual typing: " + index);
    multipleChoiceSelect(index);
  }
});


/*****************************************************************************/
/*****************************************************************************/
                               /* HANGMAN */
/*****************************************************************************/
/*****************************************************************************/

var colorHangman = function() {
  $('.hangman ul li').each(function() {
    var currentInput = $(this).children('input');
    var currentIndex = $(this).index() + 1; // 1 2 3 ...
    if (currentInput.val() == '')
      $(this).removeClass();
    else {
      if (currentInput.val() == _qa[_currentQuestion()].answer[currentIndex - 1]) {
        $(this).addClass('correct');
      }
      if (currentInput.val() != _qa[_currentQuestion()].answer[currentIndex - 1]) {
        $(this).addClass('wrong');
        updateProgressBarWrong();
        scheduleAgain();
      }
    }
  });
}

$('.hangman').on('keyup', 'input', function(e) {
  colorHangman();
  // Proverava da li je zavrseno sve:
  var typedString = '';
  var i = _qa[_currentQuestion()].answer.length
  for ( ; i >= 1; i--) {
    var currentInput = $('.hangman li:nth-child(' + i + ')').children('input');
    if (currentInput.val() == '')
      break;
  }
  if (i == 0) {
    // Sve je ukucano:
    $('.hangman ul li').each(function() {
      typedString += $(this).children("input").val();
    });
    //console.log("Otkucana cela rec: " + typedString);
    if (isCorrect(_currentQuestion(), typedString)) {
      evaluateStatsCorrect();
      //console.log('Tacan odgovor!');
      if (!ALREADY_PUNISHED) {
        _qa[_currentQuestion()].status = HANGMAN_DONE;
      }
      updateProgressBarCorrect();
      $('.card-content .next').show().focus();
    } else {
      evaluateStatsWrong();
      updateProgressBarWrong();
      //console.log('Netacan odgovor!');
    }
  }

});

$('.hangman').on('keydown', 'input', function(e) {
  var letterIndex = $(this).parent().index() + 1; // 1 2 3 ...
  switch (e.keyCode) {
    case 8: // Backspace
      //console.log('Backspace');
      if($(this).val() == '')
        $('.hangman li:nth-child(' + (letterIndex - 1) + ') input').focus();
      break;
    case 13: // Enter
      e.preventDefault();
  }
  colorHangman(); // kad se drzi backspace da bi se lepse bojilo
});

$('.hangman').on('keypress', 'input', function(e) {
  var letterIndex = $(this).parent().index() + 1; // 1 2 3 ...
  //console.log('Ukucano slovo');
  $('.hangman li:nth-child(' + (letterIndex + 1) + ') input').focus();
});

/*****************************************************************************/
/*****************************************************************************/
                               /* SCRABBLE */
/*****************************************************************************/
/*****************************************************************************/

var scrabbleRevertAll = function() {

  // Vracamo sve na pocetno stanje
  $('#scrabble-typed').attr('data-plain-text', '');
  scrabbleDataToView();
  $('#scrabble-letters li').removeClass();
  $('#scrabble=letters li .count > div').removeClass();

  // Ukljucujuci i broj uzetih slova
  var countInnerElements = $('#scrabble-letters li .count > div');
  $.each(countInnerElements, function() {
    $(this)
      .attr('data-count', $(this).attr('data-init-count'))
      .removeClass()
      .addClass('count-' + $(this).attr('data-init-count'));
  });
}

var scrabbleFindElementContaining = function(letter) {
  return $('#scrabble-letters').find('li .letter:contains("' + letter + '")').parent();
}

var scrabbleDataToView = function() {
  var string = $('#scrabble-typed').attr('data-plain-text');
  $('#scrabble-typed').html(string.replace(/\s/g, '<span style="opacity: 0">t</span>'));
}

var scrabbleRemoveLast = function() {

  // Gledamo sta je trenutno otkucano
  var typed = $('#scrabble-typed').attr('data-plain-text');

  // Ako nije otkucano nista, ne radimo nista
  if (typed.length == 0) return;

  // Uzimamo poslednje dodato slovo
  var lastChar = typed.charAt(typed.length - 1);

  // Brisemo ga
  $('#scrabble-typed').attr('data-plain-text', typed.slice(0, typed.length - 1));

  // Trazimo element koji sadrzi to slovo
  var element = scrabbleFindElementContaining(lastChar);

  var dataCount = element.find('.count > div').attr('data-count');
  //console.log(dataCount);

  dataCount++;
  element.children('.count').children('div')
    .attr('data-count', dataCount)
    .removeClass()
    .addClass('count-' + dataCount);

  // Svakako skidamo klasu, ako nije ni postojala nikom nista
  element.removeClass('used-up');

  // Vracamo nazad iz spana u input
  $('#scrabble-typing').val($('#scrabble-typed').attr('data-plain-text'));

  // Update izgled
  scrabbleDataToView();

}

var scrabblePossibleToClick = function(letter) {
  var element = scrabbleFindElementContaining(letter);
  if (element === 'undefined')
    return false;

  if (element.find('.count > div').attr('data-count') == 0)
    return false;

  return true;
}

var scrabbleClick = function(element) {

  // Koje smo slovo izabrali?
  var letter = element.children('.letter').text();

  // Ako tog slova vise nema, ne dozvoljavamo da se unese
  if (element.children('.count').children('div').attr('data-count') == 0)
    return;

  // Upisujemo dodato slovo
  var clicked = $('#scrabble-typed').attr('data-plain-text') + letter;
  $('#scrabble-typed').attr('data-plain-text', clicked);
  //console.log(clicked + "|");

  var dataCount = element.children('.count').children('div').attr('data-count');
  var dataInitCount = element.children('.count').children('div').attr('data-init-count');

  dataCount--;
  element.children('.count').children('div')
    .attr('data-count', dataCount)
    .removeClass()
    .addClass('count-' + dataCount);

  // Dodatno, ako smo spali na nulu, obelezimo da smo uzeli sve
  if (dataCount == 0) {
    element.addClass('used-up');
  }

  // Vracamo nazad iz spana u input
  $('#scrabble-typing').val($('#scrabble-typed').attr('data-plain-text'));

  // Update izgled
  scrabbleDataToView();

}

var scrabbleSpace = function() {
  $('#scrabble-typed').attr('data-plain-text',$('#scrabble-typed').attr('data-plain-text') + ' ');

  // Vracamo nazad iz spana u input
  $('#scrabble-typing').val($('#scrabble-typed').attr('data-plain-text'));

  // Update izgled
  scrabbleDataToView();
}

var scrabbleEnter = function() {
  var given = $('#scrabble-typed').attr('data-plain-text');
  //console.log('Scrabble Enter: ' + given);
  if (isCorrect(_currentQuestion(), given)) {
    // Izabran tacan odgovor:
    evaluateStatsCorrect();
    //console.log('Tacan odgovor!');
    if (!ALREADY_PUNISHED) {
      _qa[_currentQuestion()].status = SCRABBLE_DONE;
    }
    updateProgressBarCorrect();
    $('.card-content .next').show().focus();
  } else {
    // Izabran netacan odgovor.
    evaluateStatsWrong();
    updateProgressBarWrong();
    //console.log('Netacan odgovor!');
    scheduleAgain();
  }
}

$('#scrabble-letters').on('click', 'li', function(e) {
  scrabbleClick($(this));
});

$('#scrabble-backspace').click(scrabbleRemoveLast);
$('#scrabble-space').click(scrabbleSpace);
$('#scrabble-enter').click(scrabbleEnter);

$('#scrabble-typing').keyup(function(e) {

  // Kad se pritisne enter za "next" od prethodnog pitanja, on se
  // uvhati i ovde... Zato se odgovor uopste razmatar u prihvatanje samo ako
  // je nesto ukucano. (Ovo svakako treba da se uradi, nevezano za uoceni bug).

  if (e.keyCode == 13 && $('#scrabble-typed').attr('data-plain-text') != '') {
    // Enter
    scrabbleEnter();
  }
});

$('#scrabble-typing').bind('input', function() {
  this.value = this.value.toUpperCase();

  // Ako u onome sto je trenutno ukucano imamo manje slova nego ono sto cuvamo
  // u data-plain-text, znaci da smo pritisnuli backspace.
  if (this.value.length < $('#scrabble-typed').attr('data-plain-text').length) {
    scrabbleRemoveLast();
    return;
  }
  // Uzimamo poslednje ukucano slovo
  var lastChar = this.value.charAt(this.value.length - 1);

  // Ako je ukucano slovo koje zapravo ne bi moglo da se klikne, odmah ga
  // ukloni
  if (!scrabblePossibleToClick(lastChar)) {
    this.value = this.value.slice(0, this.value.length - 1);
    return;
  }

  if (lastChar == ' ')
    scrabbleSpace();
  else
    scrabbleClick(scrabbleFindElementContaining(lastChar));
});


/*****************************************************************************/
/*****************************************************************************/
                               /* REAL DEAL */
/*****************************************************************************/
/*****************************************************************************/

var realDealCheckAnswer = function() {
  if (isCorrect(_currentQuestion(), $('#type-answer').val())) {
    evaluateStatsCorrect();
    // Izabran tacan odgovor:
    //console.log('Tacan odgovor!');
    if (_qa[_currentQuestion()].status == SCRABBLE_DONE) {
      // Ako prvi put vidi pitanje na ovom levelu
      _qa[_currentQuestion()].status = CORRECT_FIRST;
      updateProgressBarCorrect();
    } else {
      // Vec je jednom video pitanje ili je ovde skocio tako sto je objavio da
      // vec zna rec.
      _qa[_currentQuestion()].status = CORRECT_SECOND;
      updateProgressBarCorrect();

      // I sad upisujemo sve zivo:
      _qa[_currentQuestion()].nextSeeMinutes = 5; // 4h //TODO PAZI
      _qa[_currentQuestion()].correctAnswers = 1;
      _qa[_currentQuestion()].wrongAnswers = 0;
      _qa[_currentQuestion()].combo = 1;
      _qa[_currentQuestion()].goodness = 0.6; // jer eto
    }
    $('.card-content .next').show().focus();
  } else {
    // Izabran netacan odgovor.
    evaluateStatsWrong();
    //console.log('Netacan odgovor!');
    updateProgressBarWrong();
    scheduleAgain();
    displayQuestionAndAnswer();
  }
}

$('#type-answer').keyup(function(e) {
  if ($('#type-answer').val() != '' && e.keyCode == 13) {
    // Enter
    realDealCheckAnswer();
  }
});

// Stampanje za debug
var dump = function() {
  console.log('Trenutna lekcija: ' + _currentLevel);
  for (var l = 0; l < _levels.length; l++) {
    console.log('Level ' + l);
    for (var q= 0 ; q < _levels[l].length; q++) {
      if (q == _questionPointer) {
        console.log('[' + _levels[l][q] + ']');
      } else {
        console.log(_levels[l][q]);
      }
    }
  }
}

// Update Progress-Bar
var createProgressBar = function() {
  $pb = $('aside.progress-bar');
  string = '';
  string += '<ul>';
    for (var i = 0; i < _qa.length; i++) {
      string += '<div>';
        for (var j = 1; j <= 5; j++) string += '<li class="unopened"></li>';
      string += '</div>';
    }
  string += '</ul>';
  $pb.html(string);
}
var colorProgressBar = function(type, q, c) {
  // type = "correct", "wrong", "current"
  // q - koje pitanje
  // c - koji kruzic po redu
  var $c = $('aside.progress-bar > ul > div:nth-child(' + q + ') > li:nth-child(' + c + ')');
  if (type != "current") $c.removeClass('wrong').removeClass('correct');
  $c.addClass(type);
}
var updateProgressBarCorrect = function() {
  colorProgressBar('correct', _currentQuestion() + 1, getCircleNumber()-5);
  colorProgressBar('correct', _currentQuestion() + 1, getCircleNumber()-4);
  colorProgressBar('correct', _currentQuestion() + 1, getCircleNumber()-3);
  colorProgressBar('correct', _currentQuestion() + 1, getCircleNumber()-2);
  colorProgressBar('correct', _currentQuestion() + 1, getCircleNumber()-1);
  colorProgressBar('correct', _currentQuestion() + 1, getCircleNumber());
}
var updateProgressBarWrong = function() {
  colorProgressBar('wrong', _currentQuestion() + 1, getCircleNumber() + 1);
}
var updateProgressBarCurrent = function() {
  $('aside.progress-bar > ul > div > li').removeClass('current'); // sklonimo svuda prvo xD
  colorProgressBar('current', _currentQuestion() + 1, getCircleNumber() + 1);
}
var getCircleNumber = function() {
  var c = -1;
  switch (_qa[_currentQuestion()].status) {
    case CORRECT_SECOND: c = 5; break;
    case CORRECT_FIRST: c = 4; break;
    case SCRABBLE_DONE: c = 3; break;
    case HANGMAN_DONE: c = 2; break;
    case MULTIPLE_CHOICE_DONE: c = 1; break;
    case SHOWN: c = 0; break;
  }
  return c;
}

var sessionCompleted = function() {

  stopSessionTimer();

  $('#cover .title').html('Uspešno odigrano!');
  $('#cover .subtitle').html('Svaka čast!');

  $('#cover .total-score span').html(_currentPoints);
  $('#cover .total-score').attr('data-init-points', _currentPoints);

  $('#cover').addClass('show');

  var dataToSend = {
    "qaInfo": _qa,
    "score": _currentPoints,
	  "courseID" : $('#table-of-god').attr('data-course-id'),
  };
  console.log(dataToSend);

  $.ajax({
    url: "/Courses/Learn", // /kontroler/akcija (klasa/funkcija u klasi)
    method: "POST",
    data: dataToSend,
    success: function (res) {
      if (res.success) {
        $('body').append('Waai uspesno!');
      } else {
        $('body').append('Nije uspelo!');
      }
    }
  });
}
