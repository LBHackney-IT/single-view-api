using System;
using Hackney.Core.Logging;
using ServiceStack.Redis;

namespace SingleViewApi.V1.Gateways
{

    public class RedisGateway : IRedisGateway
    {
        private readonly IRedisClient _redisClient;

        public RedisGateway(IRedisClient redisClient)
        {
            _redisClient = redisClient;
        }
        [LogCall]
        public string AddValue(string value, int ttlDays = 1)
        {
            var id = Guid.NewGuid().ToString();

            var ttl = new TimeSpan(ttlDays, 0, 0, 0);

            _redisClient.SetValue(id, value, ttl);

            return id;
        }
        [LogCall]
        public string GetValue(string id)
        {
            Console.WriteLine($"Getting value for {id}");
            var value = _redisClient.Get<string>(id);
            Console.WriteLine($"Got value {value}");
            return value;
        }
    }
}
