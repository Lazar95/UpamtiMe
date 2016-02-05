var hideCategories = function(cat, subcat) { //TODO refaktorisi prebaci u global majke ti
  var catID = cat.children('option:selected').val();
  subcat.children().removeAttr('hidden');
  subcat.children(':not([data-catid="' + catID + '"])').attr('hidden', '');
}

$('#course-category > select').change(function() {
  $('#course-subcategory > select').children('[value="0"]').attr('selected', '');
  hideCategories($(this), $('#course-subcategory > select'));
});
