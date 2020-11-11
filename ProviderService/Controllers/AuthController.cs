using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using BusinessLogic.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ProviderService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(ILogger<AuthController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel user)
        {
            if (user == null)
                return BadRequest("Invalid request");

            var LoginUser = await _unitOfWork.Users.LoginUser(user);

            //TODO: Need to add email, role to claim
            if (LoginUser != null)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("top$ecretK3y@123"));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


                var claims = new List<Claim>
                    {   new Claim(ClaimTypes.Email, LoginUser.Email, ClaimValueTypes.Email),
                        new Claim(ClaimTypes.Role, LoginUser.Role, ClaimValueTypes.String),
                        new Claim(ClaimTypes.NameIdentifier, LoginUser.Id.ToString(), ClaimValueTypes.String)
                    };

                var jwtTokenOptions = new JwtSecurityToken(
                     issuer: "http://localhost:5001",
                     audience: "http://localhost:5001",
                     claims: claims,                     
                     expires: DateTime.Now.AddMinutes(10),
                     signingCredentials: signingCredentials
                    );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtTokenOptions);
                return Ok(new { Token = tokenString });
            };

            return Unauthorized();
        }
    }
}
