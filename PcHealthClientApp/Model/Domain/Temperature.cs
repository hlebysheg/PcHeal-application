using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcHealthClientApp.Model.Domain
{
	public class Temperature
	{
		public float Value { get; private set; }
		public Temperature(float tmp)
		{
			if (tmp > 200 && tmp < -30) throw new ArgumentOutOfRangeException(nameof(tmp));
			Value = tmp;
		}
	}
	
}
