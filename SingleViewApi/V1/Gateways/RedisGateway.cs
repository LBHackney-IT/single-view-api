using System;
using ServiceStack.Redis;
using StackExchange.Redis;
//
// NEED TO UPDATE TO VERSION 6
// using Amazon.ElastiCacheCluster;
//
// using Enyim.Caching;
// using Enyim.Caching.Memcached;
//
// ElastiCacheClusterConfig config = new ElastiCacheClusterConfig();
// MemcachedClient memClient = new MemcachedClient(config);
//
//
// // nstall failed (project: SingleViewApi, package: Amazon.ElastiCacheCluster v1.0.0)
// // Package restore failed. Rolling back package changes for 'SingleViewApi'.
// // Package 'Amazon.ElastiCacheCluster 1.0.0' was restored using '.NETFramework,Version=v4.6.1,
// // .NETFramework,Version=v4.6.2, .NETFramework,Version=v4.7,
// // .NETFramework,Version=v4.7.1, .NETFramework,Version=v4.7.2,
// // .NETFramework ... mework,Version=v4.6.1, .NETFramework,Version=v4.6.2,
// // .NETFramework,Version=v4.7, .NETFramework,Version=v4.7.1, .NETFramework,Version=v4.7.2,
// // .NETFramework,Version=v4.8' instead of the project target framework '.NETCoreApp,Version=v3.1'.
// // This package may not be fully compatible with your project.
namespace SingleViewApi.V1.Gateways
{
    public class RedisGateway : IRedisGateway
    {
        private readonly string _host;
        // private readonly IDatabase _db;

        public RedisGateway(string host)
        {
            _host = host;

            Console.WriteLine(" ------ LOG ME PLZ ------");
        }

        public string DoTheThing(string input)
        {
            Console.WriteLine(" ------ DOING THE THING ------");

            ConnectionMultiplexer redis;
            try
            {
                Console.WriteLine(" ------ MAKING CONNECTION ------");
                var configuration = $"{_host},ssl=true";
                Console.WriteLine(configuration);
                redis = ConnectionMultiplexer.Connect(configuration);
            }
            catch (Exception e)
            {
                Console.WriteLine(" ------ CONNECTION ERROR------");
                Console.WriteLine(e);
                return "connection error";
            }

            IDatabase db;
            try
            {

                Console.WriteLine(" ------ MAKING DB ------");
                db = redis.GetDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine(" ------ DB ERROR------");
                Console.WriteLine(e);
                return "db error";
            }

            try
            {

                var key = "new Guid().ToString()";
                Console.WriteLine($" ------ SWEET NEW KEY BABY: {key} ------");

                var ttl = new TimeSpan(0, 0, 15, 0);
                Console.WriteLine(" ------ REDIS starting to add ------");


                db.StringSet(key, input, ttl);
                Console.WriteLine(" ------ REDIS KEY CREATED ------");

                string value = db.StringGet(key);
                Console.WriteLine(" ------ GOT VALUE ------");

                Console.WriteLine(value); // writes: "abcdefg"
                return $"{key} - {value}";
            }
            catch (Exception e)
            {
                Console.WriteLine(" ------ REDIS error ------");
                Console.WriteLine(e);

                return "Oops! something went wrong";

            }
        }
    }
}
