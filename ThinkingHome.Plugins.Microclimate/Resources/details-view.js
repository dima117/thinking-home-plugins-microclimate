define(
	['app', 'marionette', 'backbone', 'underscore',
	 'text!webapp/microclimate/details-template.tpl'// шаблон для страницы подробной информации
	],
	function (application, marionette, backbone, _, tmplDetails) {

		var sensorDetailsView = marionette.ItemView.extend({
			template: _.template(tmplDetails),
			triggers: {
				'click .js-show-list': 'show:sensor:list'
			}
		});

		return {
			SensorDetails: sensorDetailsView
		};
	});