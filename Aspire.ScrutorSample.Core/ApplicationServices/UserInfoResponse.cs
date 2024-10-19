using Aspire.ScrutorSample.Core.Enums;

namespace Aspire.ScrutorSample.Core.ApplicationServices;
public record UserInfoResponse(
	string Id,
	string Username,
	UserState State,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdateAt);
