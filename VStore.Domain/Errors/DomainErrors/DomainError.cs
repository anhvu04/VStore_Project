using VStore.Domain.Shared;

namespace VStore.Domain.Errors.DomainErrors;

public static class DomainError
{
    public static class Authentication
    {
        public static readonly Error IncorrectUsernameOrPassword =
            new("Authentication.IncorrectUsernameOrPassword", "Incorrect username or password.");

        public static readonly Error UsernameAlreadyExists =
            new("Authentication.UsernameAlreadyExists", "Username already exists.");

        public static readonly Error EmailAlreadyExists =
            new("Authentication.EmailAlreadyExists", "Email already exists.");

        public static readonly Error PhoneNumberAlreadyExists =
            new("Authentication.PhoneNumberAlreadyExists", "Phone number already exists.");
    }
}