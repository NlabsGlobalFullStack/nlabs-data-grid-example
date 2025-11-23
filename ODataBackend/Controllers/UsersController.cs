using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataBackend.Data;
using ODataBackend.Models;

namespace ODataBackend.Controllers;

public class UsersController : ODataController
{
    private readonly UserContext _context;

    public UsersController(UserContext context)
    {
        _context = context;
    }

    [EnableQuery(PageSize = 20, MaxTop = 1000)]
    public IActionResult Get()
    {
        return Ok(_context.Users);
    }

    [EnableQuery]
    public IActionResult Get(int key)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == key);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Post([FromBody] User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return Created(user);
    }

    [HttpPut]
    public IActionResult Put(int key, [FromBody] User user)
    {
        var existingUser = _context.Users.Find(key);
        if (existingUser == null)
        {
            return NotFound();
        }

        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.Age = user.Age;
        existingUser.Salary = user.Salary;
        existingUser.Department = user.Department;
        existingUser.JobTitle = user.JobTitle;
        existingUser.Manager = user.Manager;
        existingUser.City = user.City;
        existingUser.Country = user.Country;
        existingUser.Address = user.Address;
        existingUser.ZipCode = user.ZipCode;
        existingUser.Phone = user.Phone;
        existingUser.Active = user.Active;
        existingUser.StartDate = user.StartDate;
        existingUser.RegisteredDate = user.RegisteredDate;
        existingUser.LastLogin = user.LastLogin;
        existingUser.Notes = user.Notes;

        _context.SaveChanges();
        return Updated(user);
    }

    [HttpDelete]
    public IActionResult Delete(int key)
    {
        var user = _context.Users.Find(key);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        _context.SaveChanges();
        return NoContent();
    }
}
