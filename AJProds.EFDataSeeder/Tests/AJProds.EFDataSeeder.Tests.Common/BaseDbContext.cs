using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AJProds.EFDataSeeder.Tests.Common;

public interface ITestContext : IDisposable,
                                IAsyncDisposable
{
    public const string SCHEMA = "tst";

    public DbSet<Testee> Testees { get; }

    public DatabaseFacade Database { get; }

    public int SaveChanges();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}