using Microsoft.AspNetCore.Identity;

namespace Squill.Data.Auth;

public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Challenge { get; set; }

    public bool TryChallenge(string password)
    {
        if (string.IsNullOrWhiteSpace(Challenge))
        {
            return true;
        }
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(this, Challenge, password);
        switch(result)
        {
            case PasswordVerificationResult.Success:
                return true;
            case PasswordVerificationResult.SuccessRehashNeeded:
                SetNewPassword(password, password);
                return true;
            default:
                return false;
        }
    }

    public bool SetNewPassword(string? oldPassword, string newPassword)
    {
        if (!TryChallenge(oldPassword))
        {
            return false;
        }
        var hasher = new PasswordHasher<User>();
        Challenge = hasher.HashPassword(this, newPassword);
        return true;
    }
}
