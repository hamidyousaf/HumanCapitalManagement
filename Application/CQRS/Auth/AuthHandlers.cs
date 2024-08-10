using Domain.Abstractions.UnitOfWork;
using Domain.DTOs.Responces;
using Domain.Entities;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;
using Application.CQRS.Email;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Application.DTOs.Responces;

namespace Application.CQRS.Auth;

public sealed class RegisterCommandHandlers : IRequestHandler<RegisterCommand, Result<IEnumerable<string>>>
{
    private readonly UserManager<User> _userManger;
    private IConfiguration _configuration;
    private readonly IMediator _mediator;

    public RegisterCommandHandlers(UserManager<User> userManger, IConfiguration configuration, IMediator mediator)
    {
        _userManger = userManger;
        _configuration = configuration;
        _mediator = mediator;
    }

    public async Task<Result<IEnumerable<string>>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {

        // Compare the Password and ConfirmPassword fields.
        if (request.Register.Password != request.Register.ConfirmPassword)
        {
            return Result<IEnumerable<string>>.Fail("Confirm password doesn't match the password");
        }

        var user = new User
        {
            Email = request.Register.Email,
            UserName = string.Concat(request.Register.PhoneRegion, request.Register.Phone),
            FirstName = request.Register.FirstName,
            LastName = request.Register.LastName,
            EmailConfirmed = false,
            PhoneNumber = string.Concat(request.Register.PhoneRegion, request.Register.Phone),
            PhoneNumberConfirmed = false,
        };

        var result = await _userManger.CreateAsync(user, request.Register.Password);

        if (result.Succeeded)
        {
            var confirmEmailToken = await _userManger.GenerateEmailConfirmationTokenAsync(user);

            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{_configuration["AppUrl"]}/api/auth/confirmemail?userid={user.Id}&token={validEmailToken}";
            string content = $"<h1>Welcome to {AppConst.AppName}</h1>" +
                $"<p>Please confirm your email by <a href='{url}'>Clicking here</a></p>";

            await _mediator.Send(new SendEmailCommand(user.Email, "Confirm your email", content));

            // Add role to user
            await _userManger.AddToRoleAsync(user, "User");

            return Result<IEnumerable<string>>.Success(new List<string>(), "User created successfully!");
        }
        return Result<IEnumerable<string>>.Fail("User did not create", result.Errors.Select(e => e.Description));
    }
}

public sealed class LoginCommandHandlers : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly UserManager<User> _userManger;
    private IConfiguration _configuration;

    public LoginCommandHandlers(
        UserManager<User> userManger,
        IConfiguration configuration)
    {
        _userManger = userManger;
        _configuration = configuration;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = new User();

        if (string.IsNullOrEmpty(request.Login.MobileNo))
        {
            // login with email.
            var userResult = await _userManger.FindByEmailAsync(request.Login.Email);
            if (userResult == null)
            {
                return Result<LoginResponse>.Fail("There is no user with that Email address");
            }
            user = userResult;
        }
        else
        {
            // Login with mobile no.
            var userResult = await _userManger.FindByNameAsync(request.Login.MobileNo);
            if (userResult == null)
            {
                return Result<LoginResponse>.Fail("There is no user with that User Name");
            }
            user = userResult;
        }

        // check is password correct.
        var result = await _userManger.CheckPasswordAsync(user, request.Login.Password);
        if (!result)
        {
            return Result<LoginResponse>.Fail("Invalid password");
        }

        // add claims in jwt token.
        var claims = new[] {
                new Claim("Email", request.Login.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));
        var token = new JwtSecurityToken(
            issuer: _configuration["AuthSettings:Issuer"],
            audience: _configuration["AuthSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

        LoginResponse response = new LoginResponse()
        {
            ExpireDate = token.ValidTo,
            Token = tokenAsString
        };

        // return response.
        return Result<LoginResponse>.Success(response, "User logged in successfully!");
    }
}

public sealed class ConfirmEmailCommandHandlers : IRequestHandler<ConfirmEmailCommand, Result<IEnumerable<string>>>
{
    private readonly UserManager<User> _userManger;

    public ConfirmEmailCommandHandlers(
        UserManager<User> userManger)
    {
        _userManger = userManger;
    }

    public async Task<Result<IEnumerable<string>>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        // userId and token is null or empty.
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Token))
        {
            return Result<IEnumerable<string>>.Fail("Invalid userid or token.");
        }

        // check user exists.
        var user = await _userManger.FindByIdAsync(request.UserId);
        if (user is null)
        {
            return Result<IEnumerable<string>>.Fail("User not found.");
        }

        // decode the token.
        var decodedToken = WebEncoders.Base64UrlDecode(request.Token);
        string normalToken = Encoding.UTF8.GetString(decodedToken);

        // confirm the email.
        var result = await _userManger.ConfirmEmailAsync(user, normalToken);

        if (result.Succeeded)
        {
            return Result<IEnumerable<string>>.Success(new List<string>(), "Email confirmed successfully!");
        }

        return Result<IEnumerable<string>>.Fail("Email did not confirm.", result.Errors.Select(e => e.Description));
    }
}

public sealed class ForgetPasswordCommandHandlers : IRequestHandler<ForgetPasswordCommand, Result<bool>>
{
    private readonly UserManager<User> _userManger;
    private IConfiguration _configuration;
    private readonly IMediator _mediator;

    public ForgetPasswordCommandHandlers(UserManager<User> userManger, IConfiguration configuration, IMediator mediator)
    {
        _userManger = userManger;
        _configuration = configuration;
        _mediator = mediator;
    }

    public async Task<Result<bool>> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
    {
        // check email is correct.
        if (string.IsNullOrEmpty(request.Email))
        {
            return Result<bool>.Fail("Invalid email.");
        }

        // check eser exists.
        var user = await _userManger.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Result<bool>.Fail("No user associated with email.");
        }

        // generate token to send on email.
        var token = await _userManger.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Encoding.UTF8.GetBytes(token);
        var validToken = WebEncoders.Base64UrlEncode(encodedToken);

        string url = $"{_configuration["AppUrl"]}/ResetPassword?email={request.Email}&token={validToken}";

        // send email.
        string content = $"<h1>Follow the instructions to reset your password</h1>" +
            $"<p>To reset your password <a href='{url}'>Click here</a></p>";

        await _mediator.Send(new SendEmailCommand(request.Email, "Reset Password", content));

        return Result<bool>.Success(true, "Reset password URL has been sent to the email successfully!");
    }
}
public sealed class ResetPasswordCommandHandlers : IRequestHandler<ResetPasswordCommand, Result<IEnumerable<string>>>
{
    private readonly UserManager<User> _userManger;
    private IConfiguration _configuration;
    private readonly IMediator _mediator;

    public ResetPasswordCommandHandlers(UserManager<User> userManger, IConfiguration configuration, IMediator mediator)
    {
        _userManger = userManger;
        _configuration = configuration;
        _mediator = mediator;
    }

    public async Task<Result<IEnumerable<string>>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // check eser exists.
        var user = await _userManger.FindByEmailAsync(request.ResetPassword.Email);
        if (user is null)
        {
            return Result<IEnumerable<string>>.Fail("No user associated with email.");
        }

        // Compare the Password and ConfirmPassword fields.
        if (request.ResetPassword.NewPassword != request.ResetPassword.ConfirmPassword)
        {
            return Result<IEnumerable<string>>.Fail("Password doesn't match its confirmation.");
        }

        // decode the token.
        var decodedToken = WebEncoders.Base64UrlDecode(request.ResetPassword.Token);
        string normalToken = Encoding.UTF8.GetString(decodedToken);

        // reset the password.
        var result = await _userManger.ResetPasswordAsync(user, normalToken, request.ResetPassword.NewPassword);
        if (result.Succeeded)
        {
            return Result<IEnumerable<string>>.Success(new List<string>(), "Password has been reset successfully!");
        }

        return Result<IEnumerable<string>>.Fail("Something went wrong", result.Errors.Select(e => e.Description));
    }
}