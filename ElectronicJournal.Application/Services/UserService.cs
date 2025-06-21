using ElectronicJournal.Application.Interfaces;
using ElectronicJournal.Domain;
using ElectronicJournal.Domain.Entites;
using ElectronicJournal.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ElectronicJournal.Application.Services;

public class UserService : IUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> _context;

    public UserService(IDbContextFactory<ApplicationDbContext> context)
    {
        _context = context;
    }

    public async Task<User> RegisterUser(User user)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.PasswordHash))
            throw new ArgumentException("Username and PasswordHash are required.");

        if (await context.Users.AnyAsync(u => u.UserName == user.UserName))
            throw new InvalidOperationException($"Пользователь с логином {user.UserName} существует.");

        if (!Enum.IsDefined(typeof(UserRole), user.Role))
            throw new ArgumentException("Invalid user role specified.");

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User> Authenticate(string username, string password)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Username and password are required.");

        var user = await context.Users
            .Include(u => u.Student)
            .Include(u => u.Teacher)
            .FirstOrDefaultAsync(u => u.UserName == username && u.PasswordHash == password);

        if (user == null)
            throw new InvalidOperationException("Invalid credentials.");

        return user;
    }

    public async Task<bool> DeletUser(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        var user = await context.Users.FindAsync(id);
        if (user == null) return false;

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<User> GetUserInfo(int id)
    {
        await using var context = await _context.CreateDbContextAsync();

        return await context.Users
                   .Include(u => u.Student)
                   .Include(u => u.Teacher)
                   .FirstOrDefaultAsync(u => u.Id == id)
               ?? throw new KeyNotFoundException($"User with ID {id} not found.");
    }

    public async Task ChangePassword(int userId, string newPassword)
    {
        await using var context = await _context.CreateDbContextAsync();

        var user = await context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found.");

        user.PasswordHash = newPassword;
        await context.SaveChangesAsync();
    }

    public async Task<User> UpdateUser(User updated)
    {
        await using var context = await _context.CreateDbContextAsync();

        var user = await context.Users.FindAsync(updated.Id);
        if (user == null)
            throw new KeyNotFoundException();

        user.UserName = updated.UserName;
        user.PasswordHash = updated.PasswordHash;

        await context.SaveChangesAsync();
        return user;
    }

    public async Task<ICollection<User>> GetUsersByRole(UserRole role)
    {
        await using var context = await _context.CreateDbContextAsync();

        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new ArgumentException("Invalid user role.");

        return await context.Users
            .Include(u => u.Student)
            .Include(u => u.Teacher)
            .Where(u => u.Role == role)
            .ToListAsync();
    }
}