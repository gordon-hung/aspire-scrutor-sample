using Aspire.ScrutorSample.Core;

using Aspire.ScrutorSample.Core.ApplicationServices;
using Aspire.ScrutorSample.RESTful.ViewModels;
using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Aspire.ScrutorSample.RESTful.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	/// <summary>
	/// Creates new id.
	/// </summary>
	/// <param name="userIdGenerator">The user identifier generator.</param>
	/// <returns></returns>
	[HttpGet("new-id")]
	public ValueTask<string> NewId(
		[FromServices] IUserIdGenerator userIdGenerator)
	{
		return ValueTask.FromResult(userIdGenerator.NewId());
	}

	/// <summary>
	/// Users the add asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost]
	public async ValueTask<string> UserAddAsync(
		[FromServices] IMediator mediator,
		[FromBody] UserAddViewModel source)
		=> await mediator.Send(
			request: new UserAddRequest(
				Username: source.UserName,
				Password: source.Password),
		cancellationToken: HttpContext.RequestAborted)
		.ConfigureAwait(false);

	/// <summary>
	/// Users the get by identifier asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="id">The identifier.</param>
	/// <returns></returns>
	[HttpGet("{id}/id")]
	public async ValueTask<UserInfoViewModel?> UserGetByIdAsync(
		[FromServices] IMediator mediator,
		string id)
	{
		var response = await mediator.Send(
			request: new UserGetRequest(Id: id),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return response is null
			? null
			: new UserInfoViewModel(
				response.Id,
				response.Username,
				response.State,
				response.CreatedAt,
				response.UpdateAt);
	}

	/// <summary>
	/// Users the get by username asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="username">The username.</param>
	/// <returns></returns>
	[HttpGet("{username}/username")]
	public async ValueTask<UserInfoViewModel?> UserGetByUsernameAsync(
		[FromServices] IMediator mediator,
		string username = "Gordon_Hung")
	{
		var response = await mediator.Send(
			request: new UserGetByUsernameRequest(Username: username),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return response is null
			? null
			: new UserInfoViewModel(
				response.Id,
				response.Username,
				response.State,
				response.CreatedAt,
				response.UpdateAt);
	}

	/// <summary>
	/// Users the update password asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="id">The identifier.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPut("{id}/password")]
	public async ValueTask UserUpdatePasswordAsync(
		[FromServices] IMediator mediator,
		string id,
		[FromBody] UserUpdatePasswordViewModel source)
		=> await mediator.Send(
			request: new UserUpdatePasswordRequest(
				Id: id,
				Password: source.Password),
			cancellationToken: HttpContext.RequestAborted)
		.ConfigureAwait(false);

	/// <summary>
	/// Users the login asynchronous.
	/// </summary>
	/// <param name="mediator">The mediator.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("login")]
	public async ValueTask<bool> UserLoginAsync(
		[FromServices] IMediator mediator,
		[FromBody] UserLoginViewModel source)
		=> await mediator.Send(
			request: new UserLoginRequest(
				Username: source.UserName,
				Password: source.Password),
		cancellationToken: HttpContext.RequestAborted)
		.ConfigureAwait(false);
}
