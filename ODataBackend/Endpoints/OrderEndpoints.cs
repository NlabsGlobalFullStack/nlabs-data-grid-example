using Microsoft.EntityFrameworkCore;
using ODataBackend.Data;
using ODataBackend.Models;

namespace ODataBackend.Endpoints;

/// <summary>
/// Minimal API Endpoints for Orders
/// Organized as extension methods for cleaner Program.cs
/// </summary>
public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Orders");

        // GET /api/orders - Orders with filtering
        // Filters: customerName, startDate, endDate, productName, status
        group.MapGet("/", GetOrders)
            .WithName("GetOrders")
            .WithOpenApi();

        // GET /api/orders/{id}
        group.MapGet("/{id:int}", GetOrderById)
            .WithName("GetOrderById")
            .WithOpenApi();

        // GET /api/orders/statuses
        group.MapGet("/statuses", GetStatuses)
            .WithName("GetOrderStatuses")
            .WithOpenApi();

        // POST /api/orders
        group.MapPost("/", CreateOrder)
            .WithName("CreateOrder")
            .WithOpenApi();

        // PUT /api/orders/{id}
        group.MapPut("/{id:int}", UpdateOrder)
            .WithName("UpdateOrder")
            .WithOpenApi();

        // DELETE /api/orders/{id}
        group.MapDelete("/{id:int}", DeleteOrder)
            .WithName("DeleteOrder")
            .WithOpenApi();
    }

    /// <summary>
    /// GET /api/orders
    /// Filterable by: filter (OData-style), search, customerName, startDate, endDate, productName, status
    /// </summary>
    private static async Task<IResult> GetOrders(
        UserContext db,
        int skip = 0,
        int pageSize = 10,
        string? sort = null,
        string? filter = null,
        string? search = null,
        string? customerName = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? productName = null,
        string? status = null)
    {
        var query = db.Orders.AsQueryable();

        // Apply OData-style filter (e.g., "contains(customerName, 'John')")
        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = ApplyODataFilter(query, filter);
        }

        // Global search across multiple fields
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.ToLower();
            query = query.Where(o =>
                o.CustomerName.ToLower().Contains(searchTerm) ||
                o.OrderNumber.ToLower().Contains(searchTerm) ||
                o.City.ToLower().Contains(searchTerm) ||
                o.Country.ToLower().Contains(searchTerm) ||
                o.Status.ToLower().Contains(searchTerm) ||
                o.CustomerEmail.ToLower().Contains(searchTerm)
            );
        }

        // Filter by customer name (user fullname)
        if (!string.IsNullOrWhiteSpace(customerName))
        {
            query = query.Where(o => o.CustomerName.Contains(customerName));
        }

        // Filter by date range (orderDate)
        if (startDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= endDate.Value);
        }

        // Filter by product name (orders containing this product)
        if (!string.IsNullOrWhiteSpace(productName))
        {
            var orderIdsWithProduct = db.OrderDetails
                .Where(od => od.ProductName.Contains(productName))
                .Select(od => od.OrderId)
                .Distinct();
            query = query.Where(o => orderIdsWithProduct.Contains(o.Id));
        }

        // Filter by status
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(o => o.Status == status);
        }

        // Apply sorting
        query = ApplySorting(query, sort);

        // Get total count before pagination
        var total = await query.CountAsync();

        // Apply pagination
        var data = await query.Skip(skip).Take(pageSize).ToListAsync();

        return Results.Ok(new { data, total });
    }

    private static async Task<IResult> GetOrderById(UserContext db, int id)
    {
        var order = await db.Orders.FindAsync(id);
        return order is null ? Results.NotFound() : Results.Ok(order);
    }

    private static IResult GetStatuses()
    {
        return Results.Ok(new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" });
    }

    private static async Task<IResult> CreateOrder(UserContext db, Order order)
    {
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return Results.Created($"/api/orders/{order.Id}", order);
    }

    private static async Task<IResult> UpdateOrder(UserContext db, int id, Order order)
    {
        var existing = await db.Orders.FindAsync(id);
        if (existing is null) return Results.NotFound();

        existing.OrderNumber = order.OrderNumber;
        existing.OrderDate = order.OrderDate;
        existing.CustomerName = order.CustomerName;
        existing.CustomerEmail = order.CustomerEmail;
        existing.ShippingAddress = order.ShippingAddress;
        existing.City = order.City;
        existing.Country = order.Country;
        existing.ZipCode = order.ZipCode;
        existing.Status = order.Status;
        existing.PaymentMethod = order.PaymentMethod;
        existing.Subtotal = order.Subtotal;
        existing.Tax = order.Tax;
        existing.ShippingCost = order.ShippingCost;
        existing.TotalAmount = order.TotalAmount;
        existing.Notes = order.Notes;

        await db.SaveChangesAsync();
        return Results.Ok(existing);
    }

    private static async Task<IResult> DeleteOrder(UserContext db, int id)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null) return Results.NotFound();

        db.Orders.Remove(order);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static IQueryable<Order> ApplySorting(IQueryable<Order> query, string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderByDescending(o => o.OrderDate);
        }

        var descending = sort.EndsWith(" desc", StringComparison.OrdinalIgnoreCase);
        var field = sort
            .Replace(" desc", "", StringComparison.OrdinalIgnoreCase)
            .Replace(" asc", "", StringComparison.OrdinalIgnoreCase)
            .Trim()
            .ToLower();

        return field switch
        {
            "id" => descending ? query.OrderByDescending(o => o.Id) : query.OrderBy(o => o.Id),
            "ordernumber" => descending ? query.OrderByDescending(o => o.OrderNumber) : query.OrderBy(o => o.OrderNumber),
            "orderdate" => descending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate),
            "customername" => descending ? query.OrderByDescending(o => o.CustomerName) : query.OrderBy(o => o.CustomerName),
            "status" => descending ? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status),
            "totalamount" => descending ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount),
            "city" => descending ? query.OrderByDescending(o => o.City) : query.OrderBy(o => o.City),
            "country" => descending ? query.OrderByDescending(o => o.Country) : query.OrderBy(o => o.Country),
            _ => query.OrderByDescending(o => o.OrderDate)
        };
    }

    /// <summary>
    /// Parse and apply OData-style filter expressions
    /// </summary>
    private static IQueryable<Order> ApplyODataFilter(IQueryable<Order> query, string filter)
    {
        var conditions = filter.Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var condition in conditions)
        {
            query = ApplySingleFilter(query, condition.Trim());
        }

        return query;
    }

    private static IQueryable<Order> ApplySingleFilter(IQueryable<Order> query, string condition)
    {
        // Handle contains(field, 'value')
        if (condition.StartsWith("contains(", StringComparison.OrdinalIgnoreCase))
        {
            var match = System.Text.RegularExpressions.Regex.Match(condition, @"contains\((\w+),\s*'([^']*)'\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var field = match.Groups[1].Value.ToLower();
                var value = match.Groups[2].Value.ToLower();
                return field switch
                {
                    "customername" => query.Where(o => o.CustomerName.ToLower().Contains(value)),
                    "ordernumber" => query.Where(o => o.OrderNumber.ToLower().Contains(value)),
                    "city" => query.Where(o => o.City.ToLower().Contains(value)),
                    "country" => query.Where(o => o.Country.ToLower().Contains(value)),
                    "status" => query.Where(o => o.Status.ToLower().Contains(value)),
                    "customeremail" => query.Where(o => o.CustomerEmail.ToLower().Contains(value)),
                    _ => query
                };
            }
        }

        // Handle startswith(field, 'value')
        if (condition.StartsWith("startswith(", StringComparison.OrdinalIgnoreCase))
        {
            var match = System.Text.RegularExpressions.Regex.Match(condition, @"startswith\((\w+),\s*'([^']*)'\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var field = match.Groups[1].Value.ToLower();
                var value = match.Groups[2].Value.ToLower();
                return field switch
                {
                    "customername" => query.Where(o => o.CustomerName.ToLower().StartsWith(value)),
                    "ordernumber" => query.Where(o => o.OrderNumber.ToLower().StartsWith(value)),
                    "city" => query.Where(o => o.City.ToLower().StartsWith(value)),
                    "country" => query.Where(o => o.Country.ToLower().StartsWith(value)),
                    "status" => query.Where(o => o.Status.ToLower().StartsWith(value)),
                    _ => query
                };
            }
        }

        // Handle endswith(field, 'value')
        if (condition.StartsWith("endswith(", StringComparison.OrdinalIgnoreCase))
        {
            var match = System.Text.RegularExpressions.Regex.Match(condition, @"endswith\((\w+),\s*'([^']*)'\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var field = match.Groups[1].Value.ToLower();
                var value = match.Groups[2].Value.ToLower();
                return field switch
                {
                    "customername" => query.Where(o => o.CustomerName.ToLower().EndsWith(value)),
                    "ordernumber" => query.Where(o => o.OrderNumber.ToLower().EndsWith(value)),
                    "city" => query.Where(o => o.City.ToLower().EndsWith(value)),
                    "country" => query.Where(o => o.Country.ToLower().EndsWith(value)),
                    "status" => query.Where(o => o.Status.ToLower().EndsWith(value)),
                    _ => query
                };
            }
        }

        // Handle eq operator: field eq 'value' or field eq number
        var eqMatch = System.Text.RegularExpressions.Regex.Match(condition, @"(\w+)\s+eq\s+('([^']*)'|(\d+\.?\d*))", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (eqMatch.Success)
        {
            var field = eqMatch.Groups[1].Value.ToLower();
            var value = eqMatch.Groups[3].Success ? eqMatch.Groups[3].Value : eqMatch.Groups[4].Value;
            
            if (field is "customername" or "ordernumber" or "city" or "country" or "status")
            {
                return field switch
                {
                    "customername" => query.Where(o => o.CustomerName.ToLower() == value.ToLower()),
                    "ordernumber" => query.Where(o => o.OrderNumber.ToLower() == value.ToLower()),
                    "city" => query.Where(o => o.City.ToLower() == value.ToLower()),
                    "country" => query.Where(o => o.Country.ToLower() == value.ToLower()),
                    "status" => query.Where(o => o.Status.ToLower() == value.ToLower()),
                    _ => query
                };
            }
            
            if (decimal.TryParse(value, out var numValue))
            {
                return field switch
                {
                    "id" => query.Where(o => o.Id == (int)numValue),
                    "totalamount" => query.Where(o => o.TotalAmount == numValue),
                    _ => query
                };
            }
        }

        // Handle numeric comparisons: lt, le, gt, ge
        var compMatch = System.Text.RegularExpressions.Regex.Match(condition, @"(\w+)\s+(lt|le|gt|ge)\s+(\d+\.?\d*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (compMatch.Success)
        {
            var field = compMatch.Groups[1].Value.ToLower();
            var op = compMatch.Groups[2].Value.ToLower();
            if (decimal.TryParse(compMatch.Groups[3].Value, out var numValue))
            {
                return (field, op) switch
                {
                    ("id", "lt") => query.Where(o => o.Id < (int)numValue),
                    ("id", "le") => query.Where(o => o.Id <= (int)numValue),
                    ("id", "gt") => query.Where(o => o.Id > (int)numValue),
                    ("id", "ge") => query.Where(o => o.Id >= (int)numValue),
                    ("totalamount", "lt") => query.Where(o => o.TotalAmount < numValue),
                    ("totalamount", "le") => query.Where(o => o.TotalAmount <= numValue),
                    ("totalamount", "gt") => query.Where(o => o.TotalAmount > numValue),
                    ("totalamount", "ge") => query.Where(o => o.TotalAmount >= numValue),
                    _ => query
                };
            }
        }

        return query;
    }
}
