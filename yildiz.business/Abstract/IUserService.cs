using yildiz.entities.Concrete;

namespace yildiz.business.Abstract;

public interface IUserService
{
    Task<AppUser?> GetByUsernameAsync(string username);

    Task<AppUser?> GetByUsernameAndEmailAsync(
        string username,
        string email);

    Task<AppUser?> GetByEmailAsync(string email);

    Task AddAsync(AppUser user);

    Task UpdateAsync(AppUser user);
}