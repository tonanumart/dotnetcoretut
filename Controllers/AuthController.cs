using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using DatingApp.API.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository repo;
        private readonly IConfiguration conf;

        public AuthController(IAuthRepository repo,IConfiguration conf)
        {
            this.repo = repo;
            this.conf = conf;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto user)
        {

            user.Username = user.Username.ToLower();

            bool isExistUsr = await repo.UserExist(user.Username);
            if(isExistUsr)
                ModelState.AddModelError("Username","Username is already exists");

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var userCreating = new User(){
              Username = user.Username
            };
            var createdUser = repo.Register(userCreating,user.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLoginDto user)
        {    
            var userFromRepo = await repo.Login(user.Username.ToLower(),user.Password);
            if(userFromRepo == null)
                return Unauthorized();

            var tokenString = JWTUtils.CreateToken(userFromRepo,conf);
            return Ok(new {tokenString});
        }

    }
}