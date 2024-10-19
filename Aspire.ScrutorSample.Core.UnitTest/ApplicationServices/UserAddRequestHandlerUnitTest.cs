using Aspire.ScrutorSample.Core.ApplicationServices;
using NSubstitute;

namespace Aspire.ScrutorSample.Core.UnitTest.ApplicationServices;

public class UserAddRequestHandlerUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGenerator = Substitute.For<IUserIdGenerator>();
		var fakeHasher = Substitute.For<IPasswordHasher>();
		var fakeRepository = Substitute.For<IUserRepository>();

		var request = new UserAddRequest(
			Username: "Gordon_Hung",
			Password: "1qaz2wsx");

		var id = "4uo9qvf7jdpe";
		_ = fakeGenerator.NewId()
			.Returns(id);

		var hashedPassword = "$2a$10$6y8BGUE0MI9caEF5xH0i6un0G7Gb2lzFFRNfGzSoY50sxjIws./NO";
		_ = fakeHasher.HashPassword(
			plainPassword: Arg.Is(request.Password))
			.Returns(hashedPassword);

		var sut = new UserAddRequestHandler(
			fakeGenerator,
			fakeHasher,
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(id, actual);

		_ = fakeRepository
			.Received(1)
			.AddAsync(
			id: Arg.Is(id),
			username: Arg.Is(request.Username.ToLower()),
			hashedPassword: Arg.Is(hashedPassword),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
