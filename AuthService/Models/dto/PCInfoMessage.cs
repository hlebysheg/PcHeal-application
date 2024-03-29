﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordBook.Model.dto
{
	public class PCInfoMessage
	{
		public string PcName { get; set; }
		public string CPUName { get; set; }
		public float? CPUTemp { get; set; }
		public float CPULoad { get; set; }
		public float? CPUFrenq { get; set; }

		public string GPUName { get; set; }
		public float? GPUTemp { get; set; }
		public float GPULoad { get; set; }
	}
}
