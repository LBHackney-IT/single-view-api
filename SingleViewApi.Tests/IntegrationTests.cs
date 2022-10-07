using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using NUnit.Framework;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.Tests;

public class IntegrationTests<TStartup> where TStartup : class
{
    private DbContextOptionsBuilder _builder;
    private NpgsqlConnection _connection;

    private MockWebApplicationFactory<TStartup> _factory;
    private IDbContextTransaction _transaction;
    protected HttpClient Client { get; private set; }
    protected SingleViewContext DatabaseContext { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _connection = new NpgsqlConnection(ConnectionString.TestDatabase());
        _connection.Open();
        var npgsqlCommand = _connection.CreateCommand();
        npgsqlCommand.CommandText = "SET deadlock_timeout TO 30";
        npgsqlCommand.ExecuteNonQuery();

        _builder = new DbContextOptionsBuilder();
        _builder.UseNpgsql(_connection);
    }

    [SetUp]
    public void BaseSetup()
    {
        _factory = new MockWebApplicationFactory<TStartup>(_connection);
        Client = _factory.CreateClient();
        DatabaseContext = new SingleViewContext(_builder.Options);
        DatabaseContext.Database.EnsureCreated();
        _transaction = DatabaseContext.Database.BeginTransaction();
    }

    [TearDown]
    public void BaseTearDown()
    {
        Client.Dispose();
        _factory.Dispose();
        _transaction.Rollback();
        _transaction.Dispose();
    }
}
