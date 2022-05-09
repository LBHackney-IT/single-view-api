using System;
using ServiceStack.Redis;

namespace SingleViewApi.V1.Gateways
{
    public class RedisGateway : IRedisGateway
    {
        private readonly string _host;

        public RedisGateway(string host)
        {
            _host = host;
        }

        public dynamic DoTheThing(string input)
        {
            try
            {
                var redis = new RedisClient(_host, 6379);
                using (redis)
                {
                    Console.WriteLine(" ------ REDIS CLIENT ------");

                    try
                    {
                        var redisUsers = redis.As<RedisPerson>();
                        Console.WriteLine(" ------ REDIS USER ------");


                        var sequence = redisUsers.GetNextSequence();
                        Console.WriteLine(" ------ got sequence ------");

                        var user = new RedisPerson { Id = sequence, Name = input };
                        Console.WriteLine(" ------ new user ------");

                        redisUsers.Store(user);

                        Console.WriteLine(" ------ STORED ------");

                        var allUsers = redisUsers.GetAll();
                        Console.WriteLine(" ------ ALL USERS ------");

                        return allUsers.Count + " " + allUsers[allUsers.Count - 1];
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(" ------ SEQUENCE ERROR ------");
                        Console.WriteLine(err);
                        return "SEQUENCE ERROR";

                    }


                }
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
class RedisPerson
{
    public long Id { get; set; }
    public string Name { get; set; }
    public override string ToString()
    {
        return $"{Id} - {Name}";
    }
}
