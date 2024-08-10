using Application.CQRS.Auth;
using Application.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class AuthController : ApiBaseController
{
    public AuthController(IMediator mediator) : base(mediator){}

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        var result = await _mediator.Send(new RegisterCommand(request));

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request));

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [AllowAnonymous]
    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var result = await _mediator.Send(new ConfirmEmailCommand(userId, token));

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [AllowAnonymous]
    [HttpPost("ForgetPassword")]
    public async Task<IActionResult> ForgetPassword(string email)
    {
        var result = await _mediator.Send(new ForgetPasswordCommand(email));

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequest request)
    {
        var result = await _mediator.Send(new ResetPasswordCommand(request));

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}

