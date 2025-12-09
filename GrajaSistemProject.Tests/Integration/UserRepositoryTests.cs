using GrajaSistemProject.Data;
using GrajaSistemProject.Models.User;
using GranjaSistemProject.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace GranjaSystemProject.Tests.Integration
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _options;

        public UserRepositoryTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

            using var context = new AppDbContext(_options);
            context.Database.EnsureCreated();
        }

        [Fact(DisplayName = "Deve retornar usuário quando email e senha estão corretos")]
        public async Task FindUser_ShouldReturnUser_WhenCredentialsAreCorrect()
        {
            var user = new User
            {
                Name = "João",
                BirthDate = new DateTime(1990, 1, 1),
                Email = "joao@teste.com",
                Password = "1234",
                Type = User.UserType.Administrador,
                Cpf = "000.000.000-00",
                State = "CE",
                City = "Crateús",
                Address = "Rua Dom Pedro II",
                Phone = "(11) 99999-9999"
            };

            using (var context = new AppDbContext(_options))
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Cria novo contexto, para evitar busca em cache
            using var actContext = new AppDbContext(_options);

            var repository = new UserRepository(actContext);
            var result = await repository.FindUserByEmailAndPassword("joao@teste.com", "1234");

            result.Should().NotBeNull();
            result!.Name.Should().Be("João");
            result.Email.Should().Be("joao@teste.com");
            result.Type.Should().Be(User.UserType.Administrador);
        }

        [Fact(DisplayName = "Deve retornar NULL quando a senha estiver incorreta")]
        public async Task GetUser_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            var user = new User
            {
                Name = "João",
                BirthDate = new DateTime(1990, 1, 1),
                Email = "joao@teste.com",
                Password = "1234",
                Type = User.UserType.Administrador,
                Cpf = "000.000.000-00",
                State = "CE",
                City = "Crateús",
                Address = "Rua Dom Pedro II",
                Phone = "(11) 99999-9999"
            };

            using (var context = new AppDbContext(_options))
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using var actContext = new AppDbContext(_options);

            var repository = new UserRepository(actContext);
            var result = await repository.FindUserByEmailAndPassword("joao@teste.com", "1233");

            result.Should().BeNull();
        }
        public void Dispose()
        {
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
