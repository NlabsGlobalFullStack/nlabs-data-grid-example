using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataBackend.Data;
using ODataBackend.Models;

namespace ODataBackend.Controllers;

/// <summary>
/// OData Controller for Orders
/// Endpoint: /odata/Orders
/// </summary>
public class OrdersController : ODataController
{
    private readonly UserContext _context;

    public OrdersController(UserContext context)
    {
        _context = context;
    }

    [EnableQuery(PageSize = 20, MaxTop = 1000)]
    public IActionResult Get()
    {
        return Ok(_context.Orders);
    }

    [EnableQuery]
    public IActionResult Get(int key)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == key);

        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Order order)
    {
        _context.Orders.Add(order);
        _context.SaveChanges();
        return Created(order);
    }

    [HttpPut]
    public IActionResult Put(int key, [FromBody] Order order)
    {
        var existingOrder = _context.Orders.Find(key);
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

        _context.SaveChanges();
        return Updated(existingOrder);
    }

    [HttpDelete]
    public IActionResult Delete(int key)
    {
        var order = _context.Orders.Find(key);
        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        _context.SaveChanges();
        return NoContent();
    }
}
