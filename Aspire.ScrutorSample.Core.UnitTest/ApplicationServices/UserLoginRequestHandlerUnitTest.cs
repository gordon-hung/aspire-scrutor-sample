using Aspire.ScrutorSample.Core.ApplicationServices;
using Aspire.ScrutorSample.Core.Enums;
using Aspire.ScrutorSample.Core.Models;
using NSubstitute;

namespace Aspire.ScrutorSample.Core.UnitTest.ApplicationServices;

public class UserLoginRequestHandlerUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeHasher = Substitute.For<IPasswordHasher>();
		var fakeRepository = Substitute.For<IUserRepository>();

		var request = new UserLoginRequest(
			Username: "Gordon_Hung",
			Password: "1qaz2wsx");

		var id = "4uo9qvf7jdpe";
		_ = fakeRepository.GetIdAsync(
			username: Arg.Is(request.Username),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var info = new UserInfo(
			Id: id,
			Username: "gordon_hung",
			Password: "$2a$10$6y8BGUE0MI9caEF5xH0i6un0G7Gb2lzFFRNfGzSoY50sxjIws./NO",
			State: UserState.Enable,
			CreatedAt: DateTimeOffset.UtcNow,
			UpdateAt: DateTimeOffset.UtcNow);
		_ = fakeRepository.GetAsync(
			id: Arg.Is(id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(info);

		_ = fakeHasher.VerifyPassword(
			plainPassword: Arg.Is(request.Password),
			hashedPassword: Arg.Is(info.Password))
			.Returns(true);

		var sut = new UserLoginRequestHandler(
			fakeHasher,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.True(actual);
	}
}
