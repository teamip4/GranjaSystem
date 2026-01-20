using GranjaSystemProject.Models.Users;
using GranjaSystemProject.Tests.Tests.TestInfrastructure.Constants;

namespace GranjaSystemProject.Tests.Tests.TestInfrastructure.Builders;

public class UserBuilder
{
    private readonly User _user = new();

    public UserBuilder()
    {
        _user.Name = "Usuário Teste";
        _user.Email = $"user_{Guid.NewGuid()}@hotmail.com";
        _user.PasswordHash = TestConstants.ValidPassword;
        _user.BirthDate = new DateTime(1990, 1, 1);
        _user.Type = UserType.Administrador;
    }

    public UserBuilder WithId(int id)
    {
        _user.Id = id;
        return this;
    }

    public UserBuilder WithName(string name)
    {
        _user.Name = name;
        return this;
    }

    public UserBuilder WithBirthDate(DateTime birthDate)
    {
        _user.BirthDate = birthDate;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _user.Email = email;
        return this;
    }

    public UserBuilder WithPassword(string password)
    {
        _user.PasswordHash = password;
        return this;
    }

    public UserBuilder WithHashedPassword(string password)
    {
        _user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        return this;
    }

    public UserBuilder WithType(UserType type)
    {
        _user.Type = type;
        return this;
    }

    public User Build() => _user;
}
