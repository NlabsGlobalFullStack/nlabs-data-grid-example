using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ODataBackend.Data;
using ODataBackend.Models;

namespace ODataBackend.Controllers;

/// <summary>
/// REST API Controller for Orders (Alternative endpoint)
/// Endpoint: /api/ordersrest
///
/// Supports filtering by:
/// - filter: OData-style filter (e.g., "contains(customerName, 'John')")
/// - search: Global search across multiple fields
/// - customerName: Search by customer name (contains)
/// - startDate: Filter orders from this date
/// - endDate: Filter orders until this date
/// - status: Filter by order status
/// </summary>
[ApiController]
[Route("api/ordersrest")]
public class OrdersRestController : ControllerBase
{
    private readonly UserContext _context;

    public OrdersRestController(UserContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET /api/ordersrest
    /// Query params: skip, pageSize, sort, filter, search, customerName, startDate, endDate, status
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        int skip = 0,
        int pageSize = 10,
        string? sort = null,
        string? filter = null,
        string? search = null,
        string? customerName = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? status = null)
    {
        var query = _context.Orders.AsQueryable();

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

        // Filter by customer name (contains search) - legacy support
        if (!string.IsNullOrWhiteSpace(customerName))
        {
            query = query.Where(o => o.CustomerName.Contains(customerName));
        }

        // Filter by date range
        if (startDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= endDate.Value);
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
        var data = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new { data, total });
    }

    /// <summary>
    /// GET /api/ordersrest/{id}
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    /// <summary>
    /// GET /api/ordersrest/statuses
    /// Returns available order statuses for dropdown
    /// </summary>
    [HttpGet("statuses")]
    public IActionResult GetStatuses()
    {
        var statuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
        return Ok(statuses);
    }

    /// <summary>
    /// POST /api/ordersrest
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    /// <summary>
    /// PUT /api/ordersrest/{id}
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
    {
        var existingOrder = await _context.Orders.FindAsync(id);
        if (existingOrder == null)
        {
            return NotFound();
        }

        existingOrder.OrderNumber = order.OrderNumber;
        existingOrder.OrderDate = order.OrderDate;
        existingOrder.CustomerName = order.CustomerName;
        existingOrder.CustomerEmail = order.CustomerEmail;
        existingOrder.ShippingAddress = order.ShippingAddress;
        existingOrder.City = order.City;
        existingOrder.Country = order.Country;
        existingOrder.ZipCode = order.ZipCode;
        existingOrder.Status = order.Status;
        existingOrder.PaymentMethod = order.PaymentMethod;
        existingOrder.Subtotal = order.Subtotal;
        existingOrder.Tax = order.Tax;
        existingOrder.ShippingCost = order.ShippingCost;
        existingOrder.TotalAmount = order.TotalAmount;
        existingOrder.Notes = order.Notes;

        await _context.SaveChangesAsync();
        return Ok(existingOrder);
    }

    /// <summary>
    /// DELETE /api/ordersrest/{id}
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return NoContent();
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
            .Trim();

        return field.ToLower() switch
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
    /// Supports: contains(field, 'value'), startswith(field, 'value'), endswith(field, 'value'), 
    /// field eq 'value', field ne 'value', field lt value, field le value, field gt value, field ge value
    /// Multiple filters can be combined with 'and'
    /// </summary>
    private static IQueryable<Order> ApplyODataFilter(IQueryable<Order> query, string filter)
    {
        // Split by 'and' for multiple conditions
        var conditions = filter.Split(new[] { " and " }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var condition in conditions)
        {
            var trimmedCondition = condition.Trim();
            query = ApplySingleFilter(query, trimmedCondition);
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
                var value = match.Groups[2].Value;
                query = ApplyContainsFilter(query, field, value);
            }
            return query;
        }

        // Handle startswith(field, 'value')
        if (condition.StartsWith("startswith(", StringComparison.OrdinalIgnoreCase))
        {
            var match = System.Text.RegularExpressions.Regex.Match(condition, @"startswith\((\w+),\s*'([^']*)'\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var field = match.Groups[1].Value.ToLower();
                var value = match.Groups[2].Value;
                query = ApplyStartsWithFilter(query, field, value);
            }
            return query;
        }

        // Handle endswith(field, 'value')
        if (condition.StartsWith("endswith(", StringComparison.OrdinalIgnoreCase))
        {
            var match = System.Text.RegularExpressions.Regex.Match(condition, @"endswith\((\w+),\s*'([^']*)'\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var field = match.Groups[1].Value.ToLower();
                var value = match.Groups[2].Value;
                query = ApplyEndsWithFilter(query, field, value);
            }
            return query;
        }

        // Handle comparison operators: eq, ne, lt, le, gt, ge
        var compMatch = System.Text.RegularExpressions.Regex.Match(condition, @"(\w+)\s+(eq|ne|lt|le|gt|ge)\s+('([^']*)'|(\d+\.?\d*))", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (compMatch.Success)
        {
            var field = compMatch.Groups[1].Value.ToLower();
            var op = compMatch.Groups[2].Value.ToLower();
            var value = compMatch.Groups[4].Success ? compMatch.Groups[4].Value : compMatch.Groups[5].Value;
            query = ApplyComparisonFilter(query, field, op, value);
        }

        return query;
    }

    private static IQueryable<Order> ApplyContainsFilter(IQueryable<Order> query, string field, string value)
    {
        return field switch
        {
            "customername" => query.Where(o => o.CustomerName.ToLower().Contains(value.ToLower())),
            "ordernumber" => query.Where(o => o.OrderNumber.ToLower().Contains(value.ToLower())),
            "city" => query.Where(o => o.City.ToLower().Contains(value.ToLower())),
            "country" => query.Where(o => o.Country.ToLower().Contains(value.ToLower())),
            "status" => query.Where(o => o.Status.ToLower().Contains(value.ToLower())),
            "customeremail" => query.Where(o => o.CustomerEmail.ToLower().Contains(value.ToLower())),
            "shippingaddress" => query.Where(o => o.ShippingAddress.ToLower().Contains(value.ToLower())),
            "paymentmethod" => query.Where(o => o.PaymentMethod.ToLower().Contains(value.ToLower())),
            "notes" => query.Where(o => o.Notes != null && o.Notes.ToLower().Contains(value.ToLower())),
            _ => query
        };
    }

    private static IQueryable<Order> ApplyStartsWithFilter(IQueryable<Order> query, string field, string value)
    {
        return field switch
        {
            "customername" => query.Where(o => o.CustomerName.ToLower().StartsWith(value.ToLower())),
            "ordernumber" => query.Where(o => o.OrderNumber.ToLower().StartsWith(value.ToLower())),
            "city" => query.Where(o => o.City.ToLower().StartsWith(value.ToLower())),
            "country" => query.Where(o => o.Country.ToLower().StartsWith(value.ToLower())),
            "status" => query.Where(o => o.Status.ToLower().StartsWith(value.ToLower())),
            _ => query
        };
    }

    private static IQueryable<Order> ApplyEndsWithFilter(IQueryable<Order> query, string field, string value)
    {
        return field switch
        {
            "customername" => query.Where(o => o.CustomerName.ToLower().EndsWith(value.ToLower())),
            "ordernumber" => query.Where(o => o.OrderNumber.ToLower().EndsWith(value.ToLower())),
            "city" => query.Where(o => o.City.ToLower().EndsWith(value.ToLower())),
            "country" => query.Where(o => o.Country.ToLower().EndsWith(value.ToLower())),
            "status" => query.Where(o => o.Status.ToLower().EndsWith(value.ToLower())),
            _ => query
        };
    }

    private static IQueryable<Order> ApplyComparisonFilter(IQueryable<Order> query, string field, string op, string value)
    {
        // Handle string equality
        if (field is "customername" or "ordernumber" or "city" or "country" or "status")
        {
            return op switch
            {
                "eq" => field switch
                {
                    "customername" => query.Where(o => o.CustomerName.ToLower() == value.ToLower()),
                    "ordernumber" => query.Where(o => o.OrderNumber.ToLower() == value.ToLower()),
                    "city" => query.Where(o => o.City.ToLower() == value.ToLower()),
                    "country" => query.Where(o => o.Country.ToLower() == value.ToLower()),
                    "status" => query.Where(o => o.Status.ToLower() == value.ToLower()),
                    _ => query
                },
                "ne" => field switch
                {
                    "customername" => query.Where(o => o.CustomerName.ToLower() != value.ToLower()),
                    "ordernumber" => query.Where(o => o.OrderNumber.ToLower() != value.ToLower()),
                    "city" => query.Where(o => o.City.ToLower() != value.ToLower()),
                    "country" => query.Where(o => o.Country.ToLower() != value.ToLower()),
                    "status" => query.Where(o => o.Status.ToLower() != value.ToLower()),
                    _ => query
                },
                _ => query
            };
        }

        // Handle numeric comparisons
        if (decimal.TryParse(value, out var numValue))
        {
            return field switch
            {
                "id" => op switch
                {
                    "eq" => query.Where(o => o.Id == (int)numValue),
                    "ne" => query.Where(o => o.Id != (int)numValue),
                    "lt" => query.Where(o => o.Id < (int)numValue),
                    "le" => query.Where(o => o.Id <= (int)numValue),
                    "gt" => query.Where(o => o.Id > (int)numValue),
                    "ge" => query.Where(o => o.Id >= (int)numValue),
                    _ => query
                },
                "totalamount" => op switch
                {
                    "eq" => query.Where(o => o.TotalAmount == numValue),
                    "ne" => query.Where(o => o.TotalAmount != numValue),
                    "lt" => query.Where(o => o.TotalAmount < numValue),
                    "le" => query.Where(o => o.TotalAmount <= numValue),
                    "gt" => query.Where(o => o.TotalAmount > numValue),
                    "ge" => query.Where(o => o.TotalAmount >= numValue),
                    _ => query
                },
                "subtotal" => op switch
                {
                    "eq" => query.Where(o => o.Subtotal == numValue),
                    "ne" => query.Where(o => o.Subtotal != numValue),
                    "lt" => query.Where(o => o.Subtotal < numValue),
                    "le" => query.Where(o => o.Subtotal <= numValue),
                    "gt" => query.Where(o => o.Subtotal > numValue),
                    "ge" => query.Where(o => o.Subtotal >= numValue),
                    _ => query
                },
                _ => query
            };
        }

        return query;
    }
}
