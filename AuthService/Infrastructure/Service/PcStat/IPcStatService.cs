using AuthService.Models;
using WordBook.Model.dto;

namespace AuthService.Infrastructure.Service.PcStat
{
	public interface IPcStatService
	{
		public Task<bool> SaveStat(PCInfoMessage msg, string name);
		public Task<List<PcStatistic>> GetStats(string name);
	}
}
