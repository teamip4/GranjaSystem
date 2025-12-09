using GrajaSistemProject.Data;
using GranjaSistemProject.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GranjaSistemProject
{
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

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "grajasystem.db3");
            Console.WriteLine($"Banco de dados em: {databasePath}");

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={databasePath}"));

            builder.Services.AddScoped<UserRepository>();

            return builder.Build();
        }
    }
}
