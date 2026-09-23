using yildiz.entities.Concrete;

namespace yildiz.DataAccess.Abstract;

public interface IUserRepository : IGenericRepository<AppUser>
{
    Task<AppUser?> GetByUsernameAsync(string username);

    Task<AppUser?> GetByUsernameAndEmailAsync(
        string username,
        string email);

    Task<AppUser?> GetByEmailAsync(string email);
}