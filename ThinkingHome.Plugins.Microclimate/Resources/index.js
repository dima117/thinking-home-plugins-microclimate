define(
	['app', 'marionette', 'backbone', 'underscore',
		'webapp/microclimate/index-view'],
	function (application, marionette, backbone, _, views) {

		var module = {
			start: function () {


				var model = new backbone.Collection([
					{ displayName: 'Спальня', t: '+24' },
					{ displayName: 'Зал', t: '+25' },
					{ displayName: 'Кухня', t: '+28' }
				]);

				var view = new views.SensorList({
					collection: model
				});

				application.setContentView(view);
			}
		};
		return module;
	});