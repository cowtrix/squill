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

    public async Task Register(string username, string email, string password)
    {
        if(UserExists(username))
        {
            throw new ArgumentException($"Username {username} already exists");
        }
        var user = new User
        {
            Name = username,
            Email = email,
        };
        user.SetNewPassword(null, password);
        File.WriteAllText(Path.Combine(ProjectService.DataDirectory, "usr", $"{username}.usr"), JsonSerializer.Serialize(user));
        CurrentUser = user;
    }

    public async Task<bool> Login(string username, string password)
    {
        var user = AllUsers.SingleOrDefault(u => u.Name == username);
        if(user == null || !user.TryChallenge(password))
        {
            return false;
        }
        CurrentUser = user;
        return true;
    }
}
