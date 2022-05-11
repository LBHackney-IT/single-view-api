using System;
using ServiceStack.Redis;

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

            RedisManagerPool manager;
            try
            {
                Console.WriteLine(" ------ MAKING CONNECTION ------");

                Console.WriteLine(_host);

                manager = new RedisManagerPool(_host);

            }
            catch (Exception e)
            {
                Console.WriteLine(" ------ CONNECTION ERROR------");
                Console.WriteLine(e);
                return "connection error";
            }

            try
            {

                Console.WriteLine(" ------ MAKING CLIENT ------");
                using (var client = manager.GetClient())
                {
                    try
                    {
                        Console.WriteLine(" ------ ADDING ------");
                        client.Set("foo", input);
                    }
                    catch (Exception errorAd)
                    {
                        Console.WriteLine(" ------ ERROR ADDING ------");
                        Console.WriteLine(errorAd);
                        return "'error adding'";
                    }

                    try
                    {
                        Console.WriteLine(" ------ GETTING ------");
                        var thing = client.Get<string>("foo");
                        var returnThing = $"foo={thing}";

                        Console.WriteLine(returnThing);

                        return returnThing;
                    }
                    catch (Exception errGet)
                    {
                        Console.WriteLine(" ------ ERROR GETTING ------");
                        Console.WriteLine(errGet);
                        return "error getting";
                    }
                }            }
            catch (Exception e)
            {
                Console.WriteLine(" ------ CLIENT ERROR------");
                Console.WriteLine(e);
                return "db error";
            }


        }
    }
}
