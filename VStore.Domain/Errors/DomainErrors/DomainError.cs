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

        public static Error ExceptionHandled(string value) =>
            new($"Error.ExceptionHandled.{value}", $"Exception handled: {value}");
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

        public static readonly Error InvalidFileExtension =
            new("Error.Brand.InvalidFileExtension", "Invalid file extension.");
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

        public static readonly Error NotEnoughQuantity =
            new("Error.Product.NotEnoughQuantity", "Not enough quantity.");

        public static Error ExceedQuantity(int quantity, int productQuantity) =>
            new("Error.Product.NotEnoughQuantity", $"You have already had {quantity} products in your cart. " +
                                                   $"The remaining quantity is {productQuantity - quantity}.");

        public static readonly Error InvalidFileExtension =
            new("Error.Brand.InvalidFileExtension", "Invalid file extension.");
    }

    public static class Cart
    {
        public static readonly Error EmptyProductIds =
            new("Error.Cart.EmptyProductIds", "ProductIds is empty.");

        public static Error ProductNotExistInCart(List<Guid> invalidProduct)
        {
            var message = invalidProduct.Count == 1
                ? $"Product {invalidProduct.First()} does not exist in cart."
                : $"Products {string.Join(", ", invalidProduct)} do not exist in cart.";
            return new("Error.Cart.ProductNotExistInCart", message);
        }

        public static readonly Error CartNotFoundOrEmpty =
            new("Error.Cart.CartNotFoundOrEmpty", $"Cart not found or empty.");

        public static Error ProductOutOfStock(List<string> unavailableProducts)
        {
            var message = unavailableProducts.Count == 1
                ? $"Product {unavailableProducts.FirstOrDefault()} is out of stock."
                : $"Products {string.Join(", ", unavailableProducts)} are out of stock.";
            return new("Error.Cart.ProductOutOfStock", message);
        }
    }

    public static class ApiService
    {
        public static readonly Error ApiCallFail =
            new("Error.ApiService.ApiCallFail", "Error when calling API.");

        public static Error DeserializeFail(string eMessage) => new("Error.ApiService.DeserializeFail",
            "Error when deserializing API response: " + eMessage);
    }

    public static class CustomerAddress
    {
        public static readonly Error LimitAddress =
            new("Error.CustomerAddress.LimitAddress",
                "You have reached the limit of addresses. Maximum is 3 addresses.");

        public static readonly Error DefaultAddressNotFoundOrMoreThanOne =
            new("Error.CustomerAddress.DefaultAddressNotFound", "Default address not found or more than one exists.");

        public static readonly Error DeleteDefaultAddress =
            new("Error.CustomerAddress.DeleteDefaultAddress",
                "Cannot delete default address. Please set another address as default before deleting.");
    }

    public static class Checkout
    {
        public static readonly Error PayOsError =
            new("Error.Checkout.PayOsError", "Error when creating PayOs payment link.");


        public static readonly Error VnPayError =
            new("Error.Checkout.VnPay", "Error when creating VnPay payment link.");
    }

    public static class PayOs
    {
        public static readonly Error PayOsWebhookError =
            new("Error.Checkout.WebhookError", "Error when handling PayOs webhook. Please check the signature.");

        public static readonly Error GetPaymentInfoError =
            new("Error.Checkout.GetPaymentInfoError", "Error when getting payment information.");

        public static readonly Error CancelPaymentLinkError =
            new("Error.Checkout.CancelPaymentLinkError", "Error when canceling payment link.");
    }

    public static class Order
    {
        public static readonly Error OrderStatusMustBeIncreased =
            new("Error.Order.OrderStatusMustBeIncreased",
                "Order status must be increased. (Pending -> Processing -> Shipping -> Delivered)");

        public static readonly Error GhnServiceError =
            new("Error.Order.GhnServiceError", "Error when creating Ghn shipping order.");

        public static readonly Error OrderCannotBeCancelledByAdmin =
            new("Error.Order.OrderCannotBeCancelledByAdmin",
                "Order cannot be cancelled by admin. Only pending or processing orders can be cancelled by admin.");

        public static readonly Error VnPayOrderCancelError =
            new("Error.Order.VnPayOrderCancelError",
                "Order with Vnpay service cannot be cancelled. Please cancel the payment link instead.");

        public static readonly Error OrderCannotBeCancelledByUser =
            new("Error.Order.OrderCannotBeCancelledByUser",
                "Order cannot be cancelled by user. Only pending orders can be cancelled by user.");
    }

    public static class ProductImage
    {
        public static Error UploadImageFail(string fileName) =>
            new("Error.ProductImage.UploadImageFail", $"Error when uploading image {fileName}.");

        public static readonly Error ExceedLimit =
            new("Error.ProductImage.ExceedLimit",
                "Exceed the limit of product images. Maximum is 10 images per product.");
    }

    public static class RedisCart
    {
        public static readonly Error NotExistProduct =
            new("Error.RedisCart.NotExistProduct", "Invalid product in redis cart. Product does not exist.");
    }

    public static class Voucher
    {
        public static readonly Error VoucherNotActive =
            new("Error.Voucher.NotActive", "Voucher is not active.");

        public static readonly Error VoucherExpired =
            new("Error.Voucher.Expired", "Voucher is expired.");

        public static readonly Error VoucherOutOfStock =
            new("Error.Voucher.OutOfStock", "Voucher is out of stock.");

        public static readonly Error TotalPriceNotEnough =
            new("Error.Voucher.TotalPriceNotEnough", "Total price is not enough to use this voucher.");
    }
}