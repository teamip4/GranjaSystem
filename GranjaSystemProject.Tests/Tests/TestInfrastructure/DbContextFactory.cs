using GranjaSystemProject.Data;
using Microsoft.EntityFrameworkCore;

namespace GranjaSystemProject.Tests.Tests.TestInfrastructure;

public static class DbContextFactory
{
    public static AppDbContext Create() => Create(Guid.NewGuid().ToString());

    public static AppDbContext Create(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new AppDbContext(options);
    }
}
