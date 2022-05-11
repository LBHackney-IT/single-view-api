using System;
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

        public string AddValue(string value, int ttlDays = 1)
        {
            var id = Guid.NewGuid().ToString();

            var ttl = new TimeSpan(ttlDays, 0, 0, 0);

            _redisClient.SetValue(id, value, ttl);

            return id;
        }

        public string GetValue(string id)
        {
            var value = _redisClient.Get<string>(id);

            return value;
        }
    }
}
