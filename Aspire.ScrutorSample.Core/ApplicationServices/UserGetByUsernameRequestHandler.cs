using MediatR;

namespace Aspire.ScrutorSample.Core.ApplicationServices;

internal class UserGetByUsernameRequestHandler(
	IUserRepository repository) : IRequestHandler<UserGetByUsernameRequest, UserInfoResponse?>
{
	public async Task<UserInfoResponse?> Handle(UserGetByUsernameRequest request, CancellationToken cancellationToken)
	{
		var id = await repository.GetIdAsync(request.Username, cancellationToken).ConfigureAwait(false);

		if (string.IsNullOrWhiteSpace(id))
		{
			return null;
		}

		var info = await repository.GetAsync(id, cancellationToken).ConfigureAwait(false);

		return info is null
			? null
			: new UserInfoResponse(
				info.Id,
				info.Username,
				info.State,
				info.CreatedAt,
				info.UpdateAt);
	}
}
