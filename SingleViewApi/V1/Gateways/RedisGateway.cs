using System;
using ServiceStack.Redis;
using StackExchange.Redis;

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
                Console.WriteLine(_host);
                redis = ConnectionMultiplexer.Connect(_host);
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
