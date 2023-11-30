using DAir.Context;
using DAir.DTO;
using DAir.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DAir.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DAirDbContext _context;
        private readonly UserManager<ApiUser> _userManager;
        private readonly SignInManager<ApiUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
        DAirDbContext context,
        ILogger<AccountController> logger,
        IConfiguration configuration,
        UserManager<ApiUser> userManager,
        SignInManager<ApiUser> signInManager)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpPost]
        [Authorize(Policy = "adminPolicy")]
        public async Task<ActionResult> Register(RegisterDTO input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newUser = new ApiUser();
                    newUser.UserName = input.Email;
                    newUser.Email = input.Email;
                    newUser.FullName = input.FullName;
                    var result = await _userManager.CreateAsync(
                    newUser, input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(
                        "User {userName} ({email}) has been created.",
                        newUser.UserName, newUser.Email);
                        return StatusCode(201,
                        $"User '{newUser.UserName}' has been created.");
                    }
                    else
                        throw new Exception(
                        string.Format("Error: {0}", string.Join(" ",
                        result.Errors.Select(e => e.Description))));
                }
                else
                {
                    var details = new ValidationProblemDetails(ModelState);
                    details.Type =
                    "https:/ /tools.ietf.org/html/rfc7231#section-6.5.1";
                    details.Status = StatusCodes.Status400BadRequest;
                    return new BadRequestObjectResult(details);
                }
            }
            catch (Exception e)
            {
                var exceptionDetails = new ProblemDetails();
                exceptionDetails.Detail = e.Message;
                exceptionDetails.Status =
                StatusCodes.Status500InternalServerError;
                exceptionDetails.Type =
                "https:/ /tools.ietf.org/html/rfc7231#section-6.6.1";
                return StatusCode(
                StatusCodes.Status500InternalServerError,
                exceptionDetails);
            }
        }

        [HttpPost]
        [Authorize(Policy = "adminPolicy")]
        public async Task<ActionResult> AssignClaimToUser(string userEmail, string role)
        {
            try
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(userEmail);

                if (user == null)
                {
                    return NotFound($"User with email '{userEmail}' not found.");
                }

                // Check if the claim already exists for the user
                var existingClaim = await _userManager.GetClaimsAsync(user);
                var claimType = role;

                if (existingClaim.Any(c => c.Type == claimType && c.Value == "true"))
                {
                    return BadRequest($"Claim '{claimType}' already exists for the user.");
                }

                // Add the new claim to the user
                var result = await _userManager.AddClaimAsync(user, new Claim(claimType, "true"));

                // Check if claim assignment was successful
                if (result.Succeeded)
                {
                    return Ok($"Claim '{claimType}' added to user '{userEmail}'.");
                }
                else
                {
                    return BadRequest($"Error assigning claim: {string.Join(", ", result.Errors)}");
                }
            }
            catch (Exception ex)
            {
                var exceptionDetails = new ProblemDetails
                {
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, exceptionDetails);
            }
        }




        [HttpPost]
        public async Task<ActionResult> Login(LoginDTO input)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByNameAsync(input.UserName);
                    if (user == null || !await _userManager.CheckPasswordAsync(user, input.Password))
                        throw new Exception("Invalid login attempt.");
                    else
                    {
                        // OBS: this should be used instead but signingkey results in null..

                        //var signingCredentials = new SigningCredentials(
                        //        new SymmetricSecurityKey(
                        //        System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"])),
                        //        SecurityAlgorithms.HmacSha256);

                        var signingCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(
                                System.Text.Encoding.UTF8.GetBytes("MyVeryOwnTestSigningKey123$")),
                                SecurityAlgorithms.HmacSha256);

                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));

                        // Check if the user has the "admin" claim
                        var adminClaim = (await _userManager.GetClaimsAsync(user))
                            .FirstOrDefault(c => c.Type == "admin" && c.Value == "true");

                        if (adminClaim != null)
                        {
                            // Add the "admin" claim to the claims list
                            claims.Add(adminClaim);
                        }

                        var jwtObject = new JwtSecurityToken(
                                issuer: _configuration["JWT:Issuer"],
                                audience: _configuration["JWT:Audience"],
                                claims: claims,
                                expires: DateTime.Now.AddSeconds(300),
                                signingCredentials: signingCredentials);
                        var jwtString = new JwtSecurityTokenHandler()
                        .WriteToken(jwtObject);
                        return StatusCode(StatusCodes.Status200OK, jwtString);
                    }
                }
                else
                {
                    var details = new ValidationProblemDetails(ModelState);
                    details.Type =
                    "https:/ /tools.ietf.org/html/rfc7231#section-6.5.1";
                    details.Status = StatusCodes.Status400BadRequest;
                    return new BadRequestObjectResult(details);
                }
            }
            catch (Exception e)
            {
                var exceptionDetails = new ProblemDetails();
                exceptionDetails.Detail = e.Message;
                exceptionDetails.Status =
                StatusCodes.Status401Unauthorized;
                exceptionDetails.Type =
                "https:/ /tools.ietf.org/html/rfc7231#section-6.6.1";
                return StatusCode(
                    StatusCodes.Status401Unauthorized, exceptionDetails);
            }
        }
    }
}