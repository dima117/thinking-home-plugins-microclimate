define(
	[	'app', 'marionette', 'backbone', 'underscore',
		'webapp/microclimate/settings-view',
		'webapp/microclimate/settings-model'
	],
	function (application, marionette, backbone, _, views) {

		var api = {
			showSettings: function() {

				var rq = application.request('query:microclimate:sensor-table');

				$.when(rq).done(function (collection) {

					var view = new views.SensorTable({
						collection: collection
					});

					//view.on('childview:show:sensor:details', api.details);
					application.setContentView(view);
				});
			}
		};

		var module = {
			start: api.showSettings
		};
		return module;
	});