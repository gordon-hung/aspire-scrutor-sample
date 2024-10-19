using Aspire.ScrutorSample.Core;
using Aspire.ScrutorSample.Repositories;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddScrutorSampleRepository(
		this IServiceCollection services,
		Func<IServiceProvider, IMongoClient> mongoClientResolver,
		Func<IServiceProvider, IDatabase> redisDatabaseResolver)
		=> services
		.AddSingleton(TimeProvider.System)
		.AddScoped<IUserIdGenerator, UserIdGenerator>()
		.AddScoped<IPasswordHasher, PasswordHasher>()
		.AddTransient<IUserRepository>((sp) => ActivatorUtilities.CreateInstance<UserRepository>(sp, mongoClientResolver(sp)))
		.Decorate<IUserRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<UserRedisCacheGetRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(1)))
		.Decorate<IUserRepository>((innerRepository, sp) => ActivatorUtilities.CreateInstance<UserRedisCacheGetIdRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromMinutes(3)));
}
