using Blazored.LocalStorage;
using Microsoft.AspNetCore.Identity;
using Squill.Data.Auth;
using System.Text.Json;

namespace Squill.Services;

public class UserService
{
    public User CurrentUser { get; private set; }

    private static IEnumerable<User> AllUsers => Directory.GetFiles(Path.Combine(ProjectService.DataDirectory, "usr"), "*.usr")
        .Select(s => JsonSerializer.Deserialize<User>(File.ReadAllText(s)));

    public UserService()
    {
        var usrPath = Path.Combine(ProjectService.DataDirectory, "usr");
        if (!Directory.Exists(usrPath))
        {
            Directory.CreateDirectory(usrPath);
        }
    }

    public bool UserExists(string username) => AllUsers.Any(u => u.Name == username);

    public async Task Register(ILocalStorageService storage, string username, string email, string password)
    {
        if (UserExists(username))
        {
            throw new ArgumentException($"Username {username} already exists");
        }
        var user = new User
        {
            Name = username,
            Email = email,
        };
        user.SetNewPassword(null, password);
        await SetToken(storage, user);
        CurrentUser = user;
    }

    public async Task<bool> Login(ILocalStorageService storage, string username, string password)
    {
        var user = AllUsers.SingleOrDefault(u => u.Name == username);
        if (user == null || !user.TryChallenge(password))
        {
            return false;
        }
        await SetToken(storage, user);
        CurrentUser = user;
        return true;
    }

    private async Task SetToken(ILocalStorageService storage, User user)
    {
        var token = Guid.NewGuid().ToString();
        user.Tokens.Add(token);
        await storage.SetItemAsStringAsync("session", token);
        await SaveUser(user);
    }

    private async Task SaveUser(User user)
    {
        await File.WriteAllTextAsync(
            Path.Combine(ProjectService.DataDirectory, "usr", $"{user.Name}.usr"),
            JsonSerializer.Serialize(user));
    }

    public async Task<bool> TryAutoLogin(ILocalStorageService storage)
    {
        var token = await storage.GetItemAsStringAsync("session");
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        var user = AllUsers.SingleOrDefault(u => u.Tokens.Contains(token));
        if(user  == null)
        {
            return false;
        }
        CurrentUser = user;
        return true;
    }
}
