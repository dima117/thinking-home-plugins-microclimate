using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Plugins.Listener.Api;
using ThinkingHome.Plugins.Listener.Attributes;
using ThinkingHome.Plugins.Microclimate.Model;
using ThinkingHome.Plugins.NooLite;
using ThinkingHome.Plugins.WebUI.Attributes;

namespace ThinkingHome.Plugins.Microclimate
{
	[Plugin]
	[AppSection("Microclimate", SectionType.Common, "/webapp/microclimate/index.js", "ThinkingHome.Plugins.Microclimate.Resources.index.js")]
	[JavaScriptResource("/webapp/microclimate/index-view.js", "ThinkingHome.Plugins.Microclimate.Resources.index-view.js")]

	[HttpResource("/webapp/microclimate/item-template.tpl", "ThinkingHome.Plugins.Microclimate.Resources.item-template.tpl")]
	[HttpResource("/webapp/microclimate/list-template.tpl", "ThinkingHome.Plugins.Microclimate.Resources.list-template.tpl")]
	[CssResource("/webapp/microclimate/index.css", "ThinkingHome.Plugins.Microclimate.Resources.index.css", AutoLoad = true)]
	public class MicroclimatePlugin : PluginBase
	{
		public const int PERIOD = 2400;

		public override void InitDbModel(ModelMapper mapper)
		{
			mapper.Class<TemperatureSensor>(cfg => cfg.Table("Microclimate_TemperatureSensor"));
			mapper.Class<TemperatureData>(cfg => cfg.Table("Microclimate_TemperatureData"));
		}

		[OnMicroclimateDataReceived]
		public void MicroclimateDataReceived(int channel, decimal temperature, int humidity)
		{
			var now = DateTime.Now;

			Logger.Debug("microclimate data received: c={0}, t={1}, h={2}", channel, temperature, humidity);

			using (var session = Context.OpenSession())
			{
				var sensors = session
					.Query<TemperatureSensor>()
					.Where(s => s.Channel == channel)
					.ToList();

				Logger.Debug("{0} sensors was found", sensors.Count);

				foreach (var sensor in sensors)
				{
					var data = new TemperatureData
					{
						Id = Guid.NewGuid(),
						CurrentDate = now,
						Temperature = Convert.ToInt32(temperature),
						Humidity = humidity,
						Sensor = sensor
					};

					session.Save(data);
				}

				session.Flush();
			}
		}

		#region api

		[HttpCommand("/api/microclimate/sensors/add")]
		public object AddSensor(HttpRequestParams request)
		{
			var channel = request.GetRequiredInt32("channel");
			var displayName = request.GetRequiredString("displayName");
			var showHumidity = request.GetRequiredBool("showHumidity");

			Logger.Debug("add sensor: channel={0}; displayName={1}; showHumidity={2}", channel, displayName, showHumidity);

			using (var session = Context.OpenSession())
			{
				var sensor = new TemperatureSensor
				{
					Id = Guid.NewGuid(),
					Channel = channel,
					DisplayName = displayName,
					ShowHumidity = showHumidity
				};

				session.Save(sensor);
				session.Flush();

				return sensor.Id;
			}
		}

		[HttpCommand("/api/microclimate/sensors/details")]
		public object GetSensorDetails(HttpRequestParams request)
		{
			var sensorId = request.GetRequiredGuid("sensorId");
			var from = DateTime.Now.AddHours(-PERIOD);

			using (var session = Context.OpenSession())
			{
				var sensor = session.Query<TemperatureSensor>().First(s => s.Id == sensorId);

				var data = session.Query<TemperatureData>()
					.Where(d => d.Sensor.Id == sensor.Id && d.CurrentDate > from)
					.OrderByDescending(d => d.CurrentDate)
					.ToList();

				return CreateSensorDataModel(sensor, data);
			}
		}

		[HttpCommand("/api/microclimate/sensors/list")]
		public object GetSensorList(HttpRequestParams request)
		{
			var from = DateTime.Now.AddHours(-PERIOD);

			using (var session = Context.OpenSession())
			{
				var data = session.Query<TemperatureData>()
					.Where(d => d.CurrentDate > from)
					.ToList();

				var sensors = session.Query<TemperatureSensor>().ToList();

				var model = sensors
					.GroupJoin(data, s => s.Id, d => d.Sensor.Id, (s, d) => new { s, d })
					.Select(x => CreateSensorDataModel(x.s, x.d.OrderByDescending(d => d.CurrentDate).Take(1)))
					.ToList();

				return model;
			}
		}

		private object CreateSensorDataModel(TemperatureSensor sensor, IEnumerable<TemperatureData> gr)
		{
			return new
			{
				id = sensor.Id,
				displayName = sensor.DisplayName,
				data = gr.Select(arg => 
					new
					{
						d = arg.CurrentDate, 
						t = arg.Temperature, 
						h = arg.Humidity
					}).ToArray()
			};
		}

		#endregion
	}
}
