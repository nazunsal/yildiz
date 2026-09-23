using yildiz.business.Abstract;
using yildiz.DataAccess.Abstract;
using yildiz.entities.Concrete;

namespace yildiz.business.Concrete;

public class PasswordResetTokenManager : IPasswordResetTokenService
{
    private readonly IPasswordResetTokenRepository _tokenRepository;

    public PasswordResetTokenManager(
        IPasswordResetTokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public async Task CreateAsync(PasswordResetToken token)
    {
        await _tokenRepository.AddAsync(token);
        await _tokenRepository.SaveChangesAsync();
    }

    public async Task<PasswordResetToken?> GetByTokenHashAsync(
        string tokenHash)
    {
        return await _tokenRepository.GetByTokenHashAsync(tokenHash);
    }

    public async Task UpdateAsync(PasswordResetToken token)
    {
        await _tokenRepository.UpdateAsync(token);
        await _tokenRepository.SaveChangesAsync();
    }
}