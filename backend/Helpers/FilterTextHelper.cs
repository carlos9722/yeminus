namespace ServiceOrders.Api.Helpers;

public static class FilterTextHelper
{
    public static string? ToLikePattern(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return $"%{value.Trim()}%";
    }

    /// <summary>
    /// Solo dígitos, para comparar documentos con o sin puntos.
    /// </summary>
    public static string? ToDigitsOnly(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var digits = new string(value.Where(char.IsDigit).ToArray());
        return digits.Length == 0 ? null : digits;
    }
}
