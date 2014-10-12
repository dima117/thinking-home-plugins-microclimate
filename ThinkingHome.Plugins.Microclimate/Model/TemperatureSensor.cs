﻿using System;

namespace ThinkingHome.Plugins.Microclimate.Model
{
	public class TemperatureSensor
	{
		public virtual Guid Id { get; set; }

		public virtual int Channel { get; set; }

		public virtual string DisplayName { get; set; }

		public virtual bool ShowHumidity { get; set; }
	}
}
