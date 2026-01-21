using GranjaSystemProject.Models.Farm;
using GranjaSystemProject.Services;
using GranjaSystemProject.Tests.Tests.TestInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace GranjaSystemProject.Tests.Tests.Services;

public class FarmServiceTests
{
    [Fact]
    public async Task CreateFarmAsync_ShouldCreateFarm_WhenUserHasNoFarm()
    {
        var context = DbContextFactory.Create();
        var service = new FarmService(context);

        var farm = new Farm
        {
            Name = "Granja",
            OwnerId = 1
        };

        var (success, message) = await service.CreateFarmAsync(farm);

        Assert.True(success);
        Assert.Equal("Granja cadastrada com sucesso", message);
        Assert.Equal(1, await context.Farms.CountAsync());
    }

    [Fact]
    public async Task CreateFarmAsync_ShouldFail_WhenUserAlreadyHasFarm()
    {
        var context = DbContextFactory.Create();
        var service = new FarmService(context);

        var existingFarm = new Farm
        {
            Name = "Granja",
            OwnerId = 1
        };

        await FarmServiceFactory.SeedFarmAsync(context, existingFarm); // Adiciona diretamente no contexto

        var newFarm = new Farm
        {
            Name = "Nova Granja",
            OwnerId = 1
        };

        var (success, message) = await service.CreateFarmAsync(newFarm);

        Assert.False(success);
        Assert.Equal("Você já possui uma Granja cadastrada.", message);
        Assert.Equal(1, await context.Farms.CountAsync());
    }


}
