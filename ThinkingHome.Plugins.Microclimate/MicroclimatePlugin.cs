using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkingHome.Core.Plugins;

namespace ThinkingHome.Plugins.Microclimate
{
	[Plugin]
	public class MicroclimatePlugin : PluginBase
    {
		public override void InitPlugin()
		{
			Logger.Debug("init");
			base.InitPlugin();
		}

		public override void StartPlugin()
		{
			Logger.Debug("start");
			base.StartPlugin();
		}

		public override void StopPlugin()
		{
			Logger.Debug("stop");
			base.StopPlugin();
		}
    }
}
