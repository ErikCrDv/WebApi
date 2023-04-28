using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.DTOs;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(UserAuth userAuth)
        {
            var user = new IdentityUser()
            {
                UserName = userAuth.Email,
                Email = userAuth.Email,
            };

            var result = await userManager.CreateAsync(user, userAuth.Password);

            if (result.Succeeded)
            {
                return generateToken(userAuth);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(UserAuth userAuth)
        {
            var result = await signInManager.PasswordSignInAsync(userAuth.Email, userAuth.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return generateToken(userAuth);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        [HttpGet("renew")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public  ActionResult<AuthResponse> Renew()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var userAuth = new UserAuth()
            {
                Email = email
            };

            return generateToken(userAuth);
        }


        private AuthResponse generateToken(UserAuth userAuth)    
        {
            var claims = new List<Claim>()
            {
                new Claim("email",userAuth.Email),
            };

            var key = new SymmetricSecurityKey( Encoding.UTF8.GetBytes(configuration["secretKeyJWT"]) );
            var creds = new SigningCredentials( key, SecurityAlgorithms.HmacSha256 );
            var expiration = DateTime.UtcNow.AddMinutes(30);

            var securityToken = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
                );

            return new AuthResponse()
            { 
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken), 
                Expiration= expiration 
            };
        }
  
    }
}
