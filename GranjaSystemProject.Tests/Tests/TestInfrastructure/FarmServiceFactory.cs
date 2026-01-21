using GranjaSystemProject.Data;
using GranjaSystemProject.Models.Farm;
namespace GranjaSystemProject.Tests.Tests.TestInfrastructure;
public static class FarmServiceFactory
{
    public static async Task SeedFarmAsync(AppDbContext context, Farm farm)
    {
        context.Farms.Add(farm);
        await context.SaveChangesAsync();
    }
}