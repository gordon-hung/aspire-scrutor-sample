using Aspire.ScrutorSample.Core.Models;

namespace Aspire.ScrutorSample.Core;

public interface IUserRepository
{
	ValueTask AddAsync(string id, string username, string hashedPassword, CancellationToken cancellationToken = default);

	ValueTask<string?> GetIdAsync(string username, CancellationToken cancellationToken = default);

	ValueTask<UserInfo?> GetAsync(string id, CancellationToken cancellationToken = default);

	ValueTask UpdatePasswordAsync(string id, string hashedPassword, CancellationToken cancellationToken = default);

	ValueTask InitializationAsync(CancellationToken cancellationToken = default);
}
