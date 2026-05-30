namespace ServiceOrders.Api.Infrastructure;

public static class EntitySqlFilters
{
    public const string ActiveOnly = "deleted_at IS NULL";
}
