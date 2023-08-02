using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMeetupAPI.Entities;
using MMeetupAPI.Identity;
using MMeetupAPI.Models;

namespace MMeetupAPI.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly MeetupContext meetupContext;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IJwtProvider jwtProvider;

        public AccountController(MeetupContext meetupContext, IPasswordHasher<User> passwordHasher, IJwtProvider jwtProvider)
        {
            this.meetupContext = meetupContext;
            this.passwordHasher = passwordHasher;
            this.jwtProvider = jwtProvider;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] UserLoginDto userLoginDto)
        {
            var user = this.meetupContext.Users
                .Include(user => user.Role)
                .FirstOrDefault(user => user.Email == userLoginDto.Email);

            if (user == null)
            {
                return BadRequest("Invalid username or password");
            }

            var passwordVerificationResult = this.passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userLoginDto.PAssword);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return BadRequest("Invalid username or passwrod");
            }

            var token = this.jwtProvider.GenerateJwtToken(user);

            return Ok(token);
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }



            var newUser = new User()
            {
                Email = registerUserDto.Email,
                Nationality = registerUserDto.Nationality,
                DateOfBirth = registerUserDto.DateOfBirth,
                RoleId = registerUserDto.RoleId
            };

            var passwordHash = this.passwordHasher.HashPassword(newUser, registerUserDto.Password);
            newUser.PasswordHash = passwordHash;

            this.meetupContext.Users.Add(newUser);
            this.meetupContext.SaveChanges();

            return Ok();
        }


    }
}
