var hideCategories = function(cat, subcat) {
  var catID = cat.children('option:selected').val();
  subcat.children().removeAttr('hidden');
  subcat.children(':not([data-catid="' + catID + '"])').attr('hidden', '');
}

$('#new-course-category > select').change(function() {
  hideCategories($(this), $('#new-course-subcategory > select'));
});
