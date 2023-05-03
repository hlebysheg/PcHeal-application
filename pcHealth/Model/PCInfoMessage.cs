namespace pcHealth.Model
{
	public class PCInfoMessage
	{
		public string CPUName { get; set; }
		public float CPUTemp { get; set; }
		public float CPULoad { get; set; }
		public float CPUFrenq { get; set; }

		public string GPUName { get; set; }
		public float GPUTemp { get; set; }
		public float GPULoad { get; set; }
	}
}
