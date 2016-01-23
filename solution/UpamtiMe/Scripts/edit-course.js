var _course = []; // niz nivoa

var UNTOUCHED = 0;
var CHANGED = 1;
var NEW = 2;
var DELETED = 3;

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
          "prevStatus": -1,
        }
      );
    }); // unutrasnji each

    _course.push(
      {
        "levelID": $(this).attr('data-level-id'),
        "name": $(this).parent().find('.level-name').children('span').html(),
        "number": $(this).attr('data-level-number'),
        "type": $(this).attr('data-level-type'),
        "cards": cardsInCurrLevel,
        "icon": $(this).parent().find('.icon-picker-button').attr('data-icon-id'),
        "color": $(this).parent().find('.icon-picker-button').attr('data-color-id'),
        "status": UNTOUCHED,
        "prevStatus": -1,
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

var countWhatInWhere = function(STATUS, levelID) {
  var count = 0;
  for (var lvl = 0; lvl < _course.length; lvl++) {
    var currLevel = _course[lvl];
    if (currLevel.levelID == levelID) {
      for (card = 0; card < currLevel.cards.length; card++) {
        currCard = currLevel.cards[card];
        if (currCard.status == STATUS) {
          count++;
        }
      }
      break;
    }
  }
  return count;
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

    string += '<div class="level-info">';
      string += '<div class="level-name">';
        string += '<div class="toggle-button"><i class="fa fa-plus"></i></div>';
        string += '<span>' + newLevel.name + '</span>';
      string += '</div>';
      string += '<div class="buttons">';
        string += '<div class="options-button"><i class="fa fa-cog"></i></div>';
      string += '</div>';
    string += '</div>';

    string += '<ul class="level" data-level-id="' + newLevel.levelID + '" data-level-number="' + newLevel.number + '" data-level-type="' + newLevel.type + '">';
      string += '<li class="new-card collapsed">';
        string += '<div class="inner-wrapper">';
          string += '<div><span>Pitanje:</span> <input class="question" type="text" /></div>';
  	      string += '<div><span>Odgovor:</span> <input class="answer" type="text" /></div>';
  		    string += '<div><span>Opis:</span> <input class="description" type="text" /></div>';
        string += '</div>';
  			string += '<div class="add-button" data-function="expand"><i class="fa fa-plus"></i></div>';
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
      string += '<div class="change-button"><i class="fa fa-pencil"></i></div>';
      string += '<div class="remove-button"><i class="fa fa-trash"></i></div>';
      string += '<div class="accept-button"><i class="fa fa-check"></i></div>';
      string += '<div class="discard-button"><i class="fa fa-times"></i></div>';
      string += '<div class="undo-button"><i class="fa fa-undo"></i></div>';
    string += '</div>';
  string += '</li>';

  $('#course > li:nth-of-type(' + level + ') ul > li:last-of-type').before(string);

}

/******************************/
/******************************/
/******************************/
/******************************/
/***** Editovanje kartica *****/
/******************************/
/******************************/
/******************************/
/******************************/

// Kad se klikne na Edit dugme prilikom editovanja kartice
//   - Zameni spanove inputima ali im sacuvaj trenutno stanje u data-old (za cancel)
var onEditButtonClick = function(button) {
  hideAllButtons(button.parent());
  button.parent().children('.accept-button').show();
  button.parent().children('.discard-button').show();

  var cardInfo = button.parent().parent().find('.card-info');
  var oldQ = cardInfo.children('span.question').html().trim();
  var oldA = cardInfo.children('span.answer').html().trim();
  var oldD = cardInfo.children('span.description').html().trim();
  cardInfo.children('span.question').remove();
  cardInfo.children('span.answer').remove();
  cardInfo.children('span.description').remove();
  cardInfo.append('<input type="text" class="question" data-old-value="' + oldQ + '" value="' + oldQ + '">');
  cardInfo.append('<input type="text" class="answer" data-old-value="' + oldA + '" value="' + oldA + '">');
  cardInfo.append('<input type="text" class="description" data-old-value="' + oldD + '" value="' + oldD + '">');
}
$('#course').on('click', '.level .buttons .change-button', function() {
  onEditButtonClick($(this));
});

// Kad se klikne na Accept dugme prilikom editovanja kartice:
//   - Obradi edit (upisi u _course)
//   - Vrati spanove od inputa
var onAcceptButtonClick = function(button) {
  // Prikupljanje podataka
  var cardID = button.parent().parent().attr('data-card-id');
  var cardInfo = button.parent().parent().find('.card-info');
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
}
$('#course').on('click', '.level .buttons .accept-button', function() {
  onAcceptButtonClick($(this));
});

// Kad klikne na Discard dugme prilikom editovanja kartice, vrati sve nazad.
// Skroz je nebitno sta je upisao.
// Samo vizuelno treba da vrati sve nazad.
// To "sve nazad" se cuva u data-old-value u inputima.
var onDiscardButtonClick = function(button) {
  var cardInfo = button.parent().parent().find('.card-info');
  var oldQ = cardInfo.children('input.question').attr('data-old-value').trim();
  var oldA = cardInfo.children('input.answer').attr('data-old-value').trim();
  var oldD = cardInfo.children('input.description').attr('data-old-value').trim();
  cardInfo.children('input.question').remove();
  cardInfo.children('input.answer').remove();
  cardInfo.children('input.description').remove();
  cardInfo.append('<span class="question">'+     oldQ + '</span>');
  cardInfo.append('<span class="answer">' +      oldA + '</span>');
  cardInfo.append('<span class="description">' + oldD + '</span>');
}
$('#course').on('click', '.level .buttons .discard-button', function() {
  onDiscardButtonClick($(this));
});

/****************************/
/****************************/
/****************************/
/****************************/
/***** Brisanje kartica *****/
/****************************/
/****************************/
/****************************/
/****************************/

// Kad se klikne na Remove dugme s namerom da obrise karticu
//   - Kartica ce da izgubi opacity, tj. videce se bledo.
//   - Ovo znaci da ce klikom na SAVE da obrise ovu karticu.
//   - Dok je kartica u ovom stanju, postoji undo dugme kojim se kartica vraca,
//     tj. nece da se je obrise kada pritisne na save.
var onRemoveButtonClick = function(button) {
  // Prikupljanje podataka
  var cardID = button.parent().parent().attr('data-card-id');

  // Sustina:
  for (var level = 0; level < _course.length; level++) {
    for (var card = 0; card < _course[level].cards.length; card++) {
      if (_course[level].cards[card].cardID == cardID) {
        // Nasili smo na karticu koja se brise
        var currCard = _course[level].cards[card];
        // Obelezi je kao karticu koja treba da se obrise, sta god da je ranije
        // bilo sa njom.
        // Ali sacuvaj kakva je ranije bila za slucaj da se uradi undo.
        currCard.prevStatus = currCard.status;
        currCard.status = DELETED;
      }
    }
  }

  // Vizuelno:
  button.parent().parent().addClass('dimmed');
  hideAllButtons(button.parent());
  button.parent().children('.undo-button').show();

  // Ako je brisanjem ove kartice nivo postao prazan, obelezi to.
  //debugger;
  var levelID = button.parent().parent().parent().parent().children('.level').attr('data-level-id');
  var levelIDnum = parseInt(levelID);
  var count = countWhatInWhere(DELETED, levelIDnum);
  var length = _course[button.closest('.level').parent().index()].cards.length;
  if (count == length) {
    button.parent().parent().parent().parent().addClass('empty');
    for (var level = 0; level < _course.length; level++) {
      var currLevel = _course[level];
      if (levelID == currLevel.levelID) {
        if (currLevel.status == NEW) {
          currLevel.status = UNTOUCHED;
        } else {
          currLevel.prevStatus = currLevel.status;
          currLevel.status = DELETED;
        }
      }
    }
  }
}
// Bind:
$('#course').on('click', '.level .buttons .remove-button', function() {
  onRemoveButtonClick($(this));
});

// Kad se klikne na Undo dugme
//   - Kartici se vraca stari status.
//   - LEvelu se vraca stari status.

var onUndoButtonClick = function(button) {
  // Prikupljanje podataka
  debugger;
  var cardID = button.parent().parent().attr('data-card-id');
  var levelID = button.closest('.level').attr('data-level-id');

  // Sustina:
  for (var level = 0; level < _course.length; level++) {
    if (_course[level].levelID == levelID) {
      var currLevel = _course[level];
      currLevel.status = currLevel.prevStatus;
      currLevel.prevStatus = -1;
    }
    for (var card = 0; card < _course[level].cards.length; card++) {
      if (_course[level].cards[card].cardID == cardID) {
        // Nasili smo na karticu koja nam treba
        var currCard = _course[level].cards[card];
        // Vrati stanje kartice koje je bilo pre nego sto je obrisana.
        currCard.status = currCard.prevStatus;
        currCard.prevStatus = -1;
        break;
      }
    }
  }

  // Vizuelno:
  button.parent().parent().removeClass('dimmed');
  // Takodje cim je undovano nesto, onda SIGURNO znamo da u tom nivou
  // postoji barem jedna kartica pa sklanjamo oznaku da je nivo prazan.
  button.parent().parent().parent().parent().removeClass('empty');
}

$('#course').on('click', '.level .buttons .undo-button', function() {
  onUndoButtonClick($(this));
});

/*****************************/
/*****************************/
/*****************************/
/*****************************/
/*******  Icon picker  *******/
/*****************************/
/*****************************/
/*****************************/
/*****************************/

var closeLevelIconPicker = function(levelInfo) {
  levelInfo.parent().children('.icon-picker').addClass('remove-me');
  // sacekaj da se izvrsi animacija
  setTimeout(function() { levelInfo.parent().children('.icon-picker').remove(); }, 500);
}
// Kad se klikne negde van, svi se zatvore
$(document).click( function(event) {
  if( $(event.target).closest('.level-info').length == 0 ) {
    closeLevelIconPicker($('.level-info'));
  }
});
$('#course').on('click', '.icon-picker', function() {
  return false; // da se ne zatvori
})


// Izabrao novu ikonicu:
$('#course').on('click', '.icon-picker .icon-picker-icons li', function() {
  var newIconID = $(this).attr('data-icon-id');
  var ipb = $(this).parent().parent().parent().find('.icon-picker-button');
  var oldIconID = ipb.attr('data-icon-id');
  var oldColorID = ipb.attr('data-color-id');
  ipb.attr('data-icon-id', newIconID);
  $(this).parent().children().removeClass('selected');
  $(this).addClass('selected');

  var levelID = $(this).parent().parent().parent().children('.level').attr('data-level-id');
  for (var level = 0; level < _course.length; level++) {
    var currLevel = _course[level];
    if (currLevel.levelID == levelID) {
      // Nadjen je kurs kom je editovana ikonica
      currLevel.icon = newIconID;
      if (currLevel.status != NEW) {
        currLevel.status = CHANGED;
      }
    }
  }
});

// Izabrao novu boju
$('#course').on('click', '.icon-picker .icon-picker-colors li', function() {
  var newColorID = $(this).attr('data-color-id');
  var ipb = $(this).parent().parent().parent().find('.icon-picker-button');
  var oldIconID = ipb.attr('data-icon-id');
  var oldColorID = ipb.attr('data-color-id');
  ipb.attr('data-color-id', newColorID);
  $(this).parent().children().removeClass('selected');
  $(this).addClass('selected');

  var levelID = $(this).parent().parent().parent().children('.level').attr('data-level-id');
  for (var level = 0; level < _course.length; level++) {
    var currLevel = _course[level];
    if (currLevel.levelID == levelID) {
      // Nadjen je kurs kom je editovana boja
      currLevel.color = newColorID;
      if (currLevel.status != NEW) {
        currLevel.status = CHANGED;
      }
    }
  }
});

var toggleLevelIconPicker = function(levelElement) {
  var levelInfo = levelElement.children('.level-info');
  if (levelInfo.parent().children('.icon-picker').length) {
    // Ako je vec otvoreno, obrisi ga
    closeLevelIconPicker(levelInfo);
    return;
  }
  var string = '';
  string += '<ul class="icon-picker">';
    string += '<ul class="icon-picker-icons">';
      for (var i = 1; i <= 50; i++) {
        var temp = "";
        if (levelInfo.find('.icon-picker-button').attr('data-icon-id') == i)
          temp = "selected";
        string += '<li data-icon-id="' + i + '" class="' + temp + '"><span></span></li>';
      }
    string += '</ul>';
    string += '<ul class="icon-picker-colors">';
      for (var i = 1; i <= 10; i++) {
        var temp = "";
        if (levelInfo.find('.icon-picker-button').attr('data-color-id') == i)
          temp = "selected";
        string += '<li data-color-id="' + i + '" class="' + temp + '"><span></span></li>';
      }
    string += '</ul>';
  string += '</ul>';
  levelInfo.after(string);
}
$('#course').on('click', '.icon-picker-button', function() {
  toggleLevelIconPicker($(this).parent().parent().parent());
})

/*****************************/
/*****************************/
/*****************************/
/*****************************/
/*******  Opcije nivoa *******/
/*****************************/
/*****************************/
/*****************************/
/*****************************/

var onClickToggleLevel = function(button) {
  button.toggleClass('collapsed').parent().parent().parent().children('.level').slideToggle(200);
}
$('#course').on('click', '.toggle-button', function() {
  onClickToggleLevel($(this));
});

var closeAdvancedLevelOptions = function(levelInfo) {
  levelInfo.parent().children('.options').addClass('remove-me');
  // sacekaj da se izvrsi animacija
  setTimeout(function() { levelInfo.parent().children('.options').remove(); }, 500);
}
// Kad se klikne negde van, svi se zatvore
$(document).click( function(event) {
  if( $(event.target).closest('.level-info').length == 0 ) {
    closeAdvancedLevelOptions($('.level-info'));
  }
});

var toggleAdvancedLevelOptions = function(levelElement) {
  var levelInfo = levelElement.children('.level-info');
  if (levelInfo.parent().children('.options').length) {
    // Ako je vec otvoreno, obrisi ga
    closeAdvancedLevelOptions(levelInfo);
    return;
  }
  var string = '';
  string += '<ul class="options">';
    string += '<li data-function="name-change"><span>Promeni ime</span><i class="fa fa-fw fa-pencil"></i></li>';
    string += '<li data-function="level-delete"><span>Obriši nivo</span><i class="fa fa-fw fa-trash"></i></li>';
    string += '<li data-function="mass-edit"><span>Grupno menjanje</span><i class="fa fa-fw fa-object-group"></i></li>';
    string += '<li data-function="swap-qa"><span>Zameni pitanje i odgovor</span><i class="fa fa-fw fa-exchange"></i></li>';
    string += '<li data-function="change-description"><span>Promeni opis svima</span><i class="fa fa-fw fa-reply-all"></i></li>';
  string += '</ul>';
  levelInfo.after(string);
}
$('#course').on('click', '.options-button', function() {
  toggleAdvancedLevelOptions($(this).parent().parent().parent());
})

var onClickLevelNameChange = function(li) {
  var levelName = li.find('.level-name');
  var oldName = levelName.children('span').html().trim();
  levelName.children('span').remove();
  string = '';
  string += '<div class="edit-level-name">';
    string += '<input type="text" data-old-name="' + oldName + '" value="' + oldName + '"/>';
    string += '<div class="button-accept"><i class="fa fa-check"></i></div>';
    string += '<div class="button-discard"><i class="fa fa-times"></i></div>';
  string += '</div>';
  levelName.append(string);
}

var onClickLevelNameChangeAccept = function(li) {
  var levelID = li.children('ul.level').attr('data-level-id');
  var newName = li.find('.level-name input').val().trim();
  var oldName = li.find('.level-name input').attr('data-old-name');
  if (oldName != newName) {
    // Ako je zapravo doslo do promene necega
    // (Izbegavamo situaciju edit -> ne promeni -> accept)
    for (var level = 0; level < _course.length; level++) {
      var currLevel = _course[level];
      if (currLevel.levelID == levelID) {
        // Nadjen je kurs kom je editovano ime
        currLevel.name = newName;
        if (currLevel.status != NEW) {
          currLevel.status = CHANGED;
        }
      }
    }
  }

  li.find('.level-name .edit-level-name').remove();
  li.find('.level-name').append('<span>' + newName + '</span>');
}
$('#course').on('click', '.edit-level-name .button-accept', function() {
  onClickLevelNameChangeAccept($(this).parent().parent().parent().parent());
})

var onClickDeleteLevel = function(button) {
  var li = button.parent().parent();
  var levelID = li.children('ul.level').attr('data-level-id');

  button.parent().parent().children('ul.level').find('.remove-button').each(function() {
    onRemoveButtonClick($(this));
  });

  li.addClass('empty');
}

var onClickMassEdit = function(button) {
  button.parent().parent().children('ul.level').find('.change-button').each(function() {
    onEditButtonClick($(this));
  });
}

var onClickSwapQA = function(button) {
}

var onClickChangeDescriptionToAll = function(button) {

}
$('#course').on('click', 'ul.options > li', function() {
  switch ($(this).attr('data-function')) {
    case 'name-change': onClickLevelNameChange($(this).parent().parent()); break;
    case 'level-delete': onClickDeleteLevel($(this)); break;
    case 'mass-edit': onClickMassEdit($(this)); break;
    case 'swap-qa': onClickSwapQA($(this)); break;
    case 'change-description': onClickChangeDescriptionToAll($(this)); break;
  }
  toggleAdvancedLevelOptions($(this).parent().parent());
});

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

  // Celi nivoi:
  for (var i = 0; i < _course.length; i++) {
    var currLevel = _course[i];
    switch (currLevel.status) {
      case NEW:     _dataToSend.addedLevels.push(currLevel); break;
      case CHANGED: _dataToSend.editedLevels.push(currLevel);
          break;
      case DELETED: _dataToSend.deletedLevels.push(currLevel); break;
    }
  }

  // Pojedinacne kartice:
  for (var i = 0; i < _course.length; i++) {
    if (_course[i].status != NEW) {
      for (var j = 0; j < _course[i].cards.length; j++) {
          var currCard = _course[i].cards[j];
        switch (currCard.status) {
          case NEW:     _dataToSend.addedCards.push(currCard); break;
          case CHANGED: _dataToSend.editedCards.push(currCard); break;
          case DELETED:
              if (currCard.cardID > 0) {
                  _dataToSend.deletedCards.push(currCard.cardID);
              }
              break;
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
    showInitialButtons();
  }
});

$('#save').click(save);


var dump = function() {
  console.log(_dataToSend);
}

$(document).ready(function() {
  viewToData();
  // Inicijalno sklanjane nepotrebnih dugmadi za editovanje kartice.
  showInitialButtons();
});

/**
 * Dizajn
 */

var showInitialButtons = function(here) {
  if (here == null) {
    // sve
    $('.buttons > .accept-button').hide();
    $('.buttons > .discard-button').hide();
    $('.buttons > .undo-button').hide();
  } else {
    // samo one u prosledjenom .buttons parentu
    hideAllButtons(here);
    here.children('.change-button').show();
    here.children('.remove-button').show();
  }
}
$('#course').on('click', '.undo-button', function() {
  showInitialButtons($(this).parent());
});

var hideAllButtons = function(here) {
  here.children('.change-button').hide();
  here.children('.remove-button').hide();
  here.children('.accept-button').hide();
  here.children('.discard-button').hide();
  here.children('.undo-button').hide();
}

$('#course').on('click', '.accept-button', function() {
  showInitialButtons($(this).parent());
});

$('#course').on('click', '.discard-button', function() {
  showInitialButtons($(this).parent());
});
