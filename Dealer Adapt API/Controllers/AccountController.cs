using Dealer_Adapt_API.Models.IdentityUserModels;
using Dealer_Adapt_API.Services;
using Dealer_Adapt_API.Servicesl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Dealer_Adapt_API.Controllers;


/// <summary>
/// Controller for handling user account operations such as registration, login, logout, etc.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    /// <summary>
    /// Constructor for AccountController class that initializes the UserManager, SignInManager, and EmailSender services.
    /// </summary>
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtService _jwtService;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        RoleManager<IdentityRole> roleManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }




    /// <summary>
    /// Registers a new user with the specified email, password, first name, and last name.
    /// Sends a confirmation email to the user to verify their email address.
    /// </summary>
    /// <param name="model"></param>
    /// <returns> a success message if registration is successful.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            // Check if email is in the correct format
            if (!IsValidEmail(model.Email))
            {
                ModelState.AddModelError("Register", "Invalid email format");
                return BadRequest(ModelState);
            }

            // Check if the email is already taken
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                DoB = model.DoB,

                EmailConfirmed = true // Temporary, set to true to skip email confirmation.
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            //if (result.Succeeded)
            //{
            //    //// Check if this is the first user // check later maybe wil remove this part
            //    //if (_userManager.Users.Count() == 1)
            //    //{
            //    //    // Ensure the "BusinessOwner" role exists
            //    //    if (!await _roleManager.RoleExistsAsync("BusinessOwner"))
            //    //    {
            //    //        await _roleManager.CreateAsync(new IdentityRole("BusinessOwner"));
            //    //    }

            //    //    // Assign the "BusinessOwner" role to the first user
            //    //    await _userManager.AddToRoleAsync(user, "BusinessOwner");
            //    //}
            //    //// //////////////////////////////////

            //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);
            //    await _emailSender.SendEmailAsync(user.Email
            //        , "Confirm your email"
            //        , $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>." // , "do.not.reply@dealeradapt.ca"                                                                        
            //        , "Email Confirmation Dealer Adapt");

            //    return Ok("Registration successful. Please check your email to confirm your account.");
            //}

            if (result.Succeeded)
            {
                return Ok("Registration successful. Your email is confirm.");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return BadRequest(ModelState);
    }





    /// <summary>
    /// Logs in a user with the specified email and password.
    /// </summary>
    /// <param name="model"></param>
    /// <returns> a success message if login is successful.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("LogIn", "Email not Exist");
                return BadRequest(ModelState);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("LogIn", "Email not confirmed.");
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var id = user.Id;
                var firstName = user.FirstName;
                var lastName = user.LastName;
                var email = user.Email;
                var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? ""; // Ensure role is not null
                var token = _jwtService.GenerateJwtToken(user, role);
                var refreshToken = _jwtService.GenerateRefreshToken();
                var isEmailConfirmed = user.EmailConfirmed;

                // Store the refresh token with the user
                await _userManager.SetAuthenticationTokenAsync(user, "DealerAdapt", "RefreshToken", refreshToken);

                return Ok(new
                {
                    id,
                    firstName,
                    lastName,
                    email,
                    role,
                    isEmailConfirmed,
                    token,
                    refreshToken,
                });
            }

            if (result.IsLockedOut)
            {
                return Forbid();
            }
            else
            {
                ModelState.AddModelError("LogIn", "Invalid login attempt.");
            }
        }

        return BadRequest(ModelState);
    }







    /// <summary>
    /// Logs out the currently authenticated user.
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }





    /// <summary>
    /// Confirms the email address of a user with the specified token and email.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="email"></param>
    /// <returns> a success message if email confirmation is successful.</returns>
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BadRequest("Invalid email confirmation request.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return Ok("Email confirmed successfully.");
        }

        return BadRequest("Email confirmation failed.");
    }




    /// <summary>
    /// Assigns a role to a user with the specified email.
    /// </summary>
    /// <param name="model"></param>
    /// <returns>A success message if role assignment is successful.</returns>
    [HttpPost("assign-role")]
    [Authorize(Roles = "BusinessOwner")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var roleExists = await _roleManager.RoleExistsAsync(model.Role); // check if role exists
        if (!roleExists)
        {
            // Remove role from user
            if (model.Role == "remove")
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(user, model.Role);
                if (!removeResult.Succeeded)
                {
                    return BadRequest("Failed to remove role from user.");
                }
                return Ok("Role removed successfully");
            }
            return BadRequest("Role does not exist.");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
        {
            // Remove the Old roles from the user
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest("Failed to remove role from user.");
            }
        }

        // Assign the role to the user
        var addResult = await _userManager.AddToRoleAsync(user, model.Role);
        if (addResult.Succeeded)
        {
            return Ok("Role assigned successfully");
        }

        return BadRequest("Failed to assign new role to user");
    }







    /// <summary>
    /// Removes a role from a user with the specified email.
    /// </summary>
    /// <param name="email"> The email of the user to remove the role from.</param>
    /// <returns> returns a success message if role removal is successful.</returns>
    [HttpPost("remove-role")]
    [Authorize(Roles = "BusinessOwner")]
    public async Task<IActionResult> RemoveRole([FromBody] IdEmailModel idEmail)
    {

        var user = await _userManager.FindByEmailAsync(idEmail.Email);
        if (user == null)
        {
            return NotFound("User not found");
        }


        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Count > 0)
        {
            // Remove the Old roles from the user
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (removeResult.Succeeded)
            {
                return Ok("Role removed successfully");
            }
        }
        return BadRequest("Failed to remove role from user.");
    }





    /// <summary>
    /// Gets all accounts that have any role in the Roles table.
    /// </summary>
    /// <returns>A list of users with roles.</returns>
    [HttpGet("users-with-roles")]
    [Authorize(Roles = "BusinessOwner")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var usersWithRoles = new List<object>();

        foreach (var role in _roleManager.Roles)
        {

            IList<ApplicationUser> usersInRole = await _userManager.GetUsersInRoleAsync(role.Name ?? "");
            foreach (var user in usersInRole)
            {
                usersWithRoles.Add(new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    DoB = user.DoB,
                    IsEmailConfirmed = user.EmailConfirmed,
                    Role = role.Name
                });
            }
        }

        return Ok(usersWithRoles);
    }






    /// <summary>
    /// Gets user information by their JWT token.
    /// </summary>
    /// <returns>User information if the token is valid.</returns>
    [HttpGet("current-user")]
    [Authorize]
    public async Task<IActionResult> GetUserInfo()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Token is missing.");
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        if (jwtToken == null)
        {
            return BadRequest("Invalid token.");
        }

        var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return BadRequest("Invalid token.");
        }

        var user = await _userManager.FindByIdAsync(userIdClaim.Value);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var userInfo = new
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsEmailConfirmed = user.EmailConfirmed,
            Role = roles.FirstOrDefault() ?? ""
        };

        return Ok(userInfo);
    }









    /// <summary>
    /// Finds a user by email and returns their information.
    /// </summary>
    /// <param name="email"></param>
    /// <returns>User information if found.</returns>
    [HttpGet("find-user-by-email")]
    [Authorize(Roles = "BusinessOwner")]
    public async Task<IActionResult> FindUserByEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var userInfo = new
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            DoB = user.DoB,
            IsEmailConfirmed = user.EmailConfirmed,
            Roles = roles
        };

        return Ok(userInfo);
    }




    /// <summary>
    /// Gets all roles in the AspNetRoles table.
    /// </summary>
    /// <returns>A list of roles.</returns>
    [HttpGet("roles")]
    [Authorize(Roles = "BusinessOwner")]
    public async Task<IActionResult> GetRoles()
    {
        var roleNames = await _roleManager.Roles
                                      .Select(r => r.Name)
                                      .ToListAsync();
        return Ok(roleNames);
    }







    /// <summary>
    /// Refreshes the JWT token of a user with the specified email and refresh token.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("Invalid email.");
        }

        var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "DealerAdapt", "RefreshToken");
        if (storedRefreshToken != model.RefreshToken)
        {
            return BadRequest("Invalid refresh token.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? ""; // Ensure role is not null
        var token = _jwtService.GenerateJwtToken(user, role);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Update the refresh token in the AspNetUserTokens table
        await _userManager.SetAuthenticationTokenAsync(user, "DealerAdapt", "RefreshToken", newRefreshToken);

        return Ok(new
        {
            token,
            refreshToken = newRefreshToken
        });
    }



    // ... Helper methods
    private bool IsValidEmail(string email)
    {
        // Use regular expression to validate email format
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        return Regex.IsMatch(email, pattern);
    }

}
