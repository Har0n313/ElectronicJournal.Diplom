using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;

namespace ElectronicJournal.Application.Interfaces;

public interface IUserService
{
    Task<User> RegisterUser(User user);

    Task<User> Authenticate(string email, string password);

    Task ChangePassword(int userId, string newPassword);
    
    Task<User> UpdateUser(User user);
    Task<bool> DeletUser(int userId);

    Task<User> GetUserInfo(int id);

    Task<ICollection<User>> GetUsersByRole(UserRole role);
}