using System;
using ServiceStack.Redis;
using StackExchange.Redis;

namespace SingleViewApi.V1.Gateways
{
    public class RedisGateway : IRedisGateway
    {
        // private readonly string _host;
        private readonly IDatabase _db;

        public RedisGateway(string host)
        {
            // _host = host;
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(host);
            _db = redis.GetDatabase();
        }

        public string DoTheThing(string input)
        {
            var key = new Guid().ToString();
            Console.WriteLine($" ------ SWEET NEW KEY BABY: {key} ------");

            var ttl = new TimeSpan(0, 0, 15, 0);
            Console.WriteLine(" ------ REDIS starting to add ------");

            try
            {
                _db.StringSet(key, input, ttl);
                Console.WriteLine(" ------ REDIS KEY CREATED ------");

                string value = _db.StringGet(key);
                Console.WriteLine(" ------ GOT VALUE ------");

                Console.WriteLine(value); // writes: "abcdefg"
                return $"{key} - {value}";
            }
            catch (Exception e)
            {
                Console.WriteLine(" ------ REDIS error ------");
                Console.WriteLine(e.StackTrace);

                return "Oops! something went wrong";

            }
        }
    }
}
