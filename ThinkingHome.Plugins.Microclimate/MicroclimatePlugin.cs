using System;
using System.Linq;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Plugins.Listener.Api;
using ThinkingHome.Plugins.Listener.Attributes;
using ThinkingHome.Plugins.Microclimate.Model;
using ThinkingHome.Plugins.NooLite;

namespace ThinkingHome.Plugins.Microclimate
{
	[Plugin]
	public class MicroclimatePlugin : PluginBase
    {
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

		[HttpCommand("/api/microclimate/data")]
		public object GetSensorData(HttpRequestParams request)
		{
			var sensorId = request.GetRequiredGuid("sensorId");

			using (var session = Context.OpenSession())
			{
				var data = session.Query<TemperatureData>()
					.Where(d => d.Sensor.Id == sensorId)
					.OrderByDescending(d => d.CurrentDate)
					.Take(20)
					.ToList();

				return data.Select(CreateModel).ToList();
			}
		}

		private object CreateModel(TemperatureData arg)
		{
			return new
			{
				d = arg.CurrentDate,
				t = arg.Temperature,
				h = arg.Humidity
			};
		}

		#endregion
    }
}
