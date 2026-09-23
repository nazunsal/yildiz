using Microsoft.EntityFrameworkCore;
using yildiz.DataAccess.Abstract;
using yildiz.DataAccess.Context;
using yildiz.DataAccess.Repositories;
using yildiz.entities.Concrete;

namespace yildiz.DataAccess.Concrete;

public class UserRepository : GenericRepository<AppUser>, IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<AppUser?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<AppUser?> GetByUsernameAndEmailAsync(
        string username,
        string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Username == username &&
                x.Email == email);
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }
}