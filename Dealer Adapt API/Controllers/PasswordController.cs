using Dealer_Adapt_API.Models.IdentityUserModels;
using Dealer_Adapt_API.Services;
using Dealer_Adapt_API.Servicesl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dealer_Adapt_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IUserService _userService;

    public PasswordController(UserManager<ApplicationUser> userManager, IEmailSender emailService, IUserService userService)
    {
        _userManager = userManager;
        _emailSender = emailService;
        _userService = userService;
    }

    [HttpPost("forgot")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        ApplicationUser? user = await _userService.GetUserByEmailAsync(model.Email);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ResetPassword", "Password", new { token, email = user.Email }, Request.Scheme);

        // await _emailSender.SendEmailAsync(user.Email, "Password Reset", $"Reset your password using this link: {resetLink}");
        await _emailSender.SendEmailAsync(user.Email
                   , "Password Reset"
                   , $"Reset your password by clicking <a href='{resetLink}'>here</a>."//);
                  //  , "do.not.reply@dealeradapt.ca"
                    , "Password Reset Dealer Adapt");

        return Ok("Password reset link has been sent to your email. token:" + token);
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel resetPasswordModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
        if (user == null)
        {
            return BadRequest("User not found.");
        }


        var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest("Password reset failed.");
        }

        return Ok("Password has been reset successfully.");
    }
}
