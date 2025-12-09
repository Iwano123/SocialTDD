using SocialTDD.Domain.Models;

namespace SocialTDD.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
}

