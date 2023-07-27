using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PcHealthClientApp.Model.Domain
{
	public class PcInfo : INotifyPropertyChanged
	{
		private string _pcName;
		private string _сpuName;
		private Temperature _cpuTemperature;
		private float _cpuLoad;
		private Frequency _cpuFrenq;
		private string _gpuName;
		private Temperature _gpuTemp;
		private float _gpuLoad;

		public string PcName
		{
			get { return _pcName; }
			set
			{
				_pcName = value;
				OnPropertyChanged("PcName");
			}
		}
		public string CPUName
		{
			get { return _сpuName; }
			set
			{
				_сpuName = value;
				OnPropertyChanged("CPUName");
			}
		}
		public Temperature CPUTemp
		{
			get { return _cpuTemperature; }
			set
			{
				_cpuTemperature = value;
				OnPropertyChanged("CPUTemp");
			}
		}
		public float CPULoad
		{
			get { return _cpuLoad; }
			set
			{
				_cpuLoad = value;
				OnPropertyChanged("CPULoad");
			}
		}
		public Frequency CPUFrenq
		{
			get { return _cpuFrenq; }
			set
			{
				_cpuFrenq = value;
				OnPropertyChanged("PcName");
			}
		}

		public string GPUName
		{
			get { return _gpuName; }
			set
			{
				_gpuName = value;
				OnPropertyChanged("GPUName");
			}
		}
		public Temperature GPUTemp
		{
			get { return _gpuTemp; }
			set
			{
				_gpuTemp = value;
				OnPropertyChanged("PcName");
			}
		}
		public float GPULoad
		{
			get { return _gpuLoad; }
			set
			{
				_gpuLoad = value;
				OnPropertyChanged("GPULoad");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(prop));
		}
	}
}
