using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using HartPR.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HartPR.Services;
using HartPR.Models;
using HartPR.Helpers;
using Microsoft.AspNetCore.Identity;

namespace HartPR.Controllers
{
    [Authorize]
    [Route("api/token")]
    public class TokenController : Controller
    {
        private readonly IConfiguration _configuration;
        private IHartPRRepository _hartPRRepository;

        public TokenController(IConfiguration configuration,
            IHartPRRepository hartPRRepository)
        {
            _configuration = configuration;
            _hartPRRepository = hartPRRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody] LoginViewModel login)
        {
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                return Ok(new { token = tokenString });
            }

            return Unauthorized();

        }

        private string BuildToken(User user)
        {
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.FirstName),
                 new Claim(JwtRegisteredClaimNames.Email, user.Email)
                 //TODO has claim admin, true or false as a property of the user entity, then display things conditionally? meh.  
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private User Authenticate(LoginViewModel userToCheck)
        {
            User user = _hartPRRepository.GetUserByEmail(userToCheck.Email);

            if (user != null && userToCheck.Password != null)
            {
                //TODO: Hash the entry password lmfao
                var Hasher = new PasswordHasher<User>();
                if (0 != Hasher.VerifyHashedPassword(user, user.Password, userToCheck.Password))
                {
                    return user;
                }
            }
            return null;
        }

        //TODO: Add Register to see how the hashing works, repository methods Get Users for testing.
        //TODO: Talk to Mitch about multiple hashes, which routes should be exposed

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public IActionResult HandleRegister([FromBody] RegisterViewModel model)
        {
            //TODO: If more checks are added, refactor into separate check
            User existingUser = _hartPRRepository.GetUserByEmail(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(model.Email, "User already exists!");
            }

            if (ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                User newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    IsAdmin = false,
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now),
                };
                newUser.Password = Hasher.HashPassword(newUser, model.Password);

                _hartPRRepository.AddUser(newUser);

                if (!_hartPRRepository.Save())
                {
                    throw new Exception("Creating an user failed on save.");
                    // return StatusCode(500, "A problem happened with handling your request.");
                }

                var loginModel = new LoginViewModel()
                {
                    Email = model.Email,
                    Password = model.Password
                };

                return CreateToken(loginModel);
            }

            return new UnprocessableEntityObjectResult(ModelState);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("Here comes some secret content!");
        }

        [Authorize]
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            return Ok(_hartPRRepository.GetUsers());
        }
        
    }
}
