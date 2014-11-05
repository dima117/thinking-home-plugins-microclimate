﻿define(
	['app', 'common', 'marionette', 'backbone', 'underscore',
		'webapp/microclimate/settings-view',
		'webapp/microclimate/settings-model'
	],
	function (application, commonModule, marionette, backbone, _, views) {

		var api = {
			addSensor: function () {

				var displayName = this.ui.displayName.val();
				var channel = this.ui.channel.val();
				var showHumidity = this.ui.showHumidity.prop('checked');

				if (displayName) {

					application
						.request('cmd:microclimate:sensor:add', displayName, channel, showHumidity)
						.done(api.loadSettings);

				}
			},
			addSensorTile: function (view) {

				var sensorId = view.model.get('id');
				application.addTile('ThinkingHome.Plugins.Microclimate.MicroclimateTileDefinition', { id: sensorId });
			},
			deleteSensor: function (childView) {
				
				var displayName = childView.model.get('displayName');

				if (commonModule.utils.confirm('Delete the sensor "{0}" and all related data?', displayName)) {

					var id = childView.model.get('id');

					application.request('cmd:microclimate:sensor:delete', id)
						.done(api.loadSettings);
				}
			},
			loadSettings: function () {

				var rq = application.request('query:microclimate:sensor:table');

				$.when(rq).done(function (collection) {

					var view = new views.SensorTable({
						collection: collection
					});

					view.on('add:sensor', api.addSensor);
					view.on('childview:delete:sensor', api.deleteSensor);
					view.on('childview:add:sensor:tile', api.addSensorTile);
					application.setContentView(view);
				});
			}
		};

		var module = {
			start: api.loadSettings
		};
		return module;
	});