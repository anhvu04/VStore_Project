using VStore.Domain.Shared;

namespace VStore.Domain.Errors.DomainErrors;

public static class DomainError
{
    public static class Authentication
    {
        public static readonly Error IncorrectUsernameOrPassword =
            new("Authentication.IncorrectUsernameOrPassword", "Incorrect username or password.");
    }
}