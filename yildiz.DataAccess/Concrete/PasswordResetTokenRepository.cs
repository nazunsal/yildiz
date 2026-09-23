using Microsoft.EntityFrameworkCore;
using yildiz.DataAccess.Abstract;
using yildiz.DataAccess.Context;
using yildiz.DataAccess.Repositories;
using yildiz.entities.Concrete;

namespace yildiz.DataAccess.Concrete;

public class PasswordResetTokenRepository
    : GenericRepository<PasswordResetToken>,
      IPasswordResetTokenRepository
{
    private readonly AppDbContext _context;

    public PasswordResetTokenRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken?> GetByTokenHashAsync(
        string tokenHash)
    {
        return await _context.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
    }
}