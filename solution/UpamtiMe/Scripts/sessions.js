/**
 * Vraca string sa obeleznim razlikama, za pogresena slova da se lakse vide.
 * @param  {string} correct tacan odgovor
 * @param  {string} typed   ukucan odgovor
 * @return {string}         <span class="obelezni">html</span>string
 */
var compareAndMarkDifference = function(correct, typed) {
    var marked; // ovde cemo da cuvamo obelezenu
    var beginMissingChar = '<span class="markMissingCharacter">';
    var beginWrongChar = '<span class="markWrongCharacter">';
    var beginExtraChar = '<span class="markExtraCharacter">';
    var beginAccentChar = '<span class="markAccentMistake">';
    var endColor = '</span>';

    var allowedForA = "aàáâãäå";
    var allowedForE = "eèéêë";
    var allowedForI = "iìíîï";
    var allowedForO = "oòóôõö";
    var allowedForU = "uùúûü";
    var mistakeThreshold = 2; // ono sto je ukucao ce da se obelezi/oboji samo ako odgovor ima <= mistakeThreshold gresaka

    var rows = typed.length + 1;
    var columns = correct.length + 1;

    var mat = [];
    for (var i = 0; i < rows; i++)
    {
        mat[i] = [];
        for (var j = 0; j < columns; j++) {
            mat[i][j] = 0;
        }
    }

    var insertedCharacters = []; // ovde smestamo samo indekse visak karaktera iz typed reci
    var removedCharacters = []; // ovde smestamo u parovima, indeks karaktera iz typed reci koji nije napisan, i indeks karaktera iz correct reci koji treba tu da dodje
    var changedCharacters = []; // ovde smestamo u parovima, indeks karaktera iz typed reci koji je pogresen, i indeks karaktera iz correct reci koji treba umesto pogresnog

    var mistakeType = []; // tipovi gresaka od kraja do pocetka stringa (znaci unazad), 1 - inserted, 2 - removed, 3 - changed

    // correct je source po Levenshtein algoritmu, a typed je target (cudno malo al logicnije se poklopi posle)
    // popunjavamo prvu kolonu brojevima od 0 do correct.length
    for (var i = 1; i < rows; i++)
        mat[i][0] = i;
    // popunjavamo prvu vrstu brojevima od 0 do typed.length
    for (var j = 1; j < columns; j++)
        mat[0][j] = j;
    for (var j = 1; j < columns; j++)
    {
        for (var i = 1; i < rows; i++)
        {
            if (typed.charAt(i-1) == correct.charAt(j-1))
                mat[i][j] = mat[i - 1][j - 1];                // isti karakter, upisujemo istu vrednost
            else
                mat[i][j] = Math.min(mat[i - 1][j] + 1,       // brisanje karaktera
                                     mat[i][j - 1] + 1,       // ubacivanje karaktera
                                     mat[i - 1][j - 1] + 1);  // zamena karaktera
        }
    }

    var i = rows - 1;
    var j = columns - 1;

    var typeIndex = 0;

    // prolazimo kroz matricu od dole-desno ka gore-levo i trazimo greske u otkucanom odgovoru
    while (i > 0 && j > 0)
    {
        if (mat[i - 1][j - 1] <= mat[i][j - 1]) {
            if (mat[i - 1][j - 1] <= mat[i - 1][j]) {
                if (mat[i - 1][j - 1] < mat[i][j]) // zamena, mat[i-1][j-1] je najmanje
                {
                    mistakeType[typeIndex++] = 3;
                    changedCharacters.push(i - 1); // na kom mestu u typed reci je omasio karakter
                    changedCharacters.push(j - 1); // koji karakter (po indeksu) iz correct reci treba umesto omasenog
                }
                i--;
                j--;
            }
            else { // mat[i-1][j] je najmanje
                mistakeType[typeIndex++] = 1;
                insertedCharacters.push(i - 1);
                i--;
            }
        }
        else
        {
            if (mat[i][j-1] <= mat[i-1][j])
            {
                mistakeType[typeIndex++] = 2;
                removedCharacters.push(i); // na kom mestu u typed reci treba da se posle umetne nesto
                removedCharacters.push(j - 1); // koji karakter (po indeksu) iz correct reci treba tu da se ubaci
                j--;
            }
            else
            {
                mistakeType[typeIndex++] = 1;
                insertedCharacters.push(i - 1);
                i--;
            }
        }
    }
    while (i > 0)
    {
        insertedCharacters.push(i - 1);
        mistakeType[typeIndex++] = 1;
        i--;
    }

    while (j > 0)
    {
        removedCharacters.push(i); // na kom mestu u typed reci treba da se posle umetne nesto
        removedCharacters.push(j - 1); // koji karakter (po indeksu) iz correct reci treba tu da se umetne
        mistakeType[typeIndex++] = 2;
        j--;
    }

    marked = typed;
    if (mistakeType.length > 0 && mistakeType.length <= mistakeThreshold) // ako ima gresaka nekih uopste, i ako ih <= mistakeThreshold
    {
        var i1 = 0; // indeks za kretanje kroz insertedCharacters niz
        var i2 = 0; // indeks za kretanje kroz removedCharacters niz
        var i3 = 0; // indeks za kretanje kroz changedCharacters niz
        for (var k = 0; k < mistakeType.length; k++)
        {
            switch (mistakeType[k])
            {
                case 1: // ovo znaci da je otkucao neko slovo vise, koje ce da mu se oboji samo
                    {
                        marked = marked.substring(0, insertedCharacters[i1]) + beginExtraChar + marked.substring(insertedCharacters[i1], insertedCharacters[i1] + 1) + endColor + marked.substring(insertedCharacters[i1] + 1, marked.length);
                        i1++;
                        break;
                    }
                case 2: // ovo znaci da je izostavio neko slovo, koje ce da mu se ubaci u nekoj boji
                    {
                        marked = marked.substring(0, removedCharacters[i2]) + beginMissingChar + correct.substring(removedCharacters[i2 + 1], removedCharacters[i2 + 1] + 1) + endColor + marked.substring(removedCharacters[i2], marked.length);
                        i2 += 2;
                        break;
                    }
                case 3: // ovo znaci da je zamenio jedno slovo drugim, obojicemo slovo koje je zeznuo
                    {
                        var wrong = marked[changedCharacters[i3]]; // pogresan karakter
                        var right = correct[changedCharacters[i3 + 1]]; // dobar karakter sa kojim treba da se ispravi pogresan
                        if ((allowedForA.indexOf(wrong) >= 0 && allowedForA.indexOf(right) >= 0) || // proveravamo ako je pogodio slovo ali nije akcenat
                            (allowedForE.indexOf(wrong) >= 0 && allowedForE.indexOf(right) >= 0) ||
                            (allowedForI.indexOf(wrong) >= 0 && allowedForI.indexOf(right) >= 0) ||
                            (allowedForO.indexOf(wrong) >= 0 && allowedForO.indexOf(right) >= 0) ||
                            (allowedForU.indexOf(wrong) >= 0 && allowedForU.indexOf(right) >= 0))
                        {
                            marked = marked.substring(0, changedCharacters[i3]) + beginAccentChar + wrong + endColor + marked.substring(changedCharacters[i3] + 1, marked.length);
                        }
                        else // ako program dodje do ovde i ne radi se o gresci u akcentu, onda je definitivno pogresen karakter
                        {
                            marked = marked.substring(0, changedCharacters[i3]) + beginWrongChar + wrong + endColor + marked.substring(changedCharacters[i3] + 1, marked.length);
                        }
                        i3 += 2;
                        break;
                    }
            }
        }
    }
    return marked; // vratimo oznacenu rec
}

function shuffleArray(array) {
    for (var i = array.length - 1; i > 0; i--) {
        var j = Math.floor(Math.random() * (i + 1));
        var temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
    return array;
}

var inheritsFrom = function (child, parent) {
    child.prototype = Object.create(parent.prototype);
};

// http://stackoverflow.com/a/966938/2131286
function createArray(length) {
  var arr = new Array(length || 0), i = length;
  if (arguments.length > 1) {
    var args = Array.prototype.slice.call(arguments, 1);
    while (i--) arr[length-1 - i] = createArray.apply(this, args);
  }
  return arr;
}

// Prozivoljna osnova za logaritam
function log(base, num) {
  return Math.log(num) / Math.log(base);
}

/*
createArray();     // [] or new Array()

createArray(2);    // new Array(2)

createArray(3, 2); // [new Array(2),
                   //  new Array(2),
                   //  new Array(2)]
*/


/******************************************************************************/
/******************************************************************************/
/******************************************************************************/
/**************************** class ScoreBreakdown ****************************/
/******************************************************************************/
/******************************************************************************/
/******************************************************************************/

var ScoreBreakdown = function() {
  this.baseScore = 0;
  this.cardComboMultiplier = 0;
  this.sessionComboMultiplier = 0;
  this.timeBonusMultiplier = 0;

  this.setBaseScore = function(card) {
    this.baseScore = (1 + 4 * log(262800, card.sinceSeen + 1));
  }

  this.setCardCombo = function(card) {
    if (card.combo == 0)
      this.cardComboMultiplier = 0;
    else
      this.cardComboMultiplier = log(2, card.combo);
  }

  this.setSessionCombo = function() {
    if (Session.getInstance().getStats().combo == 0)
      this.sessionComboMultiplier = 0;
    else
      this.sessionComboMultiplier = log(2, Session.getInstance().getStats().combo);
  }

  this.setTimeBonus = function(remaningCardTimeMiliseconds) {
    var remaningCardTimeSeconds = remaningCardTimeMiliseconds / 1000;
    this.timeBonusMultiplier = Math.pow(5, remaningCardTimeSeconds/20) - 1;
  }

  this.getScore = function() {
    var zagrada = this.cardComboMultiplier + this.sessionComboMultiplier + this.timeBonusMultiplier;
    return this.baseScore * (zagrada == 0 ? 1 : zagrada);
  }
}

/******************************************************************************/
/******************************************************************************/
/******************************************************************************/
/******************************** class Schedule ******************************/
/******************************************************************************/
/******************************************************************************/
/******************************************************************************/

var Schedule = function() {
  this.pointer = 0;
  this.schedule = []; // niz pokazivaca na kartice

  /**
   * Generates schedule.
   * @param  {Cards[]} cards Array of cards that the user will be tested on during the current session.
   */
  Schedule.prototype.generate = function() {
    // Pobrisi sve za slucaj da vec ima nesto:
    this.pointer = 0;
    this.schedule = [];
    // Generisanje:
    var temp = createArray(200, 200);
    var totalNumber = 0;
    for (var card = 0; card < Session.getInstance().cards.length; card++) {
      var currCard = Session.getInstance().cards[card];
      temp[card][0] = currCard;
      temp[card][1] = currCard.strategy.length;
      totalNumber += temp[card][1];
    }
    while (totalNumber > 0) {
      var min = 1, max = totalNumber;
      var rand = Math.floor(Math.random() * (max - min + 1) + min);
      var sum = 0;
      for (var i = 0; i < temp.length; i++) {
        sum += temp[i][1];
        if (rand <= sum) {
          this.schedule.push(temp[i][0]);
          temp[i][1]--;
          break;
        }
      }
      totalNumber--;
    }
    //console.table(this.schedule);
  };

  // Parametar: da li da kao sledeci challange prikaze postview?
  this.reschedule = function (showPostview) {
    // default parametar
    var showPostview = typeof showPostview !== 'undefined' ?  b : true;
    // reschedule odmah posle da bi video postview
    if (showPostview) this.schedule.splice(this.pointer + 1, 0, this.getCurrentCard());
    // reschedule na kraj jer moras da ponovis
    this.schedule.push(Session.getInstance().schedule.getCurrentCard());
    // u niz strategija za prikaz dodaj postview i onaj koji je pogresio opet
    var currStrategyIndex =  this.getCurrentCard().currStrategy;
    var postview = this.getCurrentCard().postviewStrategy;
    var currStrategy = this.getCurrentCard().getCurrentStrategy();
    if (showPostview) {
      this.getCurrentCard().strategy.splice(currStrategyIndex + 1, 0, currStrategy);
      this.getCurrentCard().strategy.splice(currStrategyIndex + 1, 0, postview);
    }
  };

  Schedule.prototype.advance = function () {
    this.pointer++;
    if (this.pointer >= this.schedule.length) {
      gameOver();
      return false;
    }
    Session.getInstance().promptChallange();
  };

  Schedule.prototype.getCurrentCard = function () {
    return this.schedule[this.pointer];
  };
}

/******************************************************************************/
/******************************************************************************/
/******************************************************************************/
/******************************** class Session *******************************/
/******************************************************************************/
/******************************************************************************/
/******************************************************************************/

// Singleton
var Session = (function() {

  var instance; // instanca cuva referencu za Singleton

  // inicijalizacija Singletona
  function init() {
    var combo = 0;
    var maxCombo = 0;
    var correctAnswers = 0;
    var wrongAnswers = 0;
    var score = 0;

    return {
      // Javne metode i promenljive
      lastScoreBreakdown: new ScoreBreakdown(),
      cards: [], // niz objekata tipa Card
      schedule: new Schedule(),
      setScore: function(newScore) {
        score = newScore;
      },
      getScore: function() {
        return score;
      },
      getGivenAnswers: function() {
        return correctAnswers + wrongAnswers;
      },
      getCorrectness: function() {
        var givenAnswers = this.getGivenAnswers();
        var ret;
        if (givenAnswers == 0) ret = 0; else ret = correctAnswers / givenAnswers;
        return ret;
      },
      incCombo: function() {
        combo++;
        if (combo > maxCombo) {
          maxCombo = combo;
        }
      },
      resetCombo: function () { combo = 0; },
      incCorrectAnswers: function() { correctAnswers++; },
      incWrongAnswers: function() { wrongAnswers++; },
      getStats: function() {
        return {
          combo: combo,
          maxCombo: maxCombo,
          givenAnswers: this.getGivenAnswers(),
          correctness: this.getCorrectness(),
        }
      },
      getTotalNumberOfChallanges: function() {
        var ret = 0;
        for (var i = 0; i < Session.getInstance().cards.length; ++i) {
          var currCard = Session.getInstance().cards[i];
          for (var j = 0; j < currCard.strategy.length; ++j)
          {
            if ((currCard.strategy[j] instanceof PreviewStrategy) || (currCard.strategy[j] instanceof PostviewStrategy)) continue;
            ret++;
          }
        }
        return ret;
      },
      begin: function() {
        this.promptChallange();
      },
      promptChallange: function() {
        console.log('Prompt Challange...');
        this.schedule.getCurrentCard().getCurrentStrategy().display();
      },
      addCard: function(newCard) {
        this.cards.push(newCard);
      },
    }
  } // inicijalizacija Singletona

  return {
    // vrati instancu singltona ako postoji
    // ako ne postoji, ili kreiraj novu
    getInstance: function() {
      if (!instance)
        instance = init();
      return instance;
    }
  }
})();

/******************************************************************************/
/******************************************************************************/
/******************************************************************************/
/********************************* class Card *********************************/
/******************************************************************************/
/******************************************************************************/
/******************************************************************************/

var Card = function(cardID, userCardID, question, answer, desc,
                    sinceSeen, sincePlan,
                    totalCorrectAnswers, totalWrongAnswers,
                    combo, goodness) {
  this.cardID = cardID;
  this.userCardID = userCardID;
  this.question = question;
  this.answer = answer;
  this.desc = desc;
  this.sinceSeen = sinceSeen == "" ? 0 : sinceSeen; //TODO bilo: lastSeenMinutes
  this.sincePlan = sincePlan == "" ? 0 : sincePlan; //TODO bilo: nextSeeMinutes
  this.newPlan = 0; // ovo racuna funkcija calculateNewPlan() na kraju sesije
  this.totalCorrectAnswers = totalCorrectAnswers == "" ? 0 : totalCorrectAnswers; // ukupno za karticu
  this.correctAnswers = 0; // za karticu u ovoj sesiji
  this.totalWrongAnswers = totalWrongAnswers == "" ? 0 : totalWrongAnswers; // ukupno za karticu
  this.wrongAnswers = 0; // za karticu u ovoj sesiji
  this.combo = combo == "" ? 0 : combo;
  this.goodness = goodness == "" ? 0 : goodness;
  this.strategy = [];
  this.currStrategy = 0;
  this.postviewStrategy = new PostviewStrategy();
  this.postviewStrategy.card = this;

  this.nextStrategy = function() {
    this.currStrategy++;
  }

  this.getCurrentStrategy = function() {
    return this.strategy[this.currStrategy];
  }
}

Card.prototype.addStrategy = function (newStrategy) {
  this.strategy.push(newStrategy);
  newStrategy.card = this;
};

Card.prototype.calculateNewPlan = function () {
  // Racunanje newPlan za kartice
  //
  // -----+----------+----------+---------+--------
  //      A    x     B     y    C    z    D
  //
  // A = trenutak kada je kartica poslednji put vidjena
  // B = trenutak za koji je kartica planirana da bude ponovo vidjena (ali nije tada nego u C)
  // C = sadasnji trenutak (igranje sesije)
  // D = trenutak za koji je kartica planirana da se ponovo vidi (mada znamo da najverovatnije nece tada nego malo kasnije)
  //
  // x = oldPlan -> ovo je vrednost koja je bila postavljena kao newPlan u sesiji kada je prethodni put ova kartica bila vidjena
  // y = sincePlan -> koliko je korisnik "zakasnio" da vidi karticu
  // x + y = sinceSeen -> koliko je proslo od kada je korisnik poslednji put video karticu
  // z = newPlan -> vreme koje postavljamo (koliko minuta treba da prodje dok se ne stigne u trenutak D?)
  //
  // newPlan = (oldPlan * 1.33 + sincePlan * 0.33) * goodness * (1 + combo / 10) * %tacnosti,
  //
  // gde je:
  //    oldPlan, sincePlan = goreopisani,
  //    goodness = goodness procitan sa kartice (nakon zavrsetka sesije),
  //    %tacnosti = correctAnswers / (totalAnswers), u okviru ove sesije za tu karticu

  if (this.sinceSeen == 0) {
    // Ako nikad dosad nije video ovu karticu, tj. ako je ovo sesija ucenja
    this.newPlan = 240; // h
  } else {
    var firstBracket = (this.sinceSeen - this.sincePlan) * 1.33 + this.sincePlan * 0.33;
    var secondBracket = 1 + this.combo / 10;
    var percentageWrongness = this.correctAnswers /  (this.correctAnswers + this.wrongAnswers);
    this.newPlan = Math.ceil(firstBracket * this.goodness * secondBracket * percentageWrongness);
  }
};

/******************************************************************************/
/******************************************************************************/
/******************************************************************************/
/******************************* class Strategy *******************************/
/******************************************************************************/
/******************************************************************************/
/******************************************************************************/

var Strategy = function(card) {
  this.indicator = 0; //TODO enumeracija: 0 =  neotvoreno, 1 = tacno, 2 = pogresno, 3 = skipped
  this.card = card;
}

Strategy.prototype.isCorrect = function (givenAnswer) {
  return (givenAnswer.toLowerCase().trim() == this.card.answer.toLowerCase().trim());
};

Strategy.prototype.submit = function(givenAnswer) {
  if (this.isCorrect(givenAnswer)) {
    this.correctAnswer(givenAnswer);
  } else {
    this.wrongAnswer(givenAnswer);
  }
}
$('.card.current-card').on('click', 'button#submit', function() {
  Session.getInstance().schedule.getCurrentCard().getCurrentStrategy().submit($('.card.current-card input').val());
});

Strategy.prototype.correctAnswer = function(givenAnswer) {
  $('.card.current-card').children().append('<div id="jingle-correct"><i class="fa fa-check"></i></div>');
  setTimeout(function() {
    $('#jingle-correct').remove();
  }, 750);

  this.indicator = 1; // znao
  var currCard = Session.getInstance().schedule.getCurrentCard();
  currCard.combo++;
  currCard.correctAnswers++;
  currCard.totalCorrectAnswers++;
  Session.getInstance().incCombo();
  Session.getInstance().incCorrectAnswers();
  currCard.goodness = 0.7 * currCard.goodness + (1 - 0.7) * 1;
  this.evaluateScore();
  updateSidebar();
  updateProgressbar();
  Session.getInstance().schedule.getCurrentCard().nextStrategy();
  Session.getInstance().schedule.advance();
};

Strategy.prototype.wrongAnswer = function(givenAnswer) {
  $('.card.current-card').children().append('<div id="jingle-wrong"><i class="fa fa-times"></i></div>');
  setTimeout(function() {
    $('#jingle-wrong').remove();
  }, 750);

  this.help(); // pozovi help da bi kad se sledeci put bude testirala kartica bilo lakse
  this.indicator = 2; // nije znao
  var currCard = Session.getInstance().schedule.getCurrentCard();
  currCard.combo = 0;
  currCard.wrongAnswers++;
  currCard.totalWrongAnswers++;
  Session.getInstance().resetCombo();
  Session.getInstance().incWrongAnswers();
  currCard.goodness = 0.7 * currCard.goodness;
  // this.evaluateScore(); // nema poena
  updateSidebar();
  updateProgressbar();
  currCard.postviewStrategy.givenAnswer = givenAnswer;
  Session.getInstance().schedule.reschedule();
  Session.getInstance().schedule.getCurrentCard().nextStrategy();
  Session.getInstance().schedule.advance();
};

// Pomoc za karticu (npr. dodavanje slova u hangman, biranje slova u scrabble)
Strategy.prototype.help = function() {
  // ne radi nista ako se ne predifinise
}

Strategy.prototype.skip = function() {
  //TODO
}

Strategy.prototype.evaluateScore = function() {
  var score = new ScoreBreakdown();
  //TODO sve ovo dole treba da moze da se psotavi direktno iz konstruktora klase ScoreBreakdown
  score.setBaseScore(this.card);
  score.setCardCombo(this.card);
  score.setSessionCombo();
  score.setTimeBonus(0); //TODO prosledi remaningCardTimeMiliseconds
  Session.getInstance().lastScoreBreakdown = score;
  var oldScore = Session.getInstance().getScore();
  Session.getInstance().setScore(oldScore + score.getScore());
}

Strategy.prototype.evaluateScoreZero = function() {
  var score = new ScoreBreakdown();
  //TODO sve ovo dole iz konstruktora
  score.setBaseScore(0);
  score.setCardCombo(0);
  score.setSessionCombo();
  score.setTimeBonus(0); //TODO prosledi remaningCardTimeSeconds
  Session.getInstance().lastScoreBreakdown = score;
  var oldScore = Session.getInstance().getScore();
  Session.getInstance().setScore(oldScore + score.getScore());
}

Strategy.prototype.display = function () {
  // animacija
  $('.card.current-card .challange:last-child').css('opacity', '0').css('bottom', '-20px');
  setTimeout(function() {
    $('.card.current-card .challange:last-child').animate({
      opacity: "1",
      bottom: "0",
    }, 200);
    $('.card.current-card .challange:not(:last-child)').animate({
      opacity: "0",
      bottom: "20",
    }, 200, function() {
      $('.card.current-card .challange:not(:last-child)').remove();
    });
  }, 300);
};

function MultipleChoiceStrategy(choices) {
  this.choices = choices; // niz stingova (ponudjeni odgovori)
  Strategy.call(this);
}
inheritsFrom(MultipleChoiceStrategy, Strategy);

// override
MultipleChoiceStrategy.prototype.display = function () {
  var $card = $('.card.current-card');
  var string = '';
  string += '<div id="multiple-choice" class="challange">';
    string += '<div><span>' + this.card.question + '</span></div>';
    string += '<div><ul>';
      for (var i = 0; i < this.choices.length; i++) {
        string += '<li>' + this.choices[i] + '</li>';
      }
    string += '</span></div>';
  string += '</div>';
  $card.children().append(string);
  Strategy.prototype.display.call(this);
};

MultipleChoiceStrategy.prototype.help = function () {
  shuffleArray(this.choices);
}

//TODO da se shuffluju odgovori za multiplechoice kad se omane

/*
MultipleChoiceStrategy.prototype.wrongAnswer = function(givenAnswer) {
  //this.help();
  this.indicator = 2; // nije znao
  var currCard = Session.getInstance().schedule.getCurrentCard();
  currCard.combo = 0;
  currCard.wrongAnswers++;
  Session.getInstance().resetCombo();
  currCard.goodness = 0.7 * currCard.goodness;
  this.evaluateScore();
  currCard.postviewStrategy.givenAnswer = givenAnswer;
  Session.getInstance().schedule.reschedule(false); // false
  //Session.getInstance().schedule.getCurrentCard().nextStrategy();
  //Session.getInstance().schedule.advance();
}
*/

// Odabir odgovora
var multipleChoicePickAnswer = function($el) {
  if ($el.hasClass('wrong')) return false; // da ga ne kaznimo dvaput ako slucajno klikne opet na pogresno
  var index = $el.index();
  var currCard = Session.getInstance().schedule.getCurrentCard();
  var givenAnswer = currCard.getCurrentStrategy().choices[index];
  if (currCard.getCurrentStrategy().isCorrect(givenAnswer)) {
    $el.addClass('correct');
  } else {
    $el.addClass('wrong');
  }
  currCard.getCurrentStrategy().submit(givenAnswer);
}
// Biranje odgovora tastaturom
$(window).keyup(function(e) {
  if (!$('#multiple-choice').length) return; // ni ne pokusavaj ako nije u toku challange multiple choice
  var $el;
  if ((e.keyCode >= 49 && e.keyCode <= 52)) { // number row
    $el = $('#multiple-choice li:nth-child(' + (e.keyCode - 48) + ')');
  }
  if ((e.keyCode >= 97 && e.keyCode <= 100)) { // numpad
    $el = $('#multiple-choice li:nth-child(' + (e.keyCode - 96) + ')');
  }
  multipleChoicePickAnswer($el);
});
// Klik na ogovor
$('.card.current-card').on('click', '#multiple-choice > div > ul > li', function() {
  multipleChoicePickAnswer($(this));
});

function HangmanStrategy(hints) {
  this.hints = hints; // string npr die Katze => d--_K--z-
  Strategy.call(this);
}
inheritsFrom(HangmanStrategy, Strategy);

// override
HangmanStrategy.prototype.display = function () {
  var $card = $('.card.current-card');
  var hints = this.hints.replace(/ /g, '&nbsp;');
  var string = '';
  string += '<div id="hangman" class="challange">';
    string += '<div><span>' + this.card.question + '</span></div>';
    string += '<div>';
      string += '<pre>' + hints + '<input type="text"></pre>';
    string += '</div>';
    string += '<div><button id="submit">Proveri odgovor</button></div>';
  string += '</div>';
  $card.children().append(string);
  $card.find('input').focus();
  Strategy.prototype.display.call(this);
};

HangmanStrategy.prototype.help = function () {
  //TODO JAJAC: dodaj jos neko slovo u this.hints
  // this.card.answer <-- tacan odgovor
  // this.hints <-- string dipa d--_K--z-; njega samo treba da promnis
  debugger;
  var hiddenChar = '_';
  var i;
  // nasumice trazimo indeks nekog skrivenog slova u reci da bi ga prikazali
  // i to samo ako vec sva slova nisu prikazana
  if (this.hints.indexOf(hiddenChar) != -1)
  {
    do {
      i = Math.floor(Math.random() * this.hints.length);
    } while (this.hints[i] != hiddenChar);

    // kada nadjemo indeks nekog skrivenog slova,
    // samo prikazemo sada i to slovo kao dodatni hint
    this.hints = this.hints.substr(0, i) + this.card.answer[i] + this.hints.substr(i + 1);
  }
};


function ScrabbleStrategy(letters) {
  this.letters = letters; // niz dostupnih slova za kucanje
  this.used = [];
  for (var i = 0; i < this.used.length; i++) this.used[i] = false;
  this.typed = []; // niz ukucanih indeksa
  Strategy.call(this);
}
inheritsFrom(ScrabbleStrategy, Strategy);

// override
ScrabbleStrategy.prototype.display = function () {
  // svaki put kad displayujemo moramo da rstarujemo stanje
  this.used = [];
  for (var i = 0; i < this.used.length; i++) this.used[i] = false;
  this.typed = [];
  var $card = $('.card.current-card');
  var string = '';
  string += '<div id="scrabble" class="challange">';
    string += '<div><span>' + this.card.question + '</span></div>';
    string += '<div>';
      string += '<input type="text">';
      string += '<ul>';
      for (var i = 0; i < this.letters.length; ++i) {
        string += '<li class="scrabble-clickable"><pre>' + this.letters[i] + '</pre></li>';
      }
      string += '</ul>';
    string += '</div>';
    string += '<div><button id="submit">Proveri odgovor</button></div>';
  string += '</div>';
  $card.children().append(string);
  $card.find('input').focus();
  Strategy.prototype.display.call(this);
};

ScrabbleStrategy.prototype.help = function() {
  //TODO JAJAC: izbaci neko slovo koje nije deo tacnog odgovora iz this.letters
  // this.card.answer <-- tacan odgovor
  // this.letters <-- niz dostupnih slova za kucanje
  // racunamo koliko u procentima ima ponudjenih slova viska
  var extraPercent = this.letters.length / this.card.answer.length - 1;

  var tempLetters = this.letters.slice(0); // kopiramo u tempLetters trenutno

  // u removeNum, u zavisnosti od prethodna dva parametra,
  // cuvamo izracunat broj reci koje cemo da sklonimo kao pomoc
  var removeNum;
  var removeCandidates = []; // ovde ce da smestimo indekse kandidata za remove

  // prvo iz pomocnog niza sklonimo sva slova koja cine odgovor
  // njih ne smemo da izbacimo iz this.letters
  for (var i = 0; i < this.card.answer.length; i++) {
    tempLetters.splice(tempLetters.indexOf(this.card.answer[i]), 1);
  }

  // kao pomoc cemo da sklonimo polovinu slova koja su visak
  removeNum = Math.ceil(tempLetters.length / 2);

  // i sad ovde sklonimo iz this.letters jos neko slovo
  for (var i = 0; i < removeNum; i++) {
    var rnd = Math.floor(Math.random() * tempLetters.length);
    this.letters.splice(this.letters.indexOf(tempLetters[rnd]), 1);
  }
  shuffleArray(this.letters);
}

ScrabbleStrategy.prototype.addLetterAt = function(i) {
  if (this.used[i]) return false; // ako je to slovo vec iskorisceno, sori bro
  this.used[i] = true;
  this.typed.push(i);
  updateScrabble();
  return true;
}
$('.card.current-card').on('click', '.scrabble-clickable', function() {
  var i = $(this).index();
  Session.getInstance().schedule.getCurrentCard().getCurrentStrategy().addLetterAt(i);
});

ScrabbleStrategy.prototype.addLetter = function(char) {
  for (var i = 0; i < this.letters.length; i++) {
    if (this.letters[i].toLowerCase() == char.toLowerCase() && !this.used[i]) {
      this.addLetterAt(i);
      return true;
    }
  }
  return false; // sori bro
}
$('.card.current-card').on('keypress click', '#scrabble input', function(e) {
  $(this).focus();
  e.preventDefault();
  var that = Session.getInstance().schedule.getCurrentCard().getCurrentStrategy();
  var char = String.fromCharCode(e.keyCode);
  that.addLetter(char);
  this.setSelectionRange(this.value.length, this.value.length); // ne damo mu da mrdne kursorom
  return true;
});
$('.card.current-card').on('keyup', '#scrabble input', function(e) {
  var that = Session.getInstance().schedule.getCurrentCard().getCurrentStrategy();
  if (e.keyCode == 8/*backspace*/) {
    that.removeLast();
  }
  this.setSelectionRange(this.value.length, this.value.length); // ne damo mu da mrdne kursorom
  return true;
});

ScrabbleStrategy.prototype.removeLetterAt = function(i) {
  if (!this.used[this.typed[i]]) return false; // ako to slovo nisi iskoristio, sori bro
  this.used[this.typed[i]] = false;
  this.typed.pop();
  updateScrabble();
  return true;
}

ScrabbleStrategy.prototype.removeLast = function() {
  this.removeLetterAt(this.typed.length - 1);
}

ScrabbleStrategy.prototype.getTypedString = function() {
  var string = '';
  for (var i = 0; i < this.typed.length; i++) {
    string += this.letters[this.typed[i]];
  }
  return string;
}

var updateScrabble = function() {
  var currentStrategy = Session.getInstance().schedule.getCurrentCard().getCurrentStrategy();
  var $ul = $('#scrabble ul');
  var $input = $('#scrabble input');
  $input.val(currentStrategy.getTypedString());
  for (var i = 0; i < currentStrategy.letters.length; i++) {
    var $li = $ul.children('li:nth-child(' + (i + 1) + ')');
    $li.removeClass('used');
    if (currentStrategy.used[i]) $li.addClass('used');
  }
}

function RealDealStrategy() {
  Strategy.call(this);
}
inheritsFrom(RealDealStrategy, Strategy);

// override
RealDealStrategy.prototype.display = function () {
  var $card = $('.card.current-card');
  var string = '';
  string += '<div id="real-deal" class="challange">';
    string += '<div><span>' + this.card.question + '</span></div>';
    string += '<div><input type="text"></div>';
    string += '<div><button id="submit">Proveri odgovor</button></div>';
  string += '</div>';
  $card.children().append(string);
  $card.find('input').focus();
  Strategy.prototype.display.call(this);
};

// nema help



function PreviewStrategy() {
  Strategy.call(this);
}
inheritsFrom(PreviewStrategy, Strategy);

// override
PreviewStrategy.prototype.display = function () {
  var $card = $('.card.current-card');
  var string = '';
  string += '<div id="preview" class="challange">';
    string += '<div><span>' + this.card.question + '</span></div>';
    string += '<div><span>' + this.card.answer + '</span></div>';
    string += '<div><button id="go-to-next">Nauči</button><button id="skip-to-last">Već znam</button></div>';
  string += '</div>';
  $card.children().append(string);
  $card.find('button#go-to-next').focus();
  Strategy.prototype.display.call(this);
};

$('.card.current-card').on('click', 'button#go-to-next', function() {
  Session.getInstance().schedule.getCurrentCard().nextStrategy();
  Session.getInstance().schedule.advance();
  updateSidebar();
  updateProgressbar();
});

$('.card.current-card').on('click', 'button#skip-to-last', function() {
  var currCard = Session.getInstance().schedule.getCurrentCard();
  var id = currCard.cardID;
  // obrisi sve strategije osim poslednje i prve (preview, realdeal) iz kartice
  currCard.strategy.splice(1, currCard.strategy.length - 2);
  // obrisi sve strategije posle sledece iz sesije
  var session = Session.getInstance();
  var i = session.schedule.pointer;
  var passedFirst = false;
  while (i < session.schedule.schedule.length - 1) {
    i++;
    if (session.schedule.schedule[i].cardID == id) {
      if (!passedFirst) {
        passedFirst = true;
      } else {
        // ako vec jesmo prosli jedan odpozadi, ukloni ga
        session.schedule.schedule.splice(i, 1);
        i--;
      }
    }
  }
  // stavi da sledeca strategija bude poslednja (tj druga al ajde):
  currCard.currStrategy = currCard.strategy.length - 1;
  // cepaj dalje kroz sesiju:
  Session.getInstance().schedule.advance();
  updateSidebar();
  updateProgressbar();
});


function PostviewStrategy() {
  this.givenAnswer = ''; // (pogresan) dati odgovor
  Strategy.call(this);
}
inheritsFrom(PostviewStrategy, Strategy);

// override
PostviewStrategy.prototype.display = function () {
  var $card = $('.card.current-card');
  var string = '';
  string += '<div id="postview" class="challange">';
    string += '<div><span>' + this.card.question + '</span></div>';
    string += '<div><span>' + compareAndMarkDifference(this.card.answer, this.givenAnswer) + '</span><span>' + this.card.answer + '</div>';
    string += '<div><button id="go-to-next">Dalje</button></div>';
  string += '</div>';
  $card.children().append(string);
  $card.find('button#go-to-next').focus();
  Strategy.prototype.display.call(this);
};




/******************************************************************************/
/******************************************************************************/
/******************************************************************************/
/*********************************** Observer *********************************/
/******************************************************************************/
/******************************************************************************/
/******************************************************************************/

var updateSidebar = function() {
  var stats = Session.getInstance().getStats();
  var givenAnswers = stats.givenAnswers;
  var totalQuestions = Session.getInstance().getTotalNumberOfChallanges();
  var correctness = (Math.round(stats.correctness.toFixed(4) * 100)) + '%';
  var combo = stats.combo;
  var maxCombo = stats.maxCombo;
  var lastScoreBreakdown = Session.getInstance().lastScoreBreakdown;

  var $general = $('.session-info .general');
  $general.find('#cards-current-total').attr('data-current', givenAnswers).attr('data-total', totalQuestions).text(givenAnswers + '/' + totalQuestions); //TODO
  $general.find('#accuracy').text(correctness);
  $general.find('#session-combo').text(combo);

  $('.session-info #last-score').text('+' + lastScoreBreakdown.getScore().toFixed(2));
  $('.session-info #score').text(Session.getInstance().getScore().toFixed(2));
  var $lastScoreBreakdown = $('.session-info .last-score-breakdown');
  $lastScoreBreakdown.find('#base-score').text(lastScoreBreakdown.baseScore.toFixed(2));
  $lastScoreBreakdown.find('#card-combo').text(lastScoreBreakdown.cardComboMultiplier.toFixed(2));
  $lastScoreBreakdown.find('#session-combo-multiplier').text(lastScoreBreakdown.sessionComboMultiplier.toFixed(2));
  $lastScoreBreakdown.find('#time-bonus').text(lastScoreBreakdown.timeBonusMultiplier.toFixed(2));
}

var updateProgressbar = function() {
  console.log('Progressbar update');
}

$('.card.current-card').on('keypress', 'input', function(e) {
  if (e.keyCode == 13) {
    var input = $('.card.current-card input').val();
    if (input != '') Session.getInstance().schedule.getCurrentCard().getCurrentStrategy().submit(input);
  }
});

/******************************************************************************/
/******************************************************************************/
/******************************************************************************/
/********************************** GameOver() ********************************/
/******************************************************************************/
/******************************************************************************/
/******************************************************************************/

var gameOver = function() {
  alert('GAME OVER');

  for (var i = 0; i < _session.cards.length; i++) {
    _session.cards[i].calculateNewPlan();
    _session.cards[i].sincePlan = _session.cards[i].newPlan;
    _session.cards[i].getCurrentStrategy = null;
    _session.cards[i].nextStrategy = null;
    _session.cards[i].postviewStrategy = null;
    _session.cards[i].strategy = null;
    _session.cards[i].addStrategy = null;
    _session.cards[i].calculateNewPlan = null;
  }

  console.table(_session.cards);

  // prikazi ono za kraj (nastavi/back to course)

  var dataToSend = {
    "qaInfo": Session.getInstance().cards,
    "score": Session.getInstance().getScore(),
    "courseID": $('#table-of-god').attr('data-course-id'),
  }

  var m_url = $('#table-of-god').attr('data-link');

  $.ajax({
    url: m_url, // /kontroler/akcija (klasa/funkcija u klasi)
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

/******************************************************************************/
/******************************************************************************/
/******************************************************************************/
/************************************* MAIN ***********************************/
/******************************************************************************/
/******************************************************************************/
/******************************************************************************/

// Sesija
var _session = Session.getInstance();

// Citanje podataka iz Table Of God
$('#table-of-god > tbody > tr').each(function() {
  var cardID = $(this).find('#table-of-god-card-id').text();
  var userCardID = $(this).find('#table-of-god-user-card-id').text();
  var question = $(this).find('#table-of-god-question').text();
  var answer = $(this).find('#table-of-god-answer').text();
  var desc = $(this).find('#table-of-god-description').text();
  var sinceSeen = $(this).find('#table-of-god-since-seen').text();
  var sincePlan = $(this).find('#table-of-god-since-plan').text();
  var totalCorrectAnswers = $(this).find('#table-of-god-total-correct-answers').text();
  var totalWrongAnswers = $(this).find('#table-of-god-total-wrong-answers').text();
  var combo = $(this).find('#table-of-god-combo').text();
  var goodness = $(this).find('#table-of-god-goodness').text();
  var mch1 = $(this).find('#table-of-god-multiple-choice-1').text();
  var mch2 = $(this).find('#table-of-god-multiple-choice-2').text();
  var mch3 = $(this).find('#table-of-god-multiple-choice-3').text();
  var mch4 = $(this).find('#table-of-god-multiple-choice-4').text();
  var hangman = $(this).find('#table-of-god-hangman').text();
  var scrabble = $(this).find('#table-of-god-scrabble').text().split('');

  var card = new Card(cardID, userCardID, question, answer, desc,
                      sinceSeen, sincePlan, totalCorrectAnswers, totalWrongAnswers, combo, goodness);

  var challenges = $(this).find('#table-of-god-challenges').text().split(';');
  var challengesLength = challenges.length;
  for (var i = 0; i < challengesLength; i++) {
    switch (challenges[i]) {
      case 'preview': card.addStrategy(new PreviewStrategy()); break;
      case 'multiple': card.addStrategy(new MultipleChoiceStrategy(new Array(mch1, mch2, mch3, mch4))); break;
      case 'hangman': card.addStrategy(new HangmanStrategy(hangman)); break;
      case 'scrabble': card.addStrategy(new ScrabbleStrategy(scrabble)); break;
      case 'realdeal': card.addStrategy(new RealDealStrategy()); break;
      default: card.addStrategy(new RealDealStrategy());
    }
  }

  _session.addCard(card);
});

// Generise schedule za sesiju
_session.schedule.generate();
console.table(_session.schedule.schedule);

//console.table(_session.cards);
updateSidebar();
updateProgressbar();

// Pustimo sesiju da krene
_session.begin();
