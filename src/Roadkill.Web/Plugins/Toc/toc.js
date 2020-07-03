$(document).ready(function() {
	// The show/hide for table of contents
	$("a.toc-showhide").click(function() {
		var el = $(this),
			t = $(this).text();

		switch (t) {
			case 'hide': el.text('show'); break;
			case 'show': el.text('hide'); break;
			case 'skrýt': el.text('ukázat'); break;
			case 'ukázat': el.text('skrýt'); break;
		}
		el.parent().next().toggle();
	});
});
