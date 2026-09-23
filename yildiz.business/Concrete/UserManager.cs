using yildiz.business.Abstract;
using yildiz.DataAccess.Abstract;
using yildiz.DataAccess.Concrete;
using yildiz.entities.Concrete;

namespace yildiz.business.Concrete;

public class UserManager : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AppUser?> GetByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<AppUser?> GetByUsernameAndEmailAsync(
        string username,
        string email)
    {
        return await _userRepository.GetByUsernameAndEmailAsync(
            username,
            email);
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task AddAsync(AppUser user)
    {
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(AppUser user)
    {
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
    }
}