using System.Diagnostics.Eventing.Reader;

namespace VStore.Application.CoreHelper;

public static class UrlHelper
{
    public const string GetProvinceUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province";

    public const string GetDistrictUrl =
        "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id=";

    public const string GetWardUrl =
        "https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id=";


    public const string CreateOrderUrl =
        "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create";

    public const string GetOrderUrl =
        "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/detail";

    public const string GetShippingFeeUrl =
        "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee";

    private static readonly string BasePath =
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    public static readonly string BrandLogoPath = Path.Combine(BasePath, "brands");
    public static readonly string ProductThumbnailPath = Path.Combine(BasePath, "products");
}