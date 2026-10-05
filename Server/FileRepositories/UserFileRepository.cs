using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<User>> LoadAsync()
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
    }

    private async Task SaveAsync(List<User> users)
    {
        string usersAsJson = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, usersAsJson);
    }

    public async Task<User> AddAsync(User user)
    {
        List<User> users = await LoadAsync();
        user.UserId = users.Count > 0 ? users.Max(u => u.UserId) + 1 : 1;
        users.Add(user);
        await SaveAsync(users);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        List<User> users = await LoadAsync();
        User? existing = users.SingleOrDefault(u => u.UserId == user.UserId);
        if (existing is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{user.UserId}' not found");
        }

        users.Remove(existing);
        users.Add(user);
        await SaveAsync(users);
    }

    public async Task DeleteAsync(int id)
    {
        List<User> users = await LoadAsync();
        User? userToRemove = users.SingleOrDefault(u => u.UserId == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        users.Remove(userToRemove);
        await SaveAsync(users);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        List<User> users = await LoadAsync();
        User? user = users.SingleOrDefault(u => u.UserId == id);
        if (user is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        return user;
    }

    public IQueryable<User> GetMany()
    {
        string usersAsJson = File.ReadAllTextAsync(filePath).Result;
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson)!;
        return users.AsQueryable();
    }
}