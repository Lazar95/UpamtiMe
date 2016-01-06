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
          "question": $(this).children('.question').text(),
          "answer": $(this).children('.answer').text(),
          "description": $(this).children('.description').text(),
          "level": $(this).parent().attr('data-level-id'),
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
  "deletedCards":  [], // lista ID-jeva kartica
  "deletedLevels": [], // lista Id-jeva nivoa
  "editedCards":   [], // niz objekata kartica (promenjene a postojale)
  "editedLevels":  [], // niz objekata nivoa (promenjen naziv ili redosled)
  "addedCards":    [], // nove kartice
  "addedLevels":   [], // nivo nivoi -- lazni ID (negativan)
}

/*****************************
 * Dodavanje nivoa i kartica *
 *****************************/

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
        string += 'Pitanje: <input class="question" type="text" />';
				string += 'Odgovor: <input class="answer" type="text" />';
				string += 'Odgovor: <input class="answer" type="text" />';
				string += '<div class="add-button">Dodaj!</div>';
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
    "level": _course[level - 1].levelID.toString(),
    "status": NEW,
  }

  _course[level-1].cards.push(newCard);

  var string = '';
  string += '<li data-card-id="' + newCard.cardID + '" data-card-number="' + newCard.number + '">';
    string += '<span class="question">' + newCard.question + '</span>';
    string += '<span class="answer">' + newCard.answer + '</span>';
    string += '<div class="change-button">Promeni</div>';
  string += '</li>';

  $('#course > li:nth-of-type(' + level + ') ul > li:last-of-type').before(string);

}

var save = function() {

  // Osnovni podaci o kursu:
  _dataToSend.courseID = $('#course').attr('data-course-id');
  _dataToSend.name = $('#course-name > span').html();
  _dataToSend.categoryID = $('.cat-subcat #category option:selected').val();
  _dataToSend.subcategoryID = $('.cat-subcat #subcategory option:selected').val();

  // Dodati celi nivoi:
  for (var i = 0; i < _course.length; i++) {
    if (_course[i].status == NEW) {
      _dataToSend.addedLevels.push(_course[i]);
    }
  }

  // Dodate pojedinacne kartice:
  for (var i = 0; i < _course.length; i++) {
    if (_course[i].status != NEW) {
      // Sve nove kartice iz nivoa koji nisu novi (nedirani ili im je promenjeno ime)
      for (var j = 0; j < _course[i].cards.length; j++) {
        if (_course[i].cards[j].status == NEW) {
          _dataToSend.addedCards.push(_course[i].cards[j]);
        }
      }
    }
  }

  console.log(_dataToSend)

  /*
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
  */
}

/**
 * Bindovanje funkcija za dugmad
 */
$('#course').on('click', '#new-level .add-button', addLevel);
$('#course').on('click', '.new-card .add-button', function() {
  var index = $(this).parent().parent().parent().index();
  addCard(index+1);
});
$('#save').click(save);


var dump = function() {
  console.log(_dataToSend);
}

$(document).ready(function() {
  viewToData();
});
