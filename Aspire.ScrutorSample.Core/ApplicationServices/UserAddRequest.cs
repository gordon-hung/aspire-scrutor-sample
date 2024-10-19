using MediatR;

namespace Aspire.ScrutorSample.Core.ApplicationServices;

public record UserAddRequest(
	string Username,
	string Password) : IRequest<string>;
