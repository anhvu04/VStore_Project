using VStore.Domain.Shared;

namespace VStore.Domain.Errors.DomainErrors;

public static class DomainError
{
    public static class CommonError
    {
        public static Error NotFound(string value) =>
            new($"Error.NotFound.{value}", $"{value} not found.");

        public static Error AlreadyExists(string value) =>
            new($"Error.AlreadyExists.{value}", $"{value} already exists.");
    }

    public static class Authentication
    {
        public static readonly Error IncorrectUsernameOrPassword =
            new("Error.Authentication.IncorrectUsernameOrPassword", "Incorrect username or password.");

        public static readonly Error InvalidToken =
            new("Error.Authentication.InvalidToken", "Invalid token.");

        public static readonly Error TokenExpired =
            new("Error.Authentication.TokenExpired", "Token expired.");

        public static readonly Error InvalidCode =
            new("Error.Authentication.InvalidCode", "Invalid code.");
    }

    public static class User
    {
        public static readonly Error Banned =
            new("Error.User.Banned", "User is banned.");

        public static readonly Error NotActive =
            new("Error.User.NotActive", "User is not active.");

        public static readonly Error WrongPassword =
            new("Error.User.WrongPassword", "Wrong password.");
    }

    public static class Brand
    {
        public static readonly Error HasProduct =
            new("Error.Brand.HasProduct", "Brand has product.");
    }

    public static class Category
    {
        public static readonly Error HasSubCategory =
            new("Error.Category.HasSubCategory", "Category has sub category.");

        public static readonly Error InvalidParentCategory =
            new("Error.Category.InvalidParentCategory",
                "This parent category is a child category of the current category.");

        public static readonly Error HasProduct =
            new("Error.Category.HasProduct", "Category has product.");
    }

    public static class Product
    {
        public static readonly Error InvalidPrice =
            new("Error.Product.InvalidPrice", "Sale price must be less than the original price.");
    }
}