using Microsoft.EntityFrameworkCore;
using ODataBackend.Models;

namespace ODataBackend.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data - 500 users
        var users = GenerateMockUsers(500);
        modelBuilder.Entity<User>().HasData(users);
    }

    private static List<User> GenerateMockUsers(int count)
    {
        var random = new Random(42);
        var firstNames = new[] { "John", "Jane", "Michael", "Emily", "David", "Sarah", "Chris", "Lisa", "Daniel", "Emma", 
            "James", "Olivia", "Robert", "Sophia", "William", "Ava", "Joseph", "Isabella", "Charles", "Mia" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin" };
        var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego",
            "Dallas", "San Jose", "Austin", "Jacksonville", "Fort Worth", "Columbus", "Charlotte", "San Francisco", "Indianapolis",
            "Seattle", "Denver", "Boston" };
        var countries = new[] { "USA", "Canada", "UK", "Germany", "France", "Spain", "Italy", "Australia", "Japan", "Brazil" };
        var departments = new[] { "Engineering", "Marketing", "Sales", "HR", "Finance", "Operations", "IT", "Customer Service", "R&D", "Legal" };
        var jobTitles = new[] { "Software Engineer", "Senior Developer", "Project Manager", "Team Lead", "Business Analyst",
            "Data Scientist", "DevOps Engineer", "QA Engineer", "UX Designer", "Product Manager" };
        var streets = new[] { "Main St", "Oak Ave", "Maple Dr", "Pine Rd", "Cedar Ln", "Elm St", "Park Ave", "Washington Blvd",
            "Lake Dr", "Hill Rd" };

        var users = new List<User>();
        for (int i = 1; i <= count; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var startDate = DateTime.Now.AddDays(-random.Next(365 * 5));
            
            users.Add(new User
            {
                Id = i,
                Name = $"{firstName} {lastName}",
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@example.com",
                Age = random.Next(22, 65),
                Salary = random.Next(40000, 150000),
                Department = departments[random.Next(departments.Length)],
                JobTitle = jobTitles[random.Next(jobTitles.Length)],
                Manager = $"{firstNames[random.Next(firstNames.Length)]} {lastNames[random.Next(lastNames.Length)]}",
                City = cities[random.Next(cities.Length)],
                Country = countries[random.Next(countries.Length)],
                Address = $"{random.Next(100, 9999)} {streets[random.Next(streets.Length)]}",
                ZipCode = random.Next(10000, 99999).ToString(),
                Phone = $"+1-{random.Next(200, 999)}-{random.Next(200, 999)}-{random.Next(1000, 9999)}",
                Active = random.Next(100) > 10,
                StartDate = startDate,
                RegisteredDate = DateTime.Now.AddDays(-random.Next(365 * 3)),
                LastLogin = DateTime.Now.AddDays(-random.Next(30)),
                Notes = $"Employee notes for {firstName} {lastName}"
            });
        }

        return users;
    }
}
