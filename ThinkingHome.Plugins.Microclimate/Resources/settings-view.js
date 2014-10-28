﻿define([
	'app',
	'marionette',
	'backbone',
	'underscore',
	'text!webapp/microclimate/settings.tpl'
],
	function (application, marionette, backbone, _, tmplSettings) {

		var sensorTableRowView = marionette.ItemView.extend({
			template: _.template(
				'<td><%= displayName %></td>' +
				'<td class="col-md-1"><%= channel %></td>' +
				'<td class="col-md-1"><%= showHumidity %></td>' +
				'<td class="col-md-2"><a class="js-delete-sensor" href="#">delete</a></td>'),
			tagName: 'tr'
		});

		var sensorTableView = marionette.CompositeView.extend({
			template: _.template(tmplSettings),
			childView: sensorTableRowView,
			childViewContainer: 'tbody'
		});

		return {
			SensorTable: sensorTableView
		};
	});