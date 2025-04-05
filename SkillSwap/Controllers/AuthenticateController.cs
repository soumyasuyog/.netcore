using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkillSwap.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkillSwap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController (
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        //[HttpPost]
        //[Route("token")]
        //public async Task<IActionResult> Token ( [FromForm] LoginModel model )
        //{
        //    var user = await _userManager.FindByNameAsync(model.Username);
        //    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        //    {
        //        var userRoles = await _userManager.GetRolesAsync(user);
        //        var authClaims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, user.UserName),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        };

        //        foreach (var userRole in userRoles)
        //        {
        //            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        //        }

        //        var token = GetToken(authClaims);
        //        var apiResult = new ApiResult
        //        {
        //            IsSuccessfull = true,
        //            Message = "Token Generated Successfully",
        //            Data = new
        //            {
        //                AccessToken  = new JwtSecurityTokenHandler().WriteToken(token),
        //                GeneratedAt  = token.ValidFrom.ToString("yyyy-MM-dd HH:mm:ss"),
        //                ExpirationTime = token.ValidTo.ToString("yyyy-MM-dd HH:mm:ss")
        //            }
        //        };
        //        return Ok(apiResult);
        //    }
        //    return Unauthorized();
        //}

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> Token ( [FromForm] LoginModel model )
        {
            try
            {
                if (model.grant_type != "password") {
                    return Ok(new ApiResult
                    {
                        Data = Array.Empty<object>(),
                        IsSuccessfull = false,
                        Message = "Invalid grant_type. Only 'password' grant_type is supported."
                    });
                }
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        //new Claim(ClaimTypes.Role, userRole),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = GetToken(authClaims, user.UserName);
                    var apiResult = new ApiResult
                    {
                        IsSuccessfull = true,
                        Message = "Token Generated Successfully",
                        Data = new
                        {
                            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                            GeneratedAt = token.ValidFrom.ToString("yyyy-MM-dd HH:mm:ss"),
                            ExpirationTime = token.ValidTo.ToString("yyyy-MM-dd HH:mm:ss")
                        }
                    };
                    return Ok(apiResult);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Unauthorized(new ApiResult
            {
                IsSuccessfull = false,
                Message = "Unauthorized. Incorrect username or password.",
                Data = Array.Empty<object>()
            });
        }


        [HttpGet]
        [Route("register")]
        public async Task<IActionResult> Register ( [FromBody] RegisterModel model )
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.Username);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

                IdentityUser user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResult { IsSuccessfull = false, Message = "User creation failed! Please check user details and try again.", Data = Array.Empty<object>() });
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(new ApiResult { IsSuccessfull = true, Message = "User created successfully! ", Data = Array.Empty<object>() });
        }

        [HttpGet]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin ( [FromBody] RegisterModel model )
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        //private JwtSecurityToken GetToken1 ( List<Claim> authClaims )
        //{
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration ["JWT:Secret"] ?? ""));
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(authClaims),
        //        Expires = DateTime.Now.AddMinutes(30),
        //        NotBefore = DateTime.Now,
        //        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        //    };
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var token = (JwtSecurityToken) tokenHandler.CreateToken(tokenDescriptor);


        //    // Calculate the Unix timestamp for the ValidFrom time
        //    var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //    var nbf = (int) (tokenDescriptor.NotBefore.Value - unixEpoch).TotalSeconds;
        //    token.Payload ["nbf"] = nbf;

        //    // Calculate the Unix timestamp for the expiration time
        //    var exp = (int) (tokenDescriptor.Expires.Value - unixEpoch).TotalSeconds;
        //    token.Payload ["exp"] = exp;

        //    return token;
        //}

        private JwtSecurityToken GetToken ( List<Claim> authClaims, string userName )
        {
            authClaims.Add(new Claim(ClaimTypes.Name, userName));
            //authClaims.Add(new Claim(ClaimTypes.Role, ));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration ["JWT:Secret"] ?? ""));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.Now.AddMinutes(30),
                NotBefore = DateTime.Now,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = (JwtSecurityToken) tokenHandler.CreateToken(tokenDescriptor);

            // Calculate the Unix timestamp for the ValidFrom time
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var nbf = (int) (tokenDescriptor.NotBefore.Value - unixEpoch).TotalSeconds;
            token.Payload ["nbf"] = nbf;

            // Calculate the Unix timestamp for the expiration time
            var exp = (int) (tokenDescriptor.Expires.Value - unixEpoch).TotalSeconds;
            token.Payload ["exp"] = exp;

            return token;
        }

    }
}
