using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace VStore.Infrastructure.PayOsService;

public class PayOsLibrary
{
    private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new PayOsCompare());
    private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new PayOsCompare());

    public void AddRequestData(string key, string? value)
    {
        if (value != null)
        {
            _requestData.Add(key, value);
        }
    }

    public void AddResponseData(string key, string? value)
    {
        if (value != null)
        {
            _responseData.Add(key, value);
        }
    }

    private string GetResponseData()
    {
        var data = new StringBuilder();
        foreach (var (key, value) in _responseData)
        {
            data.Append(key + "=" + value + "&");
        }

        //remove last '&'
        if (data.Length > 0)
        {
            data.Remove(data.Length - 1, 1);
        }

        return data.ToString();
    }

    public bool ValidateSignature(string secretKey, string signature)
    {
        var data = GetResponseData();
        var checkSum = Utils.HmacSha256(secretKey, data);
        return checkSum.Equals(signature, StringComparison.InvariantCultureIgnoreCase);
    }

    public string GenerateSignature(string secretKey)
    {
        var data = GetResponseData();
        return Utils.HmacSha256(secretKey, data);
    }


    public void ClearResponseData()
    {
        _responseData.Clear();
    }
}

public class Utils
{
    public static string HmacSha256(string key, string data)
    {
        // var keyBytes = Encoding.UTF8.GetBytes(key);
        // using (HMACSHA256 hmac = new HMACSHA256(keyBytes))
        // {
        //     var dataBytes = Encoding.UTF8.GetBytes(data);
        //     var hash = hmac.ComputeHash(dataBytes);
        //     StringBuilder stringBuilder = new StringBuilder();
        //     foreach (byte num in hash)
        //     {
        //         stringBuilder.Append(num.ToString("x2"));
        //     }
        //
        //     return stringBuilder.ToString();
        // }
        using (HMACSHA256 hmacshA256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
        {
            byte[] hash = hmacshA256.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num in hash)
                stringBuilder.Append(num.ToString("x2"));
            return stringBuilder.ToString();
        }
    }
}

internal class PayOsCompare : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y)
        {
            return 0;
        }

        if (x == null)
        {
            return -1;
        }

        if (y == null)
        {
            return 1;
        }

        var payOsCompare = CompareInfo.GetCompareInfo("en-US");
        return payOsCompare.Compare(x, y, CompareOptions.Ordinal);
    }
}