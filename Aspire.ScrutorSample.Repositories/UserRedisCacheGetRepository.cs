using System.Text.Json;
using System.Text.Json.Serialization;
using Aspire.ScrutorSample.Core;
using Aspire.ScrutorSample.Core.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Aspire.ScrutorSample.Repositories;

internal class UserRedisCacheGetRepository(
	IUserRepository repository,
	IDatabase database,
	TimeSpan expiry,
	ILogger<UserRedisCacheGetRepository> logger) : IUserRepository
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	public ValueTask AddAsync(string id, string username, string hashedPassword, CancellationToken cancellationToken = default)
		=> repository.AddAsync(id, username, hashedPassword, cancellationToken);

	public async ValueTask<UserInfo?> GetAsync(string id, CancellationToken cancellationToken = default)
	{
		try
		{
			var info = await StringGetAsync(id).ConfigureAwait(false)
				?? await repository.GetAsync(id, cancellationToken).ConfigureAwait(false);

			if (info is not null)
			{
				await StringSetAsync(info).ConfigureAwait(false);
			}

			return info;
		}
		catch (RedisCommandException ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					Message = "This exception occurs."
				}));

			return await repository.GetAsync(id, cancellationToken).ConfigureAwait(false);
		}
		catch (RedisTimeoutException ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					Message = "This exception occurs."
				}));

			return await repository.GetAsync(id, cancellationToken).ConfigureAwait(false);
		}
		catch (RedisException ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					Message = "This exception occurs."
				}));

			return await repository.GetAsync(id, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					Message = "This exception occurs."
				}));

			throw;
		}
	}

	public ValueTask<string?> GetIdAsync(string username, CancellationToken cancellationToken = default)
		=> repository.GetIdAsync(username, cancellationToken);

	public ValueTask InitializationAsync(CancellationToken cancellationToken = default)
		=> repository.InitializationAsync(cancellationToken);

	public async ValueTask UpdatePasswordAsync(string id, string hashedPassword, CancellationToken cancellationToken = default)
	{
		try
		{
			await repository.UpdatePasswordAsync(id, hashedPassword, cancellationToken).ConfigureAwait(false);
			await KeyDeleteAsync(id).ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "{logError}",
				JsonSerializer.Serialize(new
				{
					LogOn = DateTime.Now.ToString("u"),
					Id = id,
					Message = "This exception occurs."
				}));

			throw;
		}
	}

	private static string GetKey(string id)
		=> string.Concat("sample", "user", ":", "get", ":", id);

	private async ValueTask KeyDeleteAsync(string id)
	{
		var key = GetKey(id);

		_ = await database.KeyDeleteAsync(
			key: key,
			flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);
	}

	private async ValueTask StringSetAsync(UserInfo info)
	{
		var key = GetKey(info.Id);

		_ = await database.StringSetAsync(
			key: key,
			value: JsonSerializer.Serialize(info, _SerializerOptions),
			expiry: expiry,
			When.NotExists,
			flags: CommandFlags.FireAndForget)
			.ConfigureAwait(false);
	}

	private async ValueTask<UserInfo?> StringGetAsync(string id)
	{
		var key = GetKey(id);

		if (await database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			var redisValue = await database.StringGetAsync(key).ConfigureAwait(false);

			return string.IsNullOrWhiteSpace(redisValue) ? null : JsonSerializer.Deserialize<UserInfo?>(redisValue!, _SerializerOptions);
		}

		return null;
	}
}
