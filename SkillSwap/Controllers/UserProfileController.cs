using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SkillSwap.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SkillSwap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        private readonly ILogger<UserProfileController> appLogger;

        public UserProfileController ( ApplicationDbContext context, ILogger<UserProfileController> logger )
        {
            db = context;
            appLogger = logger;
        }

        [HttpPost]
        [Route("addProfile")]
        public async Task<ActionResult> SetProfile ( [FromForm] ProfileExtended profile )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Helper.GetApiUrl());
                    string? authorizationHeader = HttpContext.Request.Headers ["Authorization"];
                    if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
                    {
                        string token = authorizationHeader.Substring("Bearer ".Length).Trim();
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                        if (jwtToken != null)
                        {
                            DateTime? expirationDate = jwtToken.ValidTo;
                            if (expirationDate != null && DateTime.Now >= expirationDate)
                            {
                                apiResult.IsSuccessfull = false;
                                apiResult.Message = "Token has been expired";
                                return Unauthorized(apiResult);
                            }
                            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                            var IsUserExists = await db.Users.Where(x => x.Id == profile.UserId).FirstOrDefaultAsync();
                            if (IsUserExists != null)
                            {
                                var existingProfile = await db.Profiles.FirstOrDefaultAsync(p => p.UserId == profile.UserId);
                                if (existingProfile == null)
                                {
                                    byte [] profileImageBytes;
                                    using (var ms = new MemoryStream())
                                    {
                                        await profile.ProfileImage.CopyToAsync(ms);
                                        profileImageBytes = ms.ToArray();
                                    }
                                    Profile profileData = new Profile
                                    {
                                        UserId = profile.UserId,
                                        Address = profile.Address,
                                        DateOfBirth = profile.DateOfBirth,
                                        Email = profile.Email,
                                        FirstName = profile.FirstName,
                                        LastName = profile.LastName,
                                        Phone = profile.Phone,
                                        ProfileImage = Convert.ToBase64String(profileImageBytes),
                                        CreatedBy = userName,
                                        CreatedDate = DateTime.Now,
                                        IsActive = true
                                    };
                                    db.Profiles.Add(profileData);
                                    int i = await db.SaveChangesAsync();
                                    if (i > 0)
                                    {
                                        apiResult.Data = Array.Empty<object>();
                                        apiResult.IsSuccessfull = true;
                                        apiResult.Message = "Data Added To Profile Successfully";
                                    }
                                    else
                                    {
                                        apiResult.Data = Array.Empty<object>();
                                        apiResult.IsSuccessfull = false;
                                        apiResult.Message = "Failed to Add Profile Data";
                                    }
                                }
                                else
                                {
                                    apiResult.Data = Array.Empty<object>();
                                    apiResult.IsSuccessfull = false;
                                    apiResult.Message = "User Profile already added Please Update";
                                }
                            }
                        }
                        else
                        {
                            apiResult.IsSuccessfull = false;
                            apiResult.Message = "Invalid token";
                            return Unauthorized(apiResult);
                        }
                    }
                    else
                    {
                        // Authorization header is missing or invalid
                        apiResult.IsSuccessfull = false;
                        apiResult.Message = "Missing or invalid authorization header";
                        return Unauthorized(apiResult);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] {ex.Message}");
                Log.Error("-----------------------------------------");
                apiResult.IsSuccessfull = false;
                apiResult.Message = "An error occurred while processing the token";
                return StatusCode(500, apiResult);
            }
            // apiResult.Url = HttpContext.Request.Host.Value + HttpContext.Request.Path;
            return Ok(apiResult);
        }

        [HttpGet]
        [Route("profilebyid")]
        public async Task<ActionResult> ProfileById ( string userId )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var userData = await db.Profiles.Where(x => x.UserId == userId && x.IsActive).FirstOrDefaultAsync();
                if (userData != null)
                {
                    apiResult.Data = userData;
                    apiResult.IsSuccessfull = true;
                    apiResult.Message = "User's Profile Data found";
                }
                else
                {
                    apiResult.Data = Array.Empty<object>();
                    apiResult.IsSuccessfull = false;
                    apiResult.Message = "User's Profile Data not Found";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(apiResult);
        }

        [HttpPut]
        [Route("updateprofile")]
        public async Task<ActionResult> UpdateProfile ( [FromForm] ProfileExtended profile )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var profileData = await db.Profiles.Where(x => x.UserId == profile.UserId && x.IsActive).FirstOrDefaultAsync();
                if (profileData != null)
                {
                    byte [] profileImageBytes;
                    using (var ms = new MemoryStream())
                    {
                        profile.ProfileImage.CopyTo(ms);
                        profileImageBytes = ms.ToArray();
                    }
                    profileData.Address = profile.Address;
                    profileData.DateOfBirth = profile.DateOfBirth;
                    profileData.Email = profile.Email;
                    profileData.FirstName = profile.FirstName;
                    profileData.LastName = profile.LastName;
                    profileData.Phone = profile.Phone;
                    profileData.UpdatedBy = User.FindFirstValue(ClaimTypes.Name);
                    profileData.UpdatedDate = DateTime.Now;
                    profileData.ProfileImage = Convert.ToBase64String(profileImageBytes);
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        apiResult.Data = Array.Empty<object>();
                        apiResult.IsSuccessfull = true;
                        apiResult.Message = "User Profile Successfully Updated";
                    }
                    else
                    {
                        apiResult.Data = Array.Empty<object>();
                        apiResult.IsSuccessfull = false;
                        apiResult.Message = "Failed to Update User Profile";
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(apiResult);
        }

        [HttpGet]
        [Route("viewprofile")]
        public async Task<ActionResult> ViewProfile ( string userId )
        {
            ApiResult apiResult = new ApiResult();
            try
            {
                var model = await db.Profiles.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                if (model != null)
                {
                    apiResult.Data = model;
                    apiResult.IsSuccessfull = true;
                    apiResult.Message = "Profile data found";
                }
                else
                {
                    apiResult.Data = Array.Empty<string>();
                    apiResult.IsSuccessfull = false;
                    apiResult.Message = "Profile data not found";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(apiResult);
        }

        [HttpDelete]
        [Route("deleteprofile")]
        public async Task<ActionResult> DeleteProfile ( string userId )
        {
            ApiResult apiResult = new ApiResult();
            Profile profile = new Profile();
            try
            {
                var activeProfileUser = await db.Profiles.Where(x => x.IsActive && x.UserId == userId).FirstOrDefaultAsync();
                if (activeProfileUser != null)
                {
                    activeProfileUser.IsActive = false;
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        apiResult.Data = Array.Empty<object>();
                        apiResult.IsSuccessfull = true;
                        apiResult.Message = "Deleted Successfully";
                    }
                    else
                    {
                        apiResult.Data = Array.Empty<object>();
                        apiResult.IsSuccessfull = false;
                        apiResult.Message = "Failed to Delete User profile";
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Ok(apiResult);
        }

    }
}
