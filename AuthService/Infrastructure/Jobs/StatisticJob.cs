using WordBook.Models;

namespace AuthService.Infrastructure.Jobs
{
	public class StatisticJob: IStatisticJob
	{
		private readonly ApplicationDbContext _context;
		public StatisticJob(ApplicationDbContext ctx)
		{
			_context = ctx;
		}
		public bool ClearJob()
		{
			try
			{
				var old = _context.PcStatistic.Where(el => el.Date.AddDays(1) < DateTime.Today);
				_context.RemoveRange(old);
				_context.SaveChanges();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
	}
	public interface IStatisticJob
	{
		public bool ClearJob();
	}

}
