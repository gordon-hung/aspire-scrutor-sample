using MediatR;

namespace Aspire.ScrutorSample.Core.ApplicationServices;
public record UserGetByUsernameRequest(
	string Username) : IRequest<UserInfoResponse?>;
