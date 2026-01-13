using GranjaSystemProject.Models.Users;
using GranjaSystemProject.Data;
namespace GranjaSystemProject.Tests.Tests.TestInfrastructure; 
public static class AuthServiceFactory 
{ 
    public static async Task SeedUserAsync(AppDbContext context, User user) 
    { 
        context.Users.Add(user); 
        await context.SaveChangesAsync(); 
    } 
}