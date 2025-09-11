using backendProject.Data.SqlDbContext;
using backendProject.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P0_ClassLibrary.Interfaces;

namespace backendProject.Controllers.UserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly SqlDbContext DbContext;
        public readonly ITokenService tokenService;
        public UserController(SqlDbContext dbContext, ITokenService tokenService)
        {
            this.DbContext = dbContext;
            this.tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(User req)
        {
            if (string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.Password))
            {
                return BadRequest(new { message = "All Details Are Required !" });
            }

            var existingUser = await DbContext.Users.FirstOrDefaultAsync(user => user.Email == req.Email);

            if (existingUser != null)
            {
                return BadRequest(new { message = "User Already Exists !" });
            }
            var encryptedPass = BCrypt.Net.BCrypt.HashPassword(req.Password);
            req.Password = encryptedPass;
            var newUser = await DbContext.Users.AddAsync(req);
            await DbContext.SaveChangesAsync();
            return Ok(new { message = "User Registered Sucessfully !" });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(User req)
        {
            if (string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.Password))
            {
                return BadRequest(new { message = "All Details Are Required !" });
            }

            var user = await DbContext.Users.FirstOrDefaultAsync(User => User.Email == req.Email);
            if (user == null)
            {
                return BadRequest(new { message = "User Not Found !" });
            }
            var verifyPass = BCrypt.Net.BCrypt.Verify(req.Password, user.Password);
            if (!verifyPass)
            {
                return BadRequest(new { message = "Password Incorrect !" });
            }
            var token = tokenService.CreateToken(user.UserId, user.Email, user.Username?? ("unknown"), 60 * 24);
            return Ok(new { message = "Logged In Sucessfully !",payload = user,token });

        }
    }
}