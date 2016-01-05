var _course = {

}

/*
var viewToData = function() {
  var course = $('.course');
  var listOfLevels;
  $.each(course, function())
}
*/

var _dataToSend = {
  "deletedCards":  [], // lista ID-jeva kartica
  "deletedLevels": [], // lista Id-jeva nivoa
  "editedCards":   [], // niz objekata kartica
  "editedLevels":  [], // niz objekata nivoa
}

var addLevel = function() {
  var newLevel = {
    "id": -1,
    "type": 0,
    "name": "",
  }

  newLevel.type = $('#new-level select option:selected').val();
  newLevel.name = $('#new-level input').val();

  _dataToSend.editedLevels.push(newLevel);

  dump();

  $.ajax({
      url: "/Courses/EditCourse", // /kontroler/akcija (klasa/funkcija u klasi)
      method: "POST",
      data: _dataToSend,
      success: function (res) {
          if (res.success) {
              $('#new-level').append('Waai uspesno!');
          }
      }
  });

}

/**
 * Bindovanje funkcija za dugmad
 */
$('#new-level .add-button').click(function() {
  alert('dafgsd');
  //addLevel();
});

$( "#new-level .add-button" ).click(function() {
  alert( "Handler for .click() called." );
});

var dump = function() {
  console.log(_dataToSend);
}

$(document).ready(function() {

});
