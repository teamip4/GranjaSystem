using GrajaSistemProject.Data;
using GrajaSistemProject.Models.User;
using Microsoft.EntityFrameworkCore;

namespace GranjaSistemProject.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public async Task<User?> FindUserByEmailAndPassword(String email, String password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }
    }
}
