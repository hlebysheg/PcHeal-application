using WordBook.Models;

namespace AuthService.Models
{
	public class PcStatistic
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public User User { get; set; }

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
