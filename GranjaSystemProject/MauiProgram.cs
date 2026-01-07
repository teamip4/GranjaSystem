using GrajaSystemProject.Data;
using GranjaSystemProject.Helpers;
using GranjaSystemProject.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GranjaSystemProject;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "granja.db");

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });

        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<FarmService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var db = serviceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();

            var authService = serviceProvider.GetRequiredService<AuthService>();

            if (!db.Users.Any())
            {
                const string defaultPassword = "password123";
                string passwordHash = authService.HashPassword(defaultPassword);

                var defaultUser = new GrajaSystemProject.Models.User.User
                {
                    Name = "Admin Teste",
                    Email = "admin@teste.com",
                    PasswordHash = passwordHash,
                    BirthDate = new DateTime(1990, 1, 1),
                    Cpf = "111.222.333-45",
                    State = "CE",
                    City = "Crateús",
                    Address = "Rua A",
                    Phone = "(88)9.9999-9999",
                    Type = GrajaSystemProject.Models.User.UserType.Administrador,
                    FailedLoginAttempts = 0
                };

                db.Users.Add(defaultUser);
                db.SaveChanges();
            }
        }

        ServiceProviderHelper.Configure(app.Services);

        return app;
    }
}
