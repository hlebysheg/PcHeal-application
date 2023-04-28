using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace pcHealth.Controllers
{
	[Route("api/pchealh")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			var re = Request.Headers.FirstOrDefault(el => el.Key == "username");
			return Ok(re);
		}
	}
}
