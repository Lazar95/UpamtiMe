var _course = []; // niz nivoa

var UNTOUCHED = 0;
var CHANGED = 1;
var NEW = 2;

var viewToData = function() {

  $('#course > li:not(#new-level) > ul').each(function() {

    var cardsInCurrLevel = [];

    $(this).children('li:not(.new-card)').each(function() {
      cardsInCurrLevel.push(
        {
          "cardID": $(this).attr('data-card-id'),
          "number": $(this).attr('data-card-number'),
          "question": $(this).find('.question').text(),
          "answer": $(this).find('.answer').text(),
          "description": $(this).find('.description').text(),
          "levelID": $(this).parent().attr('data-level-id'),
          "status": UNTOUCHED,
        }
      );
    }); // unutrasnji each

    _course.push(
      {
        "levelID": $(this).attr('data-level-id'),
        "name": $(this).children('span').html(),
        "number": $(this).attr('data-level-number'),
        "type": $(this).attr('data-level-type'),
        "cards": cardsInCurrLevel,
        "status": UNTOUCHED,
      }
    )

  }); // spoljni each

}


var _lastFakeCardID = 0;
var _lastFakeLevelID = 0;

var _dataToSend = {
  "courseID": 0,
  "name": "",
  "categoryID": 0,
  "subcategoryID": 0,
  "description": "",
  "deletedCards":  [], // lista ID-jeva kartica
  "deletedLevels": [], // lista Id-jeva nivoa
  "editedCards":   [], // niz objekata kartica (promenjene a postojale)
  "editedLevels":  [], // niz objekata nivoa (promenjen naziv ili redosled)
  "addedCards":    [], // nove kartice
  "addedLevels":   [], // nivo nivoi -- lazni ID (negativan)
}

/*****************************/
/*****************************/
/*****************************/
/*****************************/
/* Dodavanje nivoa i kartica */
/*****************************/
/*****************************/
/*****************************/
/*****************************/

var addLevel = function() {

  _lastFakeLevelID--;

  var newLevel = {
    "levelID": _lastFakeLevelID.toString(),
    "number": (_course.length + 1).toString(),
    "type": $('#new-level select option:selected').val(),
    "name": $('#new-level input').val().toString(),
    "cards": [],
    "status": NEW,
  }

  _course.push(newLevel);

  var string = '';
  string += '<li>';
    string += '<span>' + newLevel.name + '</span>';
    string += '<ul class="level" data-level-id="' + newLevel.levelID + '" data-level-number="' + newLevel.number + '" data-level-type="' + newLevel.type + '">';
      string += '<li class="new-card">';
        string += '<div class="inner-wrapper">';
          string += '<span>Pitanje:</span> <input class="question" type="text" />';
		      string += '<span>Odgovor:</span> <input class="answer" type="text" />';
			    string += '<span>Opis:</span> <input class="description" type="text" />';
        string += '</div>';
				string += '<div class="add-button">+</div>';
      string += '</li>';
    string += '</ul>';
  string += '</li>';

  $('#course > li:last-of-type').before(string);

}

var addCard = function(level) {

  _lastFakeCardID--;

  var newCard = {
    "cardID": _lastFakeCardID.toString(),
    "number": (_course[level - 1].cards.length + 1).toString(),
    "question": $('#course > li:nth-child(' + level + ') .new-card input.question').val(),
    "answer": $('#course > li:nth-child(' + level + ') .new-card input.answer').val(),
    "description": $('#course > li:nth-child(' + level + ') .new-card input.description').val(),
    "levelID": _course[level - 1].levelID.toString(),
    "status": NEW,
  }

  _course[level-1].cards.push(newCard);

  var string = '';
  string += '<li data-card-id="' + newCard.cardID + '" data-card-number="' + newCard.number + '">';
    string += '<div class="card-info">';
      string += '<span class="question">' + newCard.question + '</span>';
      string += '<span class="answer">' + newCard.answer + '</span>';
      string += '<span class="description">' + newCard.description + '</span>';
    string += '</div>';
    string += '<div class="buttons">';
      string += '<div class="change-button">E</div>';
      string += '<div class="remove-button">X</div>';
    string += '</div>';
  string += '</li>';

  $('#course > li:nth-of-type(' + level + ') ul > li:last-of-type').before(string);

}

/******************************/
/******************************/
/******************************/
/******************************/
/* Editovanje nivoa i kartica */
/******************************/
/******************************/
/******************************/
/******************************/

var editLevel = function(levelID) {

}


// Kad se klikne na Edit dugme prilikom editovanja kartice
//   - Zameni spanove inputima ali im sacuvaj trenutno stanje u data-old (za cancel)
$('#course').on('click', '.level .buttons .change-button', function() {
  var cardInfo = $(this).parent().parent().find('.card-info');
  var oldQ = cardInfo.children('span.question').html().trim();
  var oldA = cardInfo.children('span.answer').html().trim();
  var oldD = cardInfo.children('span.description').html().trim();
  cardInfo.children('span.question').remove();
  cardInfo.children('span.answer').remove();
  cardInfo.children('span.description').remove();
  cardInfo.append('<input type="text" class="question" data-old-value="' + oldQ + '" value="' + oldQ + '">');
  cardInfo.append('<input type="text" class="answer" data-old-value="' + oldA + '" value="' + oldA + '">');
  cardInfo.append('<input type="text" class="description" data-old-value="' + oldD + '" value="' + oldD + '">');
});

// Kad se klikne na Accept dugme prilikom editovanja kartice:
//   - Obradi edit (upisi u _course)
//   - Vrati spanove od inputa
$('#course').on('click', '.level .buttons .accept-button', function() {

  // Prikupljanje podataka
  var cardID = $(this).parent().parent().attr('data-card-id');
  var cardInfo = $(this).parent().parent().find('.card-info');
  var newQ = cardInfo.children('input.question').val().trim();
  var newA = cardInfo.children('input.answer').val().trim();
  var newD = cardInfo.children('input.description').val().trim();

  // Sustina:
  for (var level = 0; level < _course.length; level++) {
    for (var card = 0; card < _course[level].cards.length; card++) {
      if (_course[level].cards[card].cardID == cardID) {
        var currCard = _course[level].cards[card];
        // Nasili smo na karticu koja se edituje
        currCard.question = newQ;
        currCard.answer = newA;
        currCard.description = newD;
        if (currCard.status == NEW || currCard.status == CHANGED) {
          // Nemoj da radis nista...
          // Kartica koja je dodata a onda editovana je, sto se baze tice,
          // i dalje nova.
          // Kartica koje je editovana a onda je opet editovana je, sto se
          // baze tice, i dalje samo editovana.
        } else { // UNTOUCHED
          // Ako kartica nije dirana, onda je oznaci kao changed jer treba
          // da se prosledi bazi kao takva.
          currCard.status = CHANGED;
        }
      }
    }
  }

  // Vizuelno:
  cardInfo.children('input.question').remove();
  cardInfo.children('input.answer').remove();
  cardInfo.children('input.description').remove();
  cardInfo.append('<span class="question">'+     newQ + '</span>');
  cardInfo.append('<span class="answer">' +      newA + '</span>');
  cardInfo.append('<span class="description">' + newD + '</span>');
});


/****************************/
/****************************/
/****************************/
/****************************/
/* Brisanje nivoa i kartica */
/****************************/
/****************************/
/****************************/
/****************************/

var deleteLevel = function(levelID) {

}

var deleteCard = function(cardID) {

}

/*****************************/
/*****************************/
/*****************************/
/*****************************/
/* SAVE SAVE SAVE SAVE SAVE  */
/*****************************/
/*****************************/
/*****************************/
/*****************************/

var save = function() {

  // Osnovni podaci o kursu:
  _dataToSend.courseID = $('#course').attr('data-course-id');
  _dataToSend.name = $('#course-name > span').html();
  _dataToSend.categoryID = $('.cat-subcat #category option:selected').val();
  _dataToSend.subcategoryID = $('.cat-subcat #subcategory option:selected').val();
  _dataToSend.description = $('.basic-info > .description > span').html().trim();

  // Dodati celi nivoi:
  for (var i = 0; i < _course.length; i++) {
    if (_course[i].status == NEW) {
      _dataToSend.addedLevels.push(_course[i]);
    }
  }

  // Pojedinacne kartice:
  for (var i = 0; i < _course.length; i++) {
    if (_course[i].status != NEW) {
      for (var j = 0; j < _course[i].cards.length; j++) {
        // Sve nove kartice iz nivoa koji nisu novi (nedirani ili im je promenjeno ime)
        if (_course[i].cards[j].status == NEW) {
          _dataToSend.addedCards.push(_course[i].cards[j]);
        }
        // Sve kartice koje su vec u bazi a promenjene su:
        if (_course[i].cards[j].status == CHANGED) {
          _dataToSend.editedCards.push(_course[i].cards[j]);
        }
      }
    }
  }


  console.log(_dataToSend)

  $.ajax({
      url: "/Courses/EditCourse", // /kontroler/akcija (klasa/funkcija u klasi)
      method: "POST",
      data: _dataToSend,
      success: function (res) {
          if (res.success) {
              $('#new-level').append('Waai uspesno!');
          } else {
            $('#new-level').append('Nije uspelo!');
          }
      }
  });

}

/**
 * Bindovanje funkcija za dugmad
 */
$('#course').on('click', '#new-level .add-button', function() {
  if ($(this).attr('data-function') == 'expand') {
    $(this)
      .parent().removeClass('collapsed')
    .end().attr('data-function', 'add');
  } else {
    // data-function = "add"
    addLevel();
  }
});

$('#course').on('click', '.new-card .add-button', function() {
  if ($(this).attr('data-function') == 'expand') {
    $(this)
      .parent().removeClass('collapsed')
    .end().attr('data-function', 'add');
  } else {
    // data-function = "add"
    var index = $(this).parent().parent().parent().index();
    addCard(index+1);
  }
});

$('#save').click(save);


var dump = function() {
  console.log(_dataToSend);
}

$(document).ready(function() {
  viewToData();
});

/**
 * Dizajn
 */
