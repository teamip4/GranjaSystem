using GranjaSystemProject.Models.Users;
using GranjaSystemProject.Services;
using GranjaSystemProject.Tests.Tests.TestInfrastructure;
using GranjaSystemProject.Tests.Tests.TestInfrastructure.Builders;
using GranjaSystemProject.Tests.Tests.TestInfrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GranjaSystemProject.Tests.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterUserAsync_ShouldFail_WhenUserIsInvalid()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var user = new UserBuilder()
            .WithName("")
            .WithBirthDate(DateTime.Today.AddYears(1))
            .WithEmail(TestConstants.InvalidEmail)
            .WithPassword("")
            .WithType((UserType)999)
            .Build();

        var (success, message) = await authService.RegisterUserAsync(user);

        Assert.Equal(0, await context.Users.CountAsync());
        Assert.False(success);
        
    }


    [Fact]
    public async Task RegisterUserAsync_ShouldRegisterUser_WhenEmailIsUnique()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var user = new UserBuilder()
            .WithEmail(TestConstants.ValidEmail)
            .WithPassword(TestConstants.ValidPassword)
            .Build();

        var (Success, Message) = await authService.RegisterUserAsync(user);

        Assert.True(Success);
        Assert.Equal("Usuário cadastrado com sucesso!", Message);

        var savedUser = await context.Users.FirstOrDefaultAsync();
        Assert.NotNull(savedUser);
        Assert.NotEqual(TestConstants.ValidPassword, savedUser.PasswordHash); // Verifica se a senha não foi salva em texto claro
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldFail_WhenEmailAlreadyExists()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var existingUser = new UserBuilder()
            .WithEmail(TestConstants.ValidEmail)
            .WithPassword(TestConstants.ValidPassword)
            .Build();

        await AuthServiceFactory.SeedUserAsync(context, existingUser); // Adiciona diretamente no contexto.

        var user = new UserBuilder()
            .WithEmail(TestConstants.ValidEmail) // Mesmo email
            .WithPassword(TestConstants.ValidPassword)
            .Build();

        var (Success, Message) = await authService.RegisterUserAsync(user);

        Assert.False(Success);
        Assert.Equal("Este e-mail já está cadastrado.", Message);
    }
}
