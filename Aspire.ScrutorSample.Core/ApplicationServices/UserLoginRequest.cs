using MediatR;

namespace Aspire.ScrutorSample.Core.ApplicationServices;
public record UserLoginRequest(
	string Username,
	string Password) : IRequest<bool>;
