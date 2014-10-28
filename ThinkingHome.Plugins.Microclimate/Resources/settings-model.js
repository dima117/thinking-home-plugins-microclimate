define(
	['app', 'marionette', 'backbone', 'underscore'],
	function (application, marionette, backbone, _) {

		var api = {
			loadSensorTable: function() {

				var defer = $.Deferred();

				$.getJSON('/api/microclimate/sensors/table')
					.done(function (items) {

						var collection = new backbone.Collection(items);
						defer.resolve(collection);
					})
					.fail(function() {

						defer.resolve(undefined);
					});

				return defer.promise();
			}
		};

		// requests
		application.reqres.setHandler('query:microclimate:sensor-table', api.loadSensorTable);

		return api;
	});