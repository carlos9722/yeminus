using System.Text.RegularExpressions;

namespace ServiceOrders.Api.Helpers;

public static partial class PhoneValidator
{
    [GeneratedRegex(@"^\+?[0-9][0-9\s\-]{6,18}[0-9]$", RegexOptions.Compiled)]
    private static partial Regex PhonePattern();

    public static bool IsValid(string? phone) =>
        !string.IsNullOrWhiteSpace(phone) && PhonePattern().IsMatch(phone.Trim());
}
