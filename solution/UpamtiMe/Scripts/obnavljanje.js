/**
 * Globalne promenljive pocinju donjom crtom, _camelCase.
 * Globalne promenljive koje u sustini sakrivaju implementaciju su CAPITALS.
 *
 */

// JSON za pitanja i odgovore
var _qa = [];

var parseTableOfGod = function() {
  table = $('#table-of-god');
  numberOfEntries = table.children('tbody').children('tr').length;

  for ( var i = 1; i <= numberOfEntries; i++ ) {
    var curr = table.children('tbody').children('tr:nth-child(' + i + ')');
    _qa.push( {
      "status": -1,
      //TODO svasta nesto gledaj dole
      "userCardID": curr.children('[data-type="user-card-id"]').text().trim(), // novo
      "question" : curr.children('[data-type="question"]').text().trim(),
      "answer" : curr.children('[data-type="answer"]').text().trim(),
      "description": curr.children('[data-type="description"]').text().trim(),
      "lastSeenMinutes" : parseInt(curr.children('[data-type="last-seen-minutes"]').text().trim()),
      "nextSeeMinutes": parseInt(curr.children('[data-type="next-see-minutes"]').text().trim()),
      "totalCorrectAnswers": parseInt(curr.children('[data-type="correct-answers"]').text().trim()),
      "totalWrongAnswers": parseInt(curr.children('[data-type="wrong-answers"]').text().trim()),
      "correctAnswers": 0,
      "wrongAnswers": 0,
      "combo": parseInt(curr.children('[data-type="combo"]').text().trim()),
      "goodness": parseInt(curr.children('[data-type="goodness"]').text().trim()),
    } );
  }

  console.table(_qa);
}

var _qa = [];

$(window).bind('load', function() {
  parseTableOfGod();

  resetTimer();
  _interval = setInterval(function(){updateTimer(_timeRemaining);}, _timerUpdateInterval);

  // na osnovu broja pitanja napraviti kruzice dole
  var progress = '';
  for (var i = 0; i < _qa.length; i++) progress += '<li class="unopened"></li>';
  $('.progress-bar > ul').html(progress);

  displayCardData(_currentQuestion);
  displayProgress();
  displayQuestionAndPrompt();
  updateCurrentSessionInfo();
  $('input#type-answer').focus();
});

var UNOPENED = -1;
var CORRECT = 0;
var ONE_MISTAKE = 1;
var TWO_MISTAKE = 2;
var THREE_MISTAKE = 3;
var SKIPPED = 4;

var IGNORED = 5; // TODO

var _currentQuestion = 0;
var _currentPoints = 0;
var _lastPoints = 0;
var _currentCombo = 0;
var _maxComboReached = 0;

// Za tajmer
var _maxTimeRemaining = 20000;
var _timeRemaining = 20000;
var _timerUpdateInterval = 10;
var _interval;

/**
 * [function description]
 * @param  {[type]} timeRemaining [description]
 * @return {[type]}               [description]
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

  var remainingTime = $('#timer .remaining-time');

  // Smanji procentualno velicinu remaining-time u tajmeru
  remainingTime.css('width', (timeRemaining / _maxTimeRemaining * 100)  + '%');

  // Ispisi vreme
  remainingTime.children('span').html(seconds + '"&nbsp;' + milliseconds);

  // Smanji vreme
  _timeRemaining -= _timerUpdateInterval;

  // Ako je vreme isteklo, prestani da izvrsavas
  // TODO
  if (timeRemaining < 0) {
    clearInterval(_interval);
    remainingTime.children('span').html('');
  }

};

/**
 * Resetuj tajmer na pocetak (globalno definsiano).
 */
var resetTimer = function() {
  _timeRemaining = _maxTimeRemaining;
  updateTimer(_maxTimeRemaining);
};

var displayFinalMessage = function() {
  $('.current-card').html('');
  $('.next-card').css('flex-grow', '0.000000001').html('');
  $('.cards').css('flex-grow', '0.001');

  console.table(_qa);

  var dataToSend = {
    "qaInfo": _qa,
    "score": _currentPoints,
	   "courseID" : $('#table-of-god').attr('data-course-id'),
  };
  console.log(dataToSend);

  $.ajax({
    url: "/Courses/Review", // /kontroler/akcija (klasa/funkcija u klasi)
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

/**
 * Prikazuje pitanje sa rednim brojem koji je prosledjen kao parametar.
 * @param  {integer} questionNumber Redni broj pitanja, racunanje od 0.
 */
var displayCardData = function(questionNumber) {

  if (questionNumber == -1) {
    // Ako je sesija gotova
    $('.current-card').html('');
    displayFinalMessage();
    return;
  }

  $('.question span').html(_qa[questionNumber].question);
  $('.correct-answer span').html(_qa[questionNumber].answer);
  displayCurrentCardInfo();

  // Za prikazivanje sledeceg pitanja, virnemo sta ce da bude sledece pitanje
  temp = _currentQuestion; // cuvamo trenutno pitanje
  incQuestion();
  if (_currentQuestion == -1) {
    // Nema sledeceg pitanja, ovo je poslednje
    $('.next-card span').html('');
  } else {
    $('.next-card span').html(_qa[_currentQuestion].question);
  }
  _currentQuestion = temp; // vratimo trenutno pitanje
};

var displayQuestionAndPrompt = function() {
  $('.question').show();
  $('.prompt').show();
  $('.given-answer').hide();
  $('.correct-answer').hide();
  $('button.next').hide();
  $('.timer').show();
}

var displayQuestionAndAnswer = function() {
  $('.question').show();
  $('.prompt').hide();
  $('.given-answer').show();
  $('.correct-answer').show();
  $('button.next').show();
  $('.timer').hide();
}

/**
 * Prebrojava pitanja sa prosledjenim statusom.
 * @param  {integer} STATUS Odredjuje koja pitanja treba da se prebroje.
 * @return {integer}        Rezultat prebrojavanja.
 */
var countQuestions = function(STATUS) {
  var count = 0;
  for (var i = 0; i < _qa.length; i++) {
    if (_qa[i].status == STATUS)
      count++;
  }
  return count;
}

/**
 * Vraca prvo sledece pitanje na koje naidje na koje od korisnika treba da
 * se zahteva da odgovori (preskace pitanja na koja je vec dat tacan odgovor
 * i pitanja koja su obelezena kao skipped.
 * Vraca -1 ako su sva pitanja obidjena i nema dalje (kraj sesije).
 */
var incQuestion = function() {
  _currentQuestion = (_currentQuestion + 1) % _qa.length; // Mora najpre da ode sa trenutnog pitanja inace ce direktno da se tu zaglavi
  for (var brojProlaza = 1; brojProlaza <= _qa.length; brojProlaza++) {
    if (_qa[_currentQuestion].status != CORRECT && _qa[_currentQuestion].status != SKIPPED) {
      //alert(_currentQuestion);
      return;
    }
    _currentQuestion = (_currentQuestion + 1) % _qa.length;
  }
  _currentQuestion = -1;
}

/**
 * Vizuelni prikaz tacnih, entacnih, preskocenih, neotvorenih i trenutnog
 * pitanja na dnu ekrana koje se generise na osnovu sadasnjeg stanja JSON
 * objekta _qa svaki put kada se uradi bilo kakva akcija nad karticom.
 * @return {[type]} [description]
 */
var displayProgress = function() {
  var progressBar = $('.progress-bar ul');
  for (var i = 0; i < _qa.length; i++) {
    if (_qa[i].status == UNOPENED)
      progressBar.children('li:nth-child(' + (i+1) + ')').removeClass('correct').removeClass('incorrect').removeClass('skipped').removeClass('unopened').removeClass('current').addClass('unopened');
    if (_qa[i].status == CORRECT)
      progressBar.children('li:nth-child(' + (i+1) + ')').removeClass('correct').removeClass('incorrect').removeClass('skipped').removeClass('unopened').removeClass('current').addClass('correct');
    if (_qa[i].status == ONE_MISTAKE || _qa[i].status == TWO_MISTAKE || _qa[i].status == THREE_MISTAKE)
      progressBar.children('li:nth-child(' + (i+1) + ')').removeClass('correct').removeClass('incorrect').removeClass('skipped').removeClass('unopened').removeClass('current').addClass('incorrect');
    if (_qa[i].status == SKIPPED)
      progressBar.children('li:nth-child(' + (i+1) + ')').removeClass('correct').removeClass('incorrect').removeClass('skipped').removeClass('unopened').removeClass('current').addClass('skipped');
  }
  progressBar.children('li:nth-child(' + (_currentQuestion+1) + ')').removeClass('correct').removeClass('incorrect').removeClass('skipped').removeClass('unopened').removeClass('current').addClass('current');
}

var givenAnswerWas = function(STATE) {
  _qa[_currentQuestion].status = STATE;
  incQuestion();
  displayProgress();
}

/**
 * Da li dati dogovor treba prihvatiti kao tacan?
 * @param  {string} typedAnswer Odgovor koji je korisnik uneo.
 * @param  {string} realAnswer  Tacan odgovor sa kartice.
 * @return {boolean}            Vraca true ako je odgovor prihvacen, inace false.
 */
var answerAccepted = function(typedAnswer, realAnswer) {
  return typedAnswer.trim().toLowerCase() == realAnswer.trim().toLowerCase();
}

var calculateBaseScore = function(minutes) {
  if (minutes < 720) // 12 sati
    return 1;
  else if (minutes < 2880) // 2 dana
    return 2;
  else if (minutes < 5760) // 4 dana
    return 3;
  else if (minutes < 10080) // 1 nedelja
    return 4;
  else if (minutes < 20160) // 2 nedelje
    return 5;
  else if (minutes < 43800) // 1 mesec
    return 6;
  else if (minutes < 109500) // 2.5 meseca
    return 8;
  else // 6 meseci je 262800 minuta
    return 10;
}

var calculateTimeMultiplier = function(timeElapsed) {
  if (timeElapsed <= 1000) return 3; // level-5 (perfect)
  else if (timeElapsed <= 2000) return 2; // level-4
  else if (timeElapsed <= 3000) return 1.5; // level-3
  else if (timeElapsed <= 4000) return 1.25; // level-2
  else return 1; // level-1
}

/**
 * Racunanje "levela" multiplajera za vizuelni prikaz.
 * @param  {[type]} timeMultiplier [description]
 * @return {[type]}                [description]
 */
var calculateLevelTimeMultiplier = function(timeMultiplier) {
  switch(timeMultiplier) {
    case 3: return 5;
    case 2: return 4;
    case 1.5: return 3;
    case 1.25: return 2;
    default: return 1;
  }
}

var calculateSessionComboMultiplier = function(sessionCombo) {
  if (sessionCombo <= 4) return 1; // level-1
  else if (sessionCombo <= 9) return 1.25; // level-2
  else if (sessionCombo <= 14) return 1.5; //level-3
  else if (sessionCombo <= 19) return 1.75; //level-4
  else return 2; //level-5 (perfect!)
}

var calculateLevelSessionComboMultiplier = function(sessionComboMultiplier) {
  switch (sessionComboMultiplier) {
    case 2: return 5;
    case 1.75: return 4;
    case 1.5: return 3;
    case 1.25: return 2;
    default: return 1;
  }
}

var calculateCardComboMultiplier = function(cardCombo) {
  return cardCombo;
}

var calculateLevelCardComboMultiplier = function(cardComboMultiplier) {
  return cardComboMultiplier;
}


var evaluateAnswer = function(answer) {
  // Inicijalno, minuti za koliko cemo sledeci put da vidimo karticu
  // postavljaju se na isto onoliko koliko je proslo od kada smo je poslednji
  // put videli.
  _qa[_currentQuestion].nextSeeMinutes = _qa[_currentQuestion].lastSeenMinutes;

  // JSON struktura koja se vraca
  var eval = {
    "baseScore": 0,
    "timeMultiplier": 1,
    "sessionComboMultiplier": 1,
    "cardComboMultiplier": 1,
  }

  // Cuvamo da bismo mogli i da oduzmemo poene sa multiplierom (kad se pogresi).
  tempSessionComboMultiplier = _currentCombo;
  tempCardComboMultiplier = _qa[_currentQuestion].combo;

  if (!answerAccepted(answer, _qa[_currentQuestion].answer)) {
    // Dat je pogresan odgovor:
    // Lomi se kombo
    _qa[_currentQuestion].wrongAnswers++;
    _qa[_currentQuestion].totalWrongAnswers++;
    _qa[_currentQuestion].combo = 0; // combo na kartici
    _currentCombo = 0; // combo na sesiji

    // Postavljamo odgovarajuci status u zavisnosti od toga da li je prvi put
    // nacinjena greska (ide sa UNOPENED na ONE_MISTAKE), drugi (sa ONE na
    // TWO), treci (sa TWO na THREE) i ako se cetvrti put pogresi, kartica se
    // obelezava kao odlozena (sa THREE na SKIPPED).
    if (_qa[_currentQuestion].status == UNOPENED) {
      _qa[_currentQuestion].status = ONE_MISTAKE;
      _qa[_currentQuestion].nextSeeMinutes *= 0.33;
      console.log("Pitanje #" + _currentQuestion + ": Nacinjena prva greska.");
    }
    else if (_qa[_currentQuestion].status == ONE_MISTAKE) {
      _qa[_currentQuestion].status = TWO_MISTAKE;
      _qa[_currentQuestion].nextSeeMinutes *= 0.33;
      console.log("Pitanje #" + _currentQuestion + ": Nacinjena druga greska. Moraces dvaput da odgovoris tacno da bi ga zavrsio.");
    }
    else if (_qa[_currentQuestion].status == TWO_MISTAKE) {
      _qa[_currentQuestion].status = THREE_MISTAKE;
      _qa[_currentQuestion].nextSeeMinutes *= 0.33;
      console.log("Pitanje #" + _currentQuestion + ": Nacinjena treca greska. Moraces triput da odgovoris tacno da bi ga zavrsio. Ako opet pogresis, kartica se odlaze.");
    }
    else if (_qa[_currentQuestion].status == THREE_MISTAKE) {
      _qa[_currentQuestion].status = SKIPPED;
      _qa[_currentQuestion].nextSeeMinutes *= 240; // 4h
      console.log("Pitanje #" + _currentQuestion + ": Nacinjena cetvrta greska. Kartica odlozena za 6h.");
    }

    eval.baseScore = -1;
    eval.timeMultiplier = 1;
    eval.sessionComboMultiplier = calculateSessionComboMultiplier(tempSessionComboMultiplier + 1);
    eval.cardComboMultiplier = calculateCardComboMultiplier(tempCardComboMultiplier + 1);

  } else {
    // Tacan odgovor:
    _qa[_currentQuestion].correctAnswers++;
    _qa[_currentQuestion].totalCorrectAnswers++;
    _qa[_currentQuestion].combo++; // combo nad karticom
    _currentCombo++; // combo nad sesijom
    // Proveravamo da li je nadmasen najbolji kombo u sesiji: TODO nzm sto ne radi
    // _maxComboReached = (_currentCombo > _maxComboReached) : _currentCombo ? _maxComboReached;

    if (_qa[_currentQuestion].status == UNOPENED) {
      _qa[_currentQuestion].status = CORRECT;
      _qa[_currentQuestion].nextSeeMinutes *= 1.5;
      console.log("Pitanje #" + _currentQuestion + ": Odgovoreno tacno isprve.");
    }
    else if (_qa[_currentQuestion].status == ONE_MISTAKE) {
      _qa[_currentQuestion].status = CORRECT;
      console.log("Pitanje #" + _currentQuestion + ": Odgovoreno tacno nakon jedne greske.");
    }
    else if (_qa[_currentQuestion].status == TWO_MISTAKE) {
      _qa[_currentQuestion].status = ONE_MISTAKE;
      console.log("Pitanje #" + _currentQuestion + ": Odgovoreno tacno nakon dve greske, moras jos jednom.");
    }
    else if (_qa[_currentQuestion].status == THREE_MISTAKE) {
      _qa[_currentQuestion].status = TWO_MISTAKE;
      console.log("Pitanje #" + _currentQuestion + ": Odgovoreno tacno nakon tri greske, moras jos dvaput.");
    }

    eval.baseScore = calculateBaseScore(_qa[_currentQuestion].lastSeenMinutes);
    eval.timeMultiplier = calculateTimeMultiplier(_maxTimeRemaining - _timeRemaining);
    eval.sessionComboMultiplier = calculateSessionComboMultiplier(_currentCombo);
    eval.cardComboMultiplier = calculateCardComboMultiplier(_qa[_currentQuestion].combo);

  }

  _qa[_currentQuestion].nextSeeMinutes = Math.ceil(_qa[_currentQuestion].nextSeeMinutes);
  _qa[_currentQuestion].goodness = _qa[_currentQuestion].goodness * 0.3 + 0.7;

  _lastPoints = eval.baseScore * eval.timeMultiplier * eval.sessionComboMultiplier * eval.cardComboMultiplier;
  _currentPoints += _lastPoints;
  return eval;
}

var updateCurrentSessionInfo = function() {
  var openedCards = _qa.length - countQuestions(UNOPENED);
  $('#cards-current-total').html(countQuestions(CORRECT) + '/' + _qa.length);
  $('#accuracy').html(Math.floor(100 * countQuestions(CORRECT) / openedCards) + '%');
  $('#session-combo').html(_currentCombo);
  $('#score').html(_currentPoints.toFixed(2));
  if (_lastPoints > 0 ) $('#last-score').html('+' + _lastPoints.toFixed(2));
  else if (_lastPoints == 0) $('#last-score').html('&plusmn;' + _lastPoints.toFixed(2))
  else $('#last-score').html(_lastPoints.toFixed(2));
}

var displayCurrentCardInfo = function() {
  $('.card-info #card-last-seen').html('pre ' + _qa[_currentQuestion].lastSeenMinutes + ' minuta');
  $('.card-info #card-combo').html(_qa[_currentQuestion].combo);
}

var displayScoreBreakdown = function(eval) {
  $('.last-score-breakdown #base-score').html(eval.baseScore);
  $('.last-score-breakdown #card-combo').html('×' + eval.cardComboMultiplier).removeClass().addClass('level-' + calculateCardComboMultiplier(eval.cardComboMultiplier));
  $('.last-score-breakdown #session-combo').html('×' + eval.sessionComboMultiplier).removeClass().addClass('level-' + calculateLevelSessionComboMultiplier(eval.sessionComboMultiplier));
  $('.last-score-breakdown #time-bonus').html('×' + eval.timeMultiplier).removeClass().addClass('level-' + calculateLevelTimeMultiplier(eval.timeMultiplier)); // ne pokaze se perfect na uzastopni x5, ni timeout ne resava najbolje
}

var displayAll = function() {
  displayCardData(_currentQuestion);
  displayProgress();
  updateCurrentSessionInfo();
  displayCurrentCardInfo();
  resetTimer();
}

var loadNextQuestion = function() {
  incQuestion();
  displayAll();
  displayQuestionAndPrompt();
  $('input#type-answer').focus();
}

$('button.next').click(function() {
  loadNextQuestion();
});

$('input').bind("enterKey",function(e){
  // Ne moze tokom prve 3 sekunde da se pritisne samo Enter, a da se nista
  // nije ukucalo. Ovime se sprecava da korisnik slucajno pritisne dvaputa
  // Enter.
  // ^ Taj razlog cemo da napisemo kao glavni (svakako sam hteo da ga ubacimo),
  // ali sam ga zapravo dodao jer ima bag nakon sto se dugme "next" pozove
  // pritiskom na Enter. Onda se keydown registruje za dugme a keyup za input
  // pa se stalno preskaca. Bag i dalje ostaje ako neko pritisne next pomocu
  // Enter, pa druzi dugme preko 3 sekunde i onda ga pusti. Ali ovo vise nije
  // bag, ne interesuje me budala koja drzi dugme toliko dugo.
  if ($(this).val() == 0 && _timeRemaining > 17000) return;

  var eval = evaluateAnswer($(this).val());
  $('.given-answer span').html($(this).val()); // TODO ako je jedno slovo omanuto, oboji ga itd.
  $(this).val('');
  if (eval.baseScore == -1) {
    // Netacan odgovor.
    displayQuestionAndAnswer();
    $('button.next').focus();
  } else {
    // Tacan odgovor.
    displayQuestionAndPrompt();
    loadNextQuestion();
  }
  displayAll();
  displayScoreBreakdown(eval);
});
$('input').keyup(function(e){
  if(e.keyCode == 13) {
    $(this).trigger("enterKey");
  }
});
