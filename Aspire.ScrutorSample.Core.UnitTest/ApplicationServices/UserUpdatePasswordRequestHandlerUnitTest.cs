using Aspire.ScrutorSample.Core.ApplicationServices;
using NSubstitute;

namespace Aspire.ScrutorSample.Core.UnitTest.ApplicationServices;

public class UserUpdatePasswordRequestHandlerUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGenerator = Substitute.For<IUserIdGenerator>();
		var fakeHasher = Substitute.For<IPasswordHasher>();
		var fakeRepository = Substitute.For<IUserRepository>();

		var request = new UserUpdatePasswordRequest(
			Id: "4uo9qvf7jdpe",
			Password: "1qaz2wsx");

		var hashedPassword = "$2a$10$6y8BGUE0MI9caEF5xH0i6un0G7Gb2lzFFRNfGzSoY50sxjIws./NO";
		_ = fakeHasher.HashPassword(
			plainPassword: Arg.Is(request.Password))
			.Returns(hashedPassword);

		var sut = new UserUpdatePasswordRequestHandler(
			fakeHasher,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		await sut.Handle(request, token);

		_ = fakeRepository
			.Received(1)
			.UpdatePasswordAsync(
			id: Arg.Is(request.Id),
			hashedPassword: Arg.Is(hashedPassword),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
