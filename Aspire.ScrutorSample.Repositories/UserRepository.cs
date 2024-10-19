using Aspire.ScrutorSample.Core;
using Aspire.ScrutorSample.Core.Enums;
using Aspire.ScrutorSample.Core.Models;
using Aspire.ScrutorSample.Repositories.Extensions;
using Aspire.ScrutorSample.Repositories.Models;

using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Aspire.ScrutorSample.Repositories;

internal class UserRepository(
	ILogger<UserRepository> logger,
	TimeProvider timeProvider,
	IMongoClient client) : IUserRepository
{
	public async ValueTask AddAsync(string id, string username, string hashedPassword, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var currentAt = timeProvider.GetUtcNow();
		var document = new User
		{
			Id = id,
			Username = username,
			Password = hashedPassword,
			State = (int)UserState.Enable,
			CreatedAt = currentAt.UtcDateTime,
			UpdateAt = currentAt.UtcDateTime
		};

		await collection.InsertOneAsync(
			document: document,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<UserInfo?> GetAsync(string id, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var builder = Builders<User>.Filter;
		var filter = builder.Eq(x => x.Id, id);

		var user = (await collection.FindAsync<User>(
			filter: filter,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
			.FirstOrDefault(cancellationToken: cancellationToken);

		return user is null
			? null
			: new UserInfo(
				user.Id,
				user.Username,
				user.Password,
				(UserState)user.State,
				user.CreatedAt,
				user.UpdateAt);
	}

	public async ValueTask<string?> GetIdAsync(string username, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var builder = Builders<User>.Filter;
		var filter = builder.Regex(x => x.Username, new BsonRegularExpression($"^{username}$", "i"));

		var user = (await collection.FindAsync<User>(
			filter: filter,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
			.FirstOrDefault(cancellationToken: cancellationToken);

		return user is null
			? null
			: user.Id;
	}

	public async ValueTask UpdatePasswordAsync(string id, string hashedPassword, CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();
		var currentAt = timeProvider.GetUtcNow();
		var builder = Builders<User>.Filter;
		var filter = builder.Eq(x => x.Id, id);

		var update = Builders<User>.Update
			.Set(s => s.Password, hashedPassword)
			.Set(s => s.UpdateAt, currentAt.UtcDateTime);

		_ = await collection.UpdateOneAsync(
			filter: filter,
			update: update,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask InitializationAsync(CancellationToken cancellationToken = default)
	{
		var collection = GetCollection();

		var tasks = new Task[]{
			Task.Run(async () =>
			{
				try
				{
					_ = await Index_UniqueAsync( collection: collection,   cancellationToken: cancellationToken).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "LogOn:{logOn} Message:This index creation failed.{index}",  DateTime.Now.ToString("u"),"Index_Unique");
				}
			}, cancellationToken: cancellationToken)
		};

		await Task.WhenAll(tasks).ConfigureAwait(false);
	}

	private static Task<string> Index_UniqueAsync(IMongoCollection<User> collection, CancellationToken cancellationToken = default)
	{
		var index = Builders<User>.IndexKeys
			.Ascending(m => m.Username);

		var createIndexOptions = new CreateIndexOptions
		{
			Unique = true,
			Name = $"ux_{collection.CollectionNamespace.CollectionName}"
		};
		var createIndexModel = new CreateIndexModel<User>(index, createIndexOptions);

		return collection.Indexes.CreateOneAsync(
			model: createIndexModel,
			cancellationToken: cancellationToken);
	}

	private IMongoCollection<User> GetCollection()
		=> client
		.GetDatabase("sample")
		.GetCollection<User>(typeof(User).GetTableName());
}
