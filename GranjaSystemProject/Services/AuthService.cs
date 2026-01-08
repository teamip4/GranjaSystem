using GranjaSystemProject.Data;
using GranjaSystemProject.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GranjaSystemProject.Services;

public class AuthService
{
    private readonly AppDbContext _context;
    private const int MaxFailedAttempts = 3;
    private const int LockoutMinutes = 15;
    private const int BCryptCostFactor = 10;
    public User CurrentUser { get; private set; }

    public AuthService(AppDbContext context)
    {
        _context = context;
    }
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: BCryptCostFactor);
    }
    public async Task<(bool Success, string Message)> RegisterUserAsync(User newUser)
    {
        try
        {
            var userExists = await _context.Users.AnyAsync(u => u.Email == newUser.Email);

            if (userExists)
            {
                return (false, "Este e-mail já está cadastrado.");
            }

            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return (true, "Usuário cadastrado com sucesso!");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao salvar: {ex.Message}");
        }
    }
    public async Task<(bool Success, string Message)> UpdateUserAsync(User updateUser)
    {
        try
        {
            _context.Users.Update(updateUser);
            await _context.SaveChangesAsync();
            this.CurrentUser = updateUser;
            return (true, "Usuário cadastrado com sucesso!");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao salvar: {ex.Message}");
        }
    }
    public async Task<User?> AuthenticateUser(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return null;

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.Now)
        {
            return null;
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        if (isPasswordValid)
        {
            this.CurrentUser = user;
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();
            return user;
        }
        else
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockoutEnd = DateTime.Now.AddMinutes(LockoutMinutes);
                user.FailedLoginAttempts = 0;
            }
            await _context.SaveChangesAsync();
            return null;
        }
    }
    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("CHAVE_SECRETA_TEMP_PARA_TESTE_32X");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Type.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
