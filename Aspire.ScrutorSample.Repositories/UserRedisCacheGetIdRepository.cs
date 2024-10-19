using System.Text.Json;
using System.Text.Json.Serialization;
using Aspire.ScrutorSample.Core;
using Aspire.ScrutorSample.Core.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Aspire.ScrutorSample.Repositories;

internal class UserRedisCacheGetIdRepository(
	IUserRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<UserRedisCacheGetIdRepository> logger) : IUserRepository
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	public ValueTask AddAsync(string id, string username, string hashedPassword, CancellationToken cancellationToken = default)
		=> repository.AddAsync(id, username, hashedPassword, cancellationToken);

	public ValueTask<UserInfo?> GetAsync(string id, CancellationToken cancellationToken = default)
		=> repository.GetAsync(id, cancellationToken);

	public async ValueTask<string?> GetIdAsync(string username, CancellationToken cancellationToken = default)

	{
		try
		{
			var id = await StringGetAsync(username).ConfigureAwait(false)
				?? await repository.GetIdAsync(username, cancellationToken).ConfigureAwait(false);

			if (!string.IsNullOrEmpty(id))
			{
				await StringSetAsync(username, id).ConfigureAwait(false);
			}

			return id;
		}
		catch (RedisCommandException ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Username = username,
					Message = "This exception occurs."
				}));

			return await repository.GetIdAsync(username, cancellationToken).ConfigureAwait(false);
		}
		catch (RedisTimeoutException ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Username = username,
					Message = "This exception occurs."
				}));

			return await repository.GetIdAsync(username, cancellationToken).ConfigureAwait(false);
		}
		catch (RedisException ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Username = username,
					Message = "This exception occurs."
				}));

			return await repository.GetIdAsync(username, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Username = username,
					Message = "This exception occurs."
				}));

			throw;
		}
	}

	public ValueTask InitializationAsync(CancellationToken cancellationToken = default)
		=> repository.InitializationAsync(cancellationToken);

	public ValueTask UpdatePasswordAsync(string id, string hashedPassword, CancellationToken cancellationToken = default)
		=> repository.UpdatePasswordAsync(id, hashedPassword, cancellationToken);

	private static string GetKey(string username)
		=> string.Concat("sample", "user", ":", "get", ":", "id", ":", username);

	private async ValueTask KeyDeleteAsync(string id)
	{
		var key = GetKey(id);

		_ = await database.KeyDeleteAsync(
			key: key,
			flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);
	}

	private async ValueTask StringSetAsync(string username, string id)
	{
		var key = GetKey(username);

		_ = await database.StringSetAsync(
			key: key,
			value: id,
			expiry: expiry,
			When.NotExists,
			flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);
	}

	private async ValueTask<string?> StringGetAsync(string username)
	{
		var key = GetKey(username);

		if (await database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			var redisValue = await database.StringGetAsync(key).ConfigureAwait(false);

			return string.IsNullOrWhiteSpace(redisValue) ? null : redisValue.ToString();
		}

		return null;
	}
}
