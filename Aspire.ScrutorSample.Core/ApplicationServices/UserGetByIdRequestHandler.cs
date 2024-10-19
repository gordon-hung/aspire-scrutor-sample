﻿using MediatR;

namespace Aspire.ScrutorSample.Core.ApplicationServices;

internal class UserGetByIdRequestHandler(
	IUserRepository repository) : IRequestHandler<UserGetByIdRequest, UserInfoResponse?>
{
	public async Task<UserInfoResponse?> Handle(UserGetByIdRequest request, CancellationToken cancellationToken)
	{
		var info = await repository.GetAsync(request.Id.ToLower(), cancellationToken).ConfigureAwait(false);

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
