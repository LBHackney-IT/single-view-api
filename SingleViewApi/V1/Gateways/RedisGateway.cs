using System;
using ServiceStack.Redis;

namespace SingleViewApi.V1.Gateways
{
    public class RedisGateway : IRedisGateway
    {
        private readonly RedisManagerPool _redisManager;

        public RedisGateway(string host)
        {
            _redisManager = new RedisManagerPool(host);
        }

        public string AddValue(string value)
        {
            using var client = _redisManager.GetClient();

            var id = Guid.NewGuid().ToString();

            client.Set(id, value);

            return id;
        }

        public string GetValue(string id)
        {
            using var client = _redisManager.GetClient();

            var value = client.Get<string>(id);

            return value;
        }
    }
}
