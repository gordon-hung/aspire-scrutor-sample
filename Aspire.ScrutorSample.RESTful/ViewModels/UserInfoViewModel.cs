using Aspire.ScrutorSample.Core.Enums;

namespace Aspire.ScrutorSample.RESTful.ViewModels;

public record UserInfoViewModel(
	string Id,
	string Username,
	UserState State,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdateAt);
