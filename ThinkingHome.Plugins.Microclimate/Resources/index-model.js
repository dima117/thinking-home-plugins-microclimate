define(
	['app', 'marionette', 'backbone', 'underscore'],
	function (application, marionette, backbone, _) {

		var sensorModel = backbone.Model.extend({
			toJSON: function () {

				var json = backbone.Model.prototype.toJSON.apply(this, arguments);

				json.current = json.data && json.data.length
					? json.data[0] : null;
				
				return json;
			}
		});

		var sensorCollection = backbone.Collection.extend({
			model: sensorModel,
		});

		var api = {
			loadSensors: function() {

				var defer = $.Deferred();

				$.getJSON('/api/microclimate/sensors/list')
					.done(function (items) {

						var collection = new sensorCollection(items);
						defer.resolve(collection);
					})
					.fail(function() {

						defer.resolve(undefined);
					});

				return defer.promise();
			}
		};

		// requests
		application.reqres.setHandler('query:microclimate:sensors', api.loadSensors);

		return api;
	});