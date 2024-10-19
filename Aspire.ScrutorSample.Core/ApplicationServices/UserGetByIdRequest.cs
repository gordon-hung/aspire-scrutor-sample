using MediatR;

namespace Aspire.ScrutorSample.Core.ApplicationServices;
public record UserGetByIdRequest(
	string Id) : IRequest<UserInfoResponse?>;
