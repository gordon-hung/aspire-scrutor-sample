using MediatR;

namespace Aspire.ScrutorSample.Core.ApplicationServices;
public record UserGetRequest(
	string Id) : IRequest<UserInfoResponse?>;
