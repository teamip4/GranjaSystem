using GrajaSystemProject.Data;
using GranjaSystemProject.Models.Farm;
using Microsoft.EntityFrameworkCore;

namespace GranjaSystemProject.Services;
public class FarmService
{
    private readonly AppDbContext _context;

    public FarmService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<(bool Sucess, string Message)> CreateFarmAsync(Farm farm)
    {
        try
        {
            var existsFarm = await _context.Farms.AnyAsync(f => f.OwnerId == farm.OwnerId);

            if (existsFarm)
            {
                return (false, "Você já possui uma Granja cadastrada.");
            }

            _context.Farms.Add(farm);
            await _context.SaveChangesAsync();
            return (true, "Granja cadastrada com sucesso");
        } 
        catch (Exception ex)
        {
            return (false, $"Erro ao salvar: {ex.Message}");
        }
    }
}
