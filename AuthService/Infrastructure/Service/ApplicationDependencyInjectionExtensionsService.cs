using AuthService.Infrastructure.Service.PcStat;

namespace AuthService.Infrastructure.Service
{
	public static class ApplicationDependencyInjectionExtensionsService
	{
		public static IServiceCollection AddAppService(this IServiceCollection services)
		{
			return services
				.AddScoped<IPcStatService, PcStatService>();
		}
	}
}
