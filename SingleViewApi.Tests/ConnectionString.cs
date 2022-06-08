using System;

namespace SingleViewApi.Tests
{
    public static class ConnectionString
    {
        public static string TestDatabase()
        {
            return Environment.GetEnvironmentVariable("CONNECTION_STRING")
                   ?? "Host=127.0.0.1;Port=5432;Database=testdb;Username=postgres;Password=mypassword";
        }
    }
}
