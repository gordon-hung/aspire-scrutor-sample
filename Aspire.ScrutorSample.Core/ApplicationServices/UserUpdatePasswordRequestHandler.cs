using MediatR;

namespace Aspire.ScrutorSample.Core.ApplicationServices;

internal class UserUpdatePasswordRequestHandler(
	IPasswordHasher hasher,
	IUserRepository repository) : IRequestHandler<UserUpdatePasswordRequest>
{
	public Task Handle(UserUpdatePasswordRequest request, CancellationToken cancellationToken)
	{
		var hashedPassword = hasher.HashPassword(request.Password);

		return repository.UpdatePasswordAsync(
			id: request.Id,
			hashedPassword: hashedPassword,
			cancellationToken: cancellationToken)
			.AsTask();
	}
}
