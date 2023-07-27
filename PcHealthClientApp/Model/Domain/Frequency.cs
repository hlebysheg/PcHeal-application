using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcHealthClientApp.Model.Domain
{
	public class Frequency
	{
		public float Value { get; private set; }
		public Frequency(float fq) {
			if(fq < 0) throw new ArgumentOutOfRangeException(nameof(fq));
			Value = fq;
		}
	}
}
