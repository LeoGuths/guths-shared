using System.Security.Cryptography;
using System.Text;

namespace Guths.Shared.Helpers;

public static class ErrorCodeGeneratorHelper
{
    public static string GenerateErrorCode(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringBuilder = new StringBuilder();
        using (var rng = RandomNumberGenerator.Create())
        {
            var buffer = new byte[length];
            rng.GetBytes(buffer);
            foreach (var b in buffer)
            {
                stringBuilder.Append(chars[b % chars.Length]);
            }
        }
        return stringBuilder.ToString();
    }
}
