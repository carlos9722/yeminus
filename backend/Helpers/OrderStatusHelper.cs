namespace ServiceOrders.Api.Helpers;

public static class OrderStatusHelper
{
    public const string Pendiente = "Pendiente";
    public const string EnProgreso = "EnProgreso";
    public const string Finalizada = "Finalizada";

    private static readonly HashSet<string> ValidStatuses =
        new(StringComparer.Ordinal) { Pendiente, EnProgreso, Finalizada };

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) && ValidStatuses.Contains(status.Trim());

    public static string Normalize(string status) => status.Trim();
}
