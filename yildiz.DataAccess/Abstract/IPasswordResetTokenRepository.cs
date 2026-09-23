using yildiz.entities.Concrete;

namespace yildiz.DataAccess.Abstract;

public interface IPasswordResetTokenRepository : IGenericRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash);
}