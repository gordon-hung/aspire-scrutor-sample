using Aspire.ScrutorSample.Core.ApplicationServices;
using Aspire.ScrutorSample.Core.Enums;
using Aspire.ScrutorSample.Core.Models;
using NSubstitute;

namespace Aspire.ScrutorSample.Core.UnitTest.ApplicationServices;

public class UserGetByUsernameRequestHandlerUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeRepository = Substitute.For<IUserRepository>();

		var request = new UserGetByUsernameRequest(
			Username: "Gordon_Hung");

		var id = "4uo9qvf7jdpe";
		_ = fakeRepository.GetIdAsync(
			username: Arg.Is(request.Username.ToLower()),
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

		var sut = new UserGetByUsernameRequestHandler(
			fakeRepository);

		var cancellationTokenSource = new CancellationTokenSource();
		var token = cancellationTokenSource.Token;

		var actual = await sut.Handle(request, token);

		Assert.NotNull(actual);
		Assert.Equal(info.Id, actual.Id);
		Assert.Equal(info.Username, actual.Username);
		Assert.Equal(info.State, actual.State);
		Assert.Equal(info.CreatedAt, actual.CreatedAt);
		Assert.Equal(info.UpdateAt, actual.UpdateAt);
	}
}
