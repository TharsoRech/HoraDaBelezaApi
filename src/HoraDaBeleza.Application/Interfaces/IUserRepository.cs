using HoraDaBeleza.Domain.Entities;

namespace HoraDaBeleza.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByDocAsync(string doc);
    Task<int> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task<bool> EmailExistsAsync(string email, int? ignoreId = null);
    Task<bool> DocExistsAsync(string doc, int? ignoreId = null);
}
