var hideCategories = function(cat, subcat) { //TODO refaktorisi
  var catID = cat.children('option:selected').val();
  subcat.children().removeAttr('hidden');
  subcat.children(':not([data-catid="' + catID + '"])').attr('hidden', '');
}

$('#new-course-category > select').change(function() {
  $('#new-course-subcategory > select').children('[value="0"]').attr('selected', '');
  hideCategories($(this), $('#new-course-subcategory > select'));
});

// Validacija za kreiranje novog kursa
$('#create-new-course-button').click(function() {
  var name = $('#new-course-name > input').val();
  var cat = $('#new-course-category > select > :selected').val();
  var subcat = $('#new-course-subcategory > select > :selected').val();
  var error = "";
  if (name == "" || name == undefined)                      error += 'Kurs mora imati ime! ';
  if (cat == "0" || cat == "" || cat === undefined)         error += 'Kurs mora pripadati nekoj kategoriji! ';
  if (subcat == "0" || subcat == "" || subcat == undefined) error += 'Kurs mora pripadati nekoj podkategoriji! ';
  if (error == "") {
    return true;
  } else {
    alert(error);
    return false;
  }
});
