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
        {
            throw new ArgumentNullException(nameof(user));
        }

        if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            throw new ArgumentException("Обязательные поля (Username, PasswordHash) не могут быть пустыми.");
        }

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Пользователь с именем {user.Username} уже существует.");
        }

        if (!Enum.IsDefined(typeof(UserRole), user.Role))
        {
            throw new ArgumentException(
                $"Недопустимое значение роли: {user.Role}. Допустимые значения: Admin (1), Teacher (2), Student (3).");
        }

        if (user.StudentId.HasValue)
        {
            var student = await _context.Students.FindAsync(user.StudentId);
            if (student == null)
            {
                throw new KeyNotFoundException($"Студент с ID {user.StudentId} не найден.");
            }

            if (user.Role != UserRole.Student)
            {
                throw new ArgumentException("Если указан StudentId, роль должна быть Student (3).");
            }
        }

        if (user.TeacherId.HasValue)
        {
            var teacher = await _context.Teachers.FindAsync(user.TeacherId);
            if (teacher == null)
            {
                throw new KeyNotFoundException($"Преподаватель с ID {user.TeacherId} не найден.");
            }

            if (user.Role != UserRole.Teacher)
            {
                throw new ArgumentException("Если указан TeacherId, роль должна быть Teacher (2).");
            }
        }

        if (!user.StudentId.HasValue && !user.TeacherId.HasValue && user.Role != UserRole.Admin)
        {
            throw new ArgumentException(
                "Для роли Admin (1) не требуется StudentId или TeacherId, но для других ролей одно из них должно быть указано.");
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> Authenticate(string email, string password)
    {
        if (email == null || password == null || string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Обязательные поля (Username, Password) не могут быть пустыми.");
        }

        var user = await _context.Users
            .Include(u => u.Student)
            .Include(u => u.Teacher)
            .FirstOrDefaultAsync(u => u.Username == email && u.PasswordHash == password);

        if (user == null)
        {
            throw new InvalidOperationException("Неверное имя пользователя или пароль.");
        }

        return user;
    }

    public async Task ChangePassword(int userId, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            throw new ArgumentException("Новый пароль не может быть пустым.");
        }

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"Пользователь с ID {userId} не найден.");
        }

        user.PasswordHash = newPassword;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetUserInfo(int id)
    {
        var query = _context.Users
            .Include(u => u.Student)
            .Include(u => u.Teacher);

        var user = await query.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new KeyNotFoundException($"Пользователь с ID {id} не найден.");
        }

        return user;
    }

    public async Task<ICollection<User>> GetUsersByRole(UserRole role)
    {
        if (role == 0)
        {
            throw new ArgumentException("Роль должна быть указана.");
        }

        if (!Enum.IsDefined(typeof(UserRole), role))
        {
            throw new ArgumentException(
                $"Недопустимое значение роли: {role}. Допустимые значения: Admin (1), Teacher (2), Student (3).");
        }

        var query = _context.Users
            .Where(u => u.Role == role)
            .Include(u => u.Student)
            .Include(u => u.Teacher);

        return await query.ToListAsync();
    }
}