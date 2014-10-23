define(
	['app', 'marionette', 'backbone', 'underscore', 'highcharts'],
	function (application, marionette, backbone, _, highcharts) {

		var chart = undefined;

		var api = {
			prepareData: function (model) {
				var tdataset = [],
					hdataset = [],
					data = model.get('data');

				if (data && data.length) {
					_.each(data, function (el) {

						var timestamp = new Date(el.d).getTime();
						tdataset.push([timestamp, el.t]);
						hdataset.push([timestamp, el.h]);
					});

					tdataset = _.sortBy(tdataset, function (el) { return el[0]; });
					hdataset = _.sortBy(hdataset, function (el) { return el[0]; });
				}

				return { tdataset: tdataset, hdataset: hdataset };
			},

			getChartOptions: function(container) {
				
				var options = {
					title: { text: null },
					chart: {
						renderTo: container,
						type: 'spline'
					},
					xAxis: {
						type: 'datetime',
						title: { text: 'Timestamp' }
					},
					yAxis: [],
					series: []
				};

				return options;
			},

			addData: function (options, name, dataset, unit, opposite, zIndex, color) {

				options.yAxis.push({
					title: { text: name },
					labels: { format: '{value}' + unit },
					opposite: opposite
				});

				options.series.push({
					name: name,
					data: dataset,
					yAxis: options.yAxis.length - 1,
					tooltip: { valueSuffix: unit },
					zIndex: zIndex,
					color: color
				});
			},

			build: function (view) {
			
				var data = api.prepareData(view.model);

				var container = view.$('.js-chart').get(0);
				var options = api.getChartOptions(container);

				api.addData(options, 'Temperature', data.tdataset, '°C', false, 1, '#428bca');

				if (view.model.get('showHumidity')) {

					api.addData(options, 'Humidity', data.hdataset, '%', true, 0, '#d9534f');
				}

				chart = new highcharts.Chart(options);
			}
		};

		return {
			build: api.build,
			getChart: function() { return chart; }
		};
	});