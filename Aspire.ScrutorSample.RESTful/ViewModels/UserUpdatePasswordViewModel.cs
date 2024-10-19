using System.ComponentModel.DataAnnotations;

namespace Aspire.ScrutorSample.RESTful.ViewModels;

public record UserUpdatePasswordViewModel
{
	[Required]
	[StringLength(maximumLength: 64, MinimumLength = 8)]
	public string Password { get; init; } = default!;
}
