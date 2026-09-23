using yildiz.entities.Concrete;

namespace yildiz.business.Abstract;

public interface IPasswordResetTokenService
{
    Task CreateAsync(PasswordResetToken token);

    Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash);

    Task UpdateAsync(PasswordResetToken token);
}