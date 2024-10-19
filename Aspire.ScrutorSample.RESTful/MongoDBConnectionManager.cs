using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Aspire.ScrutorSample.RESTful;

internal class MongoDBConnectionManager(IMongoClient mongoClient)
{
	public IMongoClient MongoClient => mongoClient;
}
