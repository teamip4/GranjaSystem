using GranjaSystemProject.Models.Users;
using GranjaSystemProject.Services;
using GranjaSystemProject.Tests.Tests.TestInfrastructure;
using GranjaSystemProject.Tests.Tests.TestInfrastructure.Builders;
using GranjaSystemProject.Tests.Tests.TestInfrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        var (success, _) = await authService.RegisterUserAsync(user);

        Assert.False(success);
        Assert.Equal(0, await context.Users.CountAsync());
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

    [Fact]
    public async Task AuthenticateUser_ShouldReturnUser_WhenCredentialsAreValid()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var existingUser = new UserBuilder()
            .WithEmail(TestConstants.ValidEmail)
            .WithHashedPassword(TestConstants.ValidPassword) // Garante que a senha esteja "hasheada"
            .Build();

        await AuthServiceFactory.SeedUserAsync(context, existingUser);

        var result = await authService.AuthenticateUser(TestConstants.ValidEmail, TestConstants.ValidPassword);

        Assert.NotNull(result);
        Assert.Equal(result, authService.CurrentUser);
    }

    [Fact]
    public async Task AuthenticateUser_ShouldReturnNull_WhenEmailIsInvalid()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var existingUser = new UserBuilder()
            .WithEmail(TestConstants.ValidEmail)
            .WithHashedPassword(TestConstants.ValidPassword)
            .Build();

        await AuthServiceFactory.SeedUserAsync(context, existingUser);

        var result = await authService.AuthenticateUser(
            TestConstants.InvalidEmail, // Email errado
            TestConstants.ValidPassword 
        );

        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateUser_ShouldReturnNull_WhenPasswordIsInvalid()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var existingUser = new UserBuilder()
            .WithEmail(TestConstants.ValidEmail)
            .WithHashedPassword(TestConstants.ValidPassword)
            .Build();

        await AuthServiceFactory.SeedUserAsync(context, existingUser);

        var result = await authService.AuthenticateUser(
            TestConstants.ValidEmail, 
            TestConstants.InvalidPassword // Senha errada
        );

        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateUser_ShouldLockUser_AfterThreeFailedAttempts()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var existingUser = new UserBuilder()
            .WithEmail(TestConstants.ValidEmail)
            .WithHashedPassword(TestConstants.ValidPassword)
            .Build();

        await AuthServiceFactory.SeedUserAsync(context, existingUser);

        // 3 tentativas de login com senha errada
        await authService.AuthenticateUser(existingUser.Email, TestConstants.InvalidPassword);
        await authService.AuthenticateUser(existingUser.Email, TestConstants.InvalidPassword);
        await authService.AuthenticateUser(existingUser.Email, TestConstants.InvalidPassword);

        var lockedUser = await context.Users.FirstAsync();

        Assert.NotNull(lockedUser.LockoutEnd);
        Assert.True(lockedUser.LockoutEnd > DateTime.Now);
        Assert.Equal(0, lockedUser.FailedLoginAttempts);

        // Tentativa com credenciais corretas
        var result = await authService.AuthenticateUser(existingUser.Email, TestConstants.ValidPassword);
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthenticateUser_ShouldResetFailedAttempts_WhenLoginSucceeds()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var existingUser = new UserBuilder()
           .WithEmail(TestConstants.ValidEmail)
           .WithHashedPassword(TestConstants.ValidPassword)
           .Build();

        await AuthServiceFactory.SeedUserAsync(context, existingUser);

        // 2 tentativas de login com senha errada
        await authService.AuthenticateUser(existingUser.Email, TestConstants.InvalidPassword);
        await authService.AuthenticateUser(existingUser.Email, TestConstants.InvalidPassword);

        // Resgata usuário (sem login) para verificar seus atributos
        var user = await context.Users.FirstAsync();
        Assert.Equal(2, user.FailedLoginAttempts);
        Assert.Null(user.LockoutEnd);

        // Login correto
        var result = await authService.AuthenticateUser(existingUser.Email, TestConstants.ValidPassword);

        Assert.NotNull(result);
        Assert.Equal(0, result.FailedLoginAttempts);
        Assert.Null(result.LockoutEnd);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUser_WhenUserExists()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var existingUser = new UserBuilder()
            .WithEmail(TestConstants.ValidEmail)
            .WithPassword(TestConstants.ValidPassword)
            .Build();

        await AuthServiceFactory.SeedUserAsync(context, existingUser);

        existingUser.Name = "Nome Atualizado";

        var (success, _) = await authService.UpdateUserAsync(existingUser);

        var updatedUser = await context.Users.FirstAsync();

        Assert.True(success);
        Assert.Equal("Nome Atualizado", updatedUser.Name);
        Assert.Equal(updatedUser, authService.CurrentUser);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldFail_WhenUserDoesNotExist()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var nonExistentUser = new UserBuilder()
            .WithId(999) // ID inexistente
            .Build();

        var (success, _) = await authService.UpdateUserAsync(nonExistentUser);

        Assert.False(success);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldFail_WhenEmailAlreadyExists()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var user1 = new UserBuilder()
            .WithEmail("user1@email.com")
            .Build();

        var user2 = new UserBuilder()
            .WithEmail("user2@email.com")
            .Build();

        await AuthServiceFactory.SeedUserAsync(context, user1);
        await AuthServiceFactory.SeedUserAsync(context, user2);

        user2.Email = user1.Email; // email duplicado

        var (success, _) = await authService.UpdateUserAsync(user2);

        Assert.False(success);
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnValidToken()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var user = new UserBuilder()
            .WithId(1)
            .WithType(UserType.Administrador)
            .Build();

        var token = authService.GenerateJwtToken(user);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GenerateJwtToken_ShouldContainCorrectClaims()
    {
        var context = DbContextFactory.Create();
        var authService = new AuthService(context);

        var user = new UserBuilder()
            .WithId(42)
            .WithType(UserType.Farmer)
            .Build();

        var token = authService.GenerateJwtToken(user);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var userIdClaim = jwt.Claims.First(c => c.Type == "nameid");
        var roleClaim = jwt.Claims.First(c => c.Type == "role");

        Assert.Equal("42", userIdClaim.Value);
        Assert.Equal(UserType.Farmer.ToString(), roleClaim.Value);
    }
}