using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MovieBookingProject.DataTransfer;
using MovieBookingProject.Helpers;
using MovieBookingProject.Interfaces;
using MovieBookingProject.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MovieBookingProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMovieInterface _MovieRepository;
        private readonly ITicketInterface _TicketRepository;
        private readonly IConfiguration _config;
        private readonly IMongoCollection<User> Users;
        private readonly IEmailService _emailService;
        public UserController(IMovieInterface movieRepository, ITicketInterface ticketRepository, IConnectionWithMongoDb connectionSettings, IMongoClient mongoDBClient, IConfiguration config, IEmailService emailService)
        {
            var database = mongoDBClient.GetDatabase(connectionSettings.DatabaseName);
            Users = database.GetCollection<User>(connectionSettings.CollectionName[1]);
            _MovieRepository = movieRepository;
            _TicketRepository = ticketRepository;
            _config = config;
            _emailService = emailService;

        }
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] User userObj)
        {

            if (userObj == null)
            {
                return BadRequest();
            }
            var user = Users.Find(x => x.UserName == userObj.UserName).SingleOrDefault();
            if (user == null) { return NotFound(new { Message = "User Not Found!" }); }
            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }
            user.Token = CreateJwt(user);
            return Ok(new
            {
                Token = user.Token,
                Message = "Login Success"
            });
        }

        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] User userObj)
        {
            if (userObj == null) return BadRequest();
            if (CheckUserNameExists(userObj.UserName)) { return BadRequest(new { Message = "Username Already Exists." }); }
            if (CheckEmailExists(userObj.Email)) { return BadRequest(new { Message = "Email Already Exists." }); }
            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";
            Users.InsertOne(userObj);
            return Ok(new
            {
                Message = "User Registered!"
            });

        }



        [Authorize]
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            return Ok(Users.Find(movie => true).ToList());
        }
        private bool CheckUserNameExists(string username)
        {
            var user = Users.Find(u => u.UserName == username).SingleOrDefault();
            if (user == null) return false;
            return true;
        }
        private bool CheckEmailExists(string email)
        {
            var user = Users.Find(u => u.Email == email).SingleOrDefault();
            if (user == null) return false;
            return true;
        }
        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Created By Amit Tomar");
            var identity = new ClaimsIdentity(new Claim[]
            {
               new Claim(ClaimTypes.Role, user.Role),
               new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}")
            });
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
        [HttpPost("send-reset-email/{email}")]
        public IActionResult SendEmail(string email)
        {
            var user = Users.Find(a => a.Email == email).SingleOrDefault();
            if (user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email doesn't exist"
                });
            }
            User userFound = new User()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Password = user.Password,
                Token = user.Token,
                Role = user.Role,
                Email = user.Email
            };


            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            userFound.ResetPasswordToken = emailToken;
            userFound.ResetPasswordExpiry = DateTime.Now.AddMinutes(30);
            string from = _config["EmailSettings:From"];
            var emailModel = new EmailModel(email, "Reset Password", EmailBody.EmaiilStringBody(email, emailToken));
            _emailService.SendEmail(emailModel);

            Users.ReplaceOne(user.Id, userFound);
            return Ok(new
            {
                StatusCode = 200,
                Message = "Email Sent!"
            });


        }
        [HttpPost("reset-password")]
        public IActionResult ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var newToken = resetPasswordDto.EmailToken.Replace(" ", "+");
            var user = Users.Find(a => a.Email == resetPasswordDto.Email).SingleOrDefault();
            if (user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User doesn't exist"
                });
            }
            var tokenCode = user.ResetPasswordToken;
            User userFound = new User()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Password = "",
                Token = user.Token,
                Role = user.Role,
                Email = user.Email
            };

            DateTime emailTokenExpiry = user.ResetPasswordExpiry;
            if (tokenCode != resetPasswordDto.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid Reset link"
                });
            }
            userFound.Password = PasswordHasher.HashPassword(resetPasswordDto.NewPassword);
            Users.ReplaceOne(user.Id, userFound);
            return Ok(new
            {
                StatusCode = 200,
                Message = "Password Reset Successfully"
            });
        }
    }
}
