var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("Mongo");
var mongodb = mongo.AddDatabase("Mongodb");

var redis = builder.AddRedis("Redis").WithRedisCommander();

builder.AddProject<Projects.Aspire_ScrutorSample_RESTful>("aspire-scrutorsample-restful")
	.WithReference(mongodb)
	.WithReference(redis);

builder.Build().Run();
