$('#search-results-outer').on('click', '.show-more', function() {
  $(this).closest('.result').toggleClass('expanded');
})
