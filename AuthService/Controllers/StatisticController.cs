using AuthService.Infrastructure.Jobs;
using AuthService.Infrastructure.Service.PcStat;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StatisticController : ControllerBase
	{
		private readonly IBackgroundJobClient _backgroundJobClient;
		private readonly IRecurringJobManager _recurringJobManager;
		private readonly IPcStatService _stat;
		public StatisticController(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager, IPcStatService stat)
		{
			_backgroundJobClient = backgroundJobClient;
			_recurringJobManager = recurringJobManager;
			_stat = stat;
		}//IPcStatService
		[HttpGet]
		[Route("stat")]
		public IActionResult Statistic()
		{
			var name = User?.Identity?.Name;
			var stat = _stat.GetStats(name?? "string");
			return Ok(stat);
		}
		[HttpGet]
		[Route("IFireAndForgetJob")]
		public string FireAndForgetJob()
		{
			//Fire - and - Forget Jobs
			//Fire - and - forget jobs are executed only once and almost immediately after creation.
			var jobId = _backgroundJobClient.Enqueue<IStatisticJob>(x => x.ClearJob());

			return $"Job ID: {jobId}.Fire and forget clear";
		}
		[HttpGet]
		[Route("IDelayedJob")]
		public string DelayedJob()
		{
			//Delayed Jobs
			//Delayed jobs are executed only once too, but not immediately, after a certain time interval.
			var jobId = _backgroundJobClient.Schedule(() => Console.WriteLine("Welcome user in Delayed Job Demo!"), TimeSpan.FromSeconds(60));

			return $"Job ID: {jobId}. Welcome user in Delayed Job Demo!";
		}

		[HttpGet]
		[Route("IContinuousJob")]
		public string ContinuousJob()
		{
			//Fire - and - Forget Jobs
			//Fire - and - forget jobs are executed only once and almost immediately after creation.
			var parentjobId = _backgroundJobClient.Enqueue(() => Console.WriteLine("Welcome user in Fire and Forget Job Demo!"));

			//Continuations
			//Continuations are executed when its parent job has been finished.
			BackgroundJob.ContinueJobWith(parentjobId, () => Console.WriteLine("Welcome Sachchi in Continuos Job Demo!"));

			return "Welcome user in Continuos Job Demo!";
		}

		[HttpGet]
		[Route("IRecurringJob")]
		public string RecurringJobs()
		{
			//Recurring Jobs
			//Recurring jobs fire many times on the specified CRON schedule.
			_recurringJobManager.AddOrUpdate<IStatisticJob>("jobId", x => x.ClearJob(), Cron.Daily);

			return "clear job start";
		}
	}
}
