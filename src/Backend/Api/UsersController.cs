using Backend.Auth;
using Backend.DataManagement.Users.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api;

[ApiController]
[Route("[controller]")]
public class UsersController(AuthService authService) : Controller
{
    [HttpGet("/login")]
    public IActionResult Login()
    {
        return Challenge(new AuthenticationProperties { RedirectUri = "/" }, AuthExtensions.AuthenticationScheme);
    }

    [HttpGet("/logout")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" }, AuthExtensions.CookieScheme);
    }

    [HttpDelete("/account")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<IActionResult> DeleteAccount(CancellationToken cancellationToken)
    {
        DeleteUserResult result = await authService.DeleteUserAccountAsync(HttpContext.User, cancellationToken);

        return result switch
        {
            DeleteUserResult.NotAuthenticated => Problem("User is not authenticated",
                                                         statusCode: StatusCodes.Status409Conflict),
            DeleteUserResult.UserNotInDb => SignOut(new AuthenticationProperties { RedirectUri = "/" },
                                                    AuthExtensions.CookieScheme),
            DeleteUserResult.SuccessDelete => SignOut(new AuthenticationProperties { RedirectUri = "/" },
                                                      AuthExtensions.CookieScheme),
            DeleteUserResult.Fail => Problem("Cannot delete user account now",
                                             statusCode: StatusCodes.Status409Conflict),
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpGet("/user-info")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<User>> GetUserInfo(CancellationToken cancellationToken)
    {
        return Ok(await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken));
    }
}
