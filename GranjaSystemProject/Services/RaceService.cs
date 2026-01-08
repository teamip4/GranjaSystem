using GranjaSystemProject.Data;
using GranjaSystemProject.Models.Farm;
using Microsoft.EntityFrameworkCore;

namespace GranjaSystemProject.Services;
public class RaceService
{
    private readonly AppDbContext _context;

    public RaceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Sucess, string Message)> RegisterRaceAsync(Race race)
    {
        try
        {
            var existsRace = await _context.Races.AnyAsync(r => r.Name.ToLower() == race.Name.ToLower());

            if (existsRace)
            {
                return (false, "Essa raça já foi cadastrada.");
            }

            _context.Races.Add(race);
            await _context.SaveChangesAsync();
            return (true, "Raça cadastrada com sucesso!");
        }
        catch (Exception ex)
        {
            return (false, $"Erro ao salvar: {ex.Message}");
        }
    }
}
