using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.PasswordHash))
            throw new ArgumentException("Username and PasswordHash are required.");

        if (await _context.Users.AnyAsync(u => u.UserName == user.UserName))
            throw new InvalidOperationException($"User with name {user.UserName} already exists.");

        if (!Enum.IsDefined(typeof(UserRole), user.Role))
            throw new ArgumentException("Invalid user role specified.");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Username and password are required.");

        var user = await _context.Users
            .Include(u => u.Student)
            .Include(u => u.Teacher)
            .FirstOrDefaultAsync(u => u.UserName == username && u.PasswordHash == password);

        if (user == null)
            throw new InvalidOperationException("Invalid credentials.");

        return user;
    }

    public Task<bool> DeletUser(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserInfo(int id)
    {
        return await _context.Users
            .Include(u => u.Student)
            .Include(u => u.Teacher)
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");
    }

    public async Task ChangePassword(int userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        user.PasswordHash = newPassword;
        await _context.SaveChangesAsync();
    }

    public Task<User> UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<User>> GetUsersByRole(UserRole role)
    {
        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new ArgumentException("Invalid user role.");

        return await _context.Users
            .Include(u => u.Student)
            .Include(u => u.Teacher)
            .Where(u => u.Role == role)
            .ToListAsync();
    }
}