namespace VStore.Application.Abstractions.BCrypt;

public interface IPasswordHasher
{
    string HashPassword(string password);

    /// <summary>
    /// Verify password
    /// </summary>
    /// <param name="password">password</param>
    /// <param name="passwordHash">hashed password</param>
    /// <returns></returns>
    bool VerifyPassword(string password, string passwordHash);
}