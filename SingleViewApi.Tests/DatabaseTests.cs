using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;
using SingleViewApi.V1.Infrastructure;

namespace SingleViewApi.Tests;

[TestFixture]
public class DatabaseTests
{
    [SetUp]
    public void RunBeforeAnyTests()
    {
        var builder = new DbContextOptionsBuilder();
        builder.UseNpgsql(ConnectionString.TestDatabase());
        SingleViewContext = new SingleViewContext(builder.Options);

        SingleViewContext.Database.EnsureCreated();
        _transaction = SingleViewContext.Database.BeginTransaction();
    }

    [TearDown]
    public void RunAfterAnyTests()
    {
        _transaction.Rollback();
        _transaction.Dispose();
    }

    private IDbContextTransaction _transaction;
    protected SingleViewContext SingleViewContext { get; private set; }
}
