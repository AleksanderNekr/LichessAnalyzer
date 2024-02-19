﻿using Backend.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api;

[ApiController]
[Route("[controller]")]
public class UsersController : Controller
{
    [HttpGet("/login")]
    public IActionResult Login()
    {
        return Challenge(new AuthenticationProperties { RedirectUri = "/" }, AuthExtensions.AuthenticationScheme);
    }
}
