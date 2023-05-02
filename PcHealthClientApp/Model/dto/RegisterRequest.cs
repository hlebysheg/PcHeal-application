
namespace PcHealthClientApp.Model.dto
{
    public class RegisterRequest
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
	public class LoginRequest
	{
		public string Name { get; set; }
		public string Password { get; set; }
	}
}
