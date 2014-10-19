define(
	['app', 'marionette', 'backbone', 'underscore',
		'webapp/microclimate/index-view',
		'webapp/microclimate/index-model'
	],
	function (application, marionette, backbone, _, views) {

		var module = {
			start: function () {

				var rq = application.request('query:microclimate:sensors');

				$.when(rq).done(function (collection) {

					var view = new views.SensorList({
						collection: collection
					});

					application.setContentView(view);
				});
			}
		};
		return module;
	});