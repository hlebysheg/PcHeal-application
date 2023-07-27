using LibreHardwareMonitor.Hardware;
using PcHealthClientApp.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcHealthClientApp.Infrastructure.Fabrics
{
	public static class PcInfoFabric
	{
		static Computer c = new Computer();
		public static PcInfo InitialCreate()
		{
			var initialInfo = new PcInfo();
			initialInfo.PcName = "";
			initialInfo.GPUName = "";
			initialInfo.CPULoad = 0;
			initialInfo.GPULoad = 0;
			initialInfo.CPUFrenq = new Frequency(0);
			initialInfo.CPUTemp = new Temperature(0);
			initialInfo.GPUTemp = new Temperature(0);
			return initialInfo;
		}
			public static PcInfo Create()
		{
			c.Open();
			c.Accept(new UpdateVisitor());
			c.IsCpuEnabled = true;
			c.IsGpuEnabled = true;
			c.IsMotherboardEnabled = true;
			PcInfo info = new PcInfo();
			info.PcName =  Environment.MachineName;
			foreach (var hardware in c.Hardware)
			{
				if (hardware.HardwareType == HardwareType.Cpu)
				{
					// only fire the update when found
					info.CPUName = hardware.Name;

					// loop through the data
					foreach (var sensor in hardware.Sensors)
						if (sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("CPU Package"))
						{
							info.CPUTemp = new Temperature(sensor.Value.GetValueOrDefault());
						}
						else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
						{
							info.CPULoad = sensor.Value.GetValueOrDefault();

						}

						else if (sensor.SensorType == SensorType.Clock && sensor.Name.Contains("CPU Core #1"))
						{
							info.CPUFrenq = new Frequency(sensor.Value.GetValueOrDefault());
						}
				}

				if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAmd || hardware.HardwareType == HardwareType.GpuIntel)
				{
					// only fire the update when found
					hardware.Update();
					info.GPUName = hardware.Name;
					// loop through the data
					foreach (var sensor in hardware.Sensors)
						if (sensor.SensorType == SensorType.Temperature)
						{
							// store
							hardware.Update();
							info.GPUTemp = new Temperature((sensor.Value.GetValueOrDefault()));

						}
						else if (sensor.SensorType == SensorType.Load && sensor.Name.Contains("CPU Total"))
						{
							info.GPULoad = sensor.Value.GetValueOrDefault();
						}
				}
			}

			return info;

		}
	}
}
