using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WordBook.Models;
using WordBook.Models.dto;
using WordBook.reposit;

namespace WordBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLogin : ControllerBase
    {
        private readonly ILogger<UserLogin> _logger;
        private _userRep db;
        private IConfiguration _conf;
        public UserLogin(ApplicationDbContext context, IConfiguration config, ILogger<UserLogin>? log)
        {

            db = new _userRep(context);
            _conf = config;
            _logger = log;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("log")]
        public IActionResult Login([FromBody] StudentRequest logData)
        {
           
            var student = db.Auth(logData.Name, logData.Password);
            if (student != null)
            {
                var token = Generate(student);
                var TokenContext = GenerateRandomStr(25);
                var refTokenToResponse = new RefreshToken
                {
                    Used = false,
                    CreationTime = DateTime.UtcNow,
                    ExpiryData = DateTime.UtcNow.AddMonths(6),
                    StudentId = student.Id,
                    Student = student,
                    Token = TokenContext + Guid.NewGuid()
                };
                db.saveToken(refTokenToResponse);

                TokenResponse resp = new TokenResponse
                {
                    RefreshToken = refTokenToResponse.Token,
                    AccesToken = Generate(student)
                };

                return Ok(resp);
            }
            return NotFound("User not found");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh([FromBody] TokenRequest token)
        {
            string refreshToken = token.Token;

            var storedToken = db.FindToken(refreshToken);
            if (storedToken == null)
            {
                return NotFound("token not found");
            }
            if(storedToken.ExpiryData < DateTime.UtcNow)
            {
                return BadRequest("time out");
            }
            if (storedToken.Used)
            {
                return BadRequest("used");
            }

            db.setUsed(storedToken);

            var user = db.getUserByToken(storedToken);
            if (user == null)
            {
                return BadRequest("user not found");
            }
            var TokenContext = GenerateRandomStr(25);
            var refTokenToResponse = new RefreshToken
            {
                Used = false,
                CreationTime = DateTime.UtcNow,
                ExpiryData = DateTime.UtcNow.AddMonths(6),
                StudentId = user.Id,
                Student = user,
                Token = TokenContext + Guid.NewGuid()
            };
            db.saveToken(refTokenToResponse);

            TokenResponse resp = new TokenResponse
            {
                RefreshToken = refTokenToResponse.Token,
                AccesToken = Generate(user)
            };

            return Ok(resp); 
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("logout")]
        public IActionResult Logout([FromBody] TokenRequest token)
        {
            string refreshToken = token.Token;

            var storedToken = db.FindToken(refreshToken);
            if (storedToken == null)
            {
                return NotFound("token not found");
            }

            if (db.deleteToken(refreshToken))
            {
                return Ok("deleted");
            }

            return BadRequest("token not found");
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("reg")]
        public IActionResult Register([FromBody] StudentRequest student)
        {
            string msg = db.Reg(student.Name, student.Password, student.Email);
            //var isReg = true;
            if(msg != "created")
            {
                return BadRequest(msg);
            }
            return Ok(msg);
        }
        private string Generate(User student)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_conf["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, student.Name),
                new Claim(ClaimTypes.Email, student.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, student.Role?.Name)
            };
            var token = new JwtSecurityToken(_conf["Jwt:Issuer"],
                _conf["Jwt:Audience"],
                claims, expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRandomStr (int len)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[len];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }
}
