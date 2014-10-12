using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq;
using ThinkingHome.Core.Plugins;
using ThinkingHome.Plugins.Microclimate.Model;
using ThinkingHome.Plugins.NooLite;

namespace ThinkingHome.Plugins.Microclimate
{
	[Plugin]
	public class MicroclimatePlugin : PluginBase
    {
		[OnMicroclimateDataReceived]
		public void MicroclimateDataReceived(int channel, decimal temperature, int humidity)
		{
			var now = DateTime.Now;

			using (var session = Context.OpenSession())
			{
				var sensors = session
					.Query<TemperatureSensor>()
					.Where(s => s.Channel == channel)
					.ToList();

				foreach (var sensor in sensors)
				{
					var data = new TemperatureData
					{
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
    }
}
