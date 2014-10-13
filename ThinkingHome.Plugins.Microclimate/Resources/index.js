define(
	['app', 'marionette', 'backbone', 'underscore'],
	function (application, marionette, backbone, _) {
		var module = {
			start: function () {
				alert("Microclimate module started!");
			}
		};
		return module;
	});