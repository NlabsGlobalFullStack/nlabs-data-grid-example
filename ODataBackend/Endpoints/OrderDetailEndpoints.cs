using Microsoft.EntityFrameworkCore;
using ODataBackend.Data;
using ODataBackend.Models;

namespace ODataBackend.Endpoints;

/// <summary>
/// Minimal API Endpoints for Order Details
/// </summary>
public static class OrderDetailEndpoints
{
    public static void MapOrderDetailEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/orderdetails")
            .WithTags("OrderDetails");

        // GET /api/orderdetails - OrderDetails with filtering
        group.MapGet("/", GetOrderDetails)
            .WithName("GetOrderDetails")
            .WithOpenApi();

        // GET /api/orderdetails/{id}
        group.MapGet("/{id:int}", GetOrderDetailById)
            .WithName("GetOrderDetailById")
            .WithOpenApi();

        // GET /api/orderdetails/categories
        group.MapGet("/categories", GetCategories)
            .WithName("GetProductCategories")
            .WithOpenApi();

        // GET /api/orderdetails/by-order/{orderId}
        group.MapGet("/by-order/{orderId:int}", GetByOrderId)
            .WithName("GetOrderDetailsByOrderId")
            .WithOpenApi();
    }

    /// <summary>
    /// GET /api/orderdetails
    /// Filterable by: orderId, productName, category
    /// </summary>
    private static async Task<IResult> GetOrderDetails(
        UserContext db,
        int skip = 0,
        int pageSize = 10,
        string? sort = null,
        int? orderId = null,
        string? productName = null,
        string? category = null)
    {
        var query = db.OrderDetails.AsQueryable();

        // Filter by order
        if (orderId.HasValue)
        {
            query = query.Where(od => od.OrderId == orderId.Value);
        }

        // Filter by product name
        if (!string.IsNullOrWhiteSpace(productName))
        {
            query = query.Where(od => od.ProductName.Contains(productName));
        }

        // Filter by category
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(od => od.Category == category);
        }

        // Apply sorting
        query = ApplySorting(query, sort);

        var total = await query.CountAsync();
        var data = await query.Skip(skip).Take(pageSize).ToListAsync();

        return Results.Ok(new { data, total });
    }

    private static async Task<IResult> GetOrderDetailById(UserContext db, int id)
    {
        var detail = await db.OrderDetails
            .Include(od => od.Order)
            .FirstOrDefaultAsync(od => od.Id == id);

        return detail is null ? Results.NotFound() : Results.Ok(detail);
    }

    private static async Task<IResult> GetByOrderId(UserContext db, int orderId)
    {
        var details = await db.OrderDetails
            .Where(od => od.OrderId == orderId)
            .ToListAsync();

        return Results.Ok(new { data = details, total = details.Count });
    }

    private static IResult GetCategories()
    {
        return Results.Ok(new[] { "Electronics", "Furniture", "Clothing", "Home & Kitchen", "Books" });
    }

    private static IQueryable<OrderDetail> ApplySorting(IQueryable<OrderDetail> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderBy(od => od.Id);
        }

        var descending = sort.EndsWith(" desc", StringComparison.OrdinalIgnoreCase);
        var field = sort
            .Replace(" desc", "", StringComparison.OrdinalIgnoreCase)
            .Replace(" asc", "", StringComparison.OrdinalIgnoreCase)
            .Trim()
            .ToLower();

        return field switch
        {
            "id" => descending ? query.OrderByDescending(od => od.Id) : query.OrderBy(od => od.Id),
            "productname" => descending ? query.OrderByDescending(od => od.ProductName) : query.OrderBy(od => od.ProductName),
            "category" => descending ? query.OrderByDescending(od => od.Category) : query.OrderBy(od => od.Category),
            "quantity" => descending ? query.OrderByDescending(od => od.Quantity) : query.OrderBy(od => od.Quantity),
            "unitprice" => descending ? query.OrderByDescending(od => od.UnitPrice) : query.OrderBy(od => od.UnitPrice),
            "linetotal" => descending ? query.OrderByDescending(od => od.LineTotal) : query.OrderBy(od => od.LineTotal),
            _ => query.OrderBy(od => od.Id)
        };
    }
}
