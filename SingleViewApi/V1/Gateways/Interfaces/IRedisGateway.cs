namespace SingleViewApi.V1.Gateways.Interfaces
{
    public interface IRedisGateway
    {
        string AddValue(string value, int ttlDays);
        void AddValueWithKey(string key, string value, int ttlHours);
        string GetValue(string id);

    }
}
