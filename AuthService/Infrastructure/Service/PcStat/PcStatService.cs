using AuthService.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WordBook.Model.dto;
using WordBook.Models;

namespace AuthService.Infrastructure.Service.PcStat
{
    public class PcStatService : IPcStatService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public PcStatService(IMapper mapper, ApplicationDbContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> SaveStat(PCInfoMessage msg, string name)
        {
            var info = _mapper.Map<PcStatistic>(msg);
            var user = await _context.Student.FirstOrDefaultAsync(el => el.Name == name);
            if (user == null)
            {
                return false;
            }
            info.User = user;
            info.Date = DateTime.UtcNow;
            await _context.PcStatistic.AddAsync(info);

            return await SaveDb();
        }

        public async Task<List<PcStatistic>> GetStats(string name)
        {
            var response = await _context.PcStatistic
                .Where(st => st.User.Name == name)
				.OrderBy(st => st.PcName)
                .ThenBy(st => st.Date)
                .ToListAsync();

            return response;
        }

        private async Task<bool> SaveDb()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
