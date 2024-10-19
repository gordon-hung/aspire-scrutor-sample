using Aspire.ScrutorSample.Core.Enums;

namespace Aspire.ScrutorSample.Core.Models;

public record UserInfo(
	string Id,
	string Username,
	string Password,
	UserState State,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdateAt);
