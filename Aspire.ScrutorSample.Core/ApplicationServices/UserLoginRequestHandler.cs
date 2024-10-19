using MediatR;

namespace Aspire.ScrutorSample.Core.ApplicationServices;

internal class UserLoginRequestHandler(
	IPasswordHasher hasher,
	IUserRepository repository) : IRequestHandler<UserLoginRequest, bool>
{
	public async Task<bool> Handle(UserLoginRequest request, CancellationToken cancellationToken)
	{
		var id = await repository.GetIdAsync(
			username: request.Username,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		ArgumentException.ThrowIfNullOrWhiteSpace(id);

		var info = await repository.GetAsync(
			id: id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(id);

		return hasher.VerifyPassword(request.Password, info.Password);
	}
}
