using Microsoft.EntityFrameworkCore;
using ODataBackend.Models;

namespace ODataBackend.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany()
            .HasForeignKey(od => od.OrderId);

        // Seed data - 500 users
        var users = GenerateMockUsers(500);
        modelBuilder.Entity<User>().HasData(users);

        // Seed data - Orders and OrderDetails
        var (orders, orderDetails) = GenerateMockOrders(users, 1000);
        modelBuilder.Entity<Order>().HasData(orders);
        modelBuilder.Entity<OrderDetail>().HasData(orderDetails);
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

    private static (List<Order>, List<OrderDetail>) GenerateMockOrders(List<User> users, int orderCount)
    {
        var random = new Random(123);
        var orders = new List<Order>();
        var orderDetails = new List<OrderDetail>();

        var statuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
        var paymentMethods = new[] { "Credit Card", "Debit Card", "PayPal", "Bank Transfer", "Cash on Delivery" };
        var products = new[]
        {
            ("Laptop", "TECH-001", "Electronics", 999.99m),
            ("Smartphone", "TECH-002", "Electronics", 699.99m),
            ("Tablet", "TECH-003", "Electronics", 449.99m),
            ("Headphones", "TECH-004", "Electronics", 149.99m),
            ("Monitor", "TECH-005", "Electronics", 299.99m),
            ("Keyboard", "TECH-006", "Electronics", 79.99m),
            ("Mouse", "TECH-007", "Electronics", 49.99m),
            ("Office Chair", "FURN-001", "Furniture", 249.99m),
            ("Desk", "FURN-002", "Furniture", 399.99m),
            ("Bookshelf", "FURN-003", "Furniture", 149.99m),
            ("T-Shirt", "CLOTH-001", "Clothing", 29.99m),
            ("Jeans", "CLOTH-002", "Clothing", 59.99m),
            ("Sneakers", "CLOTH-003", "Clothing", 89.99m),
            ("Jacket", "CLOTH-004", "Clothing", 129.99m),
            ("Coffee Maker", "HOME-001", "Home & Kitchen", 79.99m),
            ("Blender", "HOME-002", "Home & Kitchen", 49.99m),
            ("Toaster", "HOME-003", "Home & Kitchen", 34.99m),
            ("Vacuum Cleaner", "HOME-004", "Home & Kitchen", 199.99m),
            ("Book: Programming", "BOOK-001", "Books", 44.99m),
            ("Book: Business", "BOOK-002", "Books", 29.99m)
        };

        var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego" };
        var countries = new[] { "USA", "Canada", "UK", "Germany", "France" };
        var streets = new[] { "Main St", "Oak Ave", "Maple Dr", "Pine Rd", "Cedar Ln", "Elm St", "Park Ave", "Washington Blvd" };

        int orderDetailId = 1;

        for (int i = 1; i <= orderCount; i++)
        {
            var user = users[random.Next(users.Count)];
            var orderDate = DateTime.Now.AddDays(-random.Next(365 * 2)); // Son 2 yıl içinde
            var itemCount = random.Next(1, 6); // 1-5 ürün

            decimal subtotal = 0;
            var orderDetailsList = new List<OrderDetail>();

            // Order Details
            for (int j = 0; j < itemCount; j++)
            {
                var product = products[random.Next(products.Length)];
                var quantity = random.Next(1, 4);
                var discount = random.Next(100) < 20 ? product.Item4 * 0.1m : 0; // %20 ihtimalle %10 indirim
                var lineTotal = (product.Item4 * quantity) - discount;

                orderDetailsList.Add(new OrderDetail
                {
                    Id = orderDetailId++,
                    OrderId = i,
                    ProductName = product.Item1,
                    ProductCode = product.Item2,
                    Category = product.Item3,
                    Quantity = quantity,
                    UnitPrice = product.Item4,
                    Discount = discount,
                    LineTotal = lineTotal
                });

                subtotal += lineTotal;
            }

            var tax = subtotal * 0.08m; // %8 vergi
            var shippingCost = subtotal > 100 ? 0 : 9.99m; // 100$ üzeri ücretsiz kargo
            var totalAmount = subtotal + tax + shippingCost;

            orders.Add(new Order
            {
                Id = i,
                OrderNumber = $"ORD-{orderDate.Year}-{i:D6}",
                OrderDate = orderDate,
                UserId = user.Id,
                CustomerName = user.Name,
                CustomerEmail = user.Email,
                ShippingAddress = $"{random.Next(100, 9999)} {streets[random.Next(streets.Length)]}",
                City = cities[random.Next(cities.Length)],
                Country = countries[random.Next(countries.Length)],
                ZipCode = random.Next(10000, 99999).ToString(),
                Status = statuses[random.Next(statuses.Length)],
                PaymentMethod = paymentMethods[random.Next(paymentMethods.Length)],
                Subtotal = Math.Round(subtotal, 2),
                Tax = Math.Round(tax, 2),
                ShippingCost = Math.Round(shippingCost, 2),
                TotalAmount = Math.Round(totalAmount, 2),
                Notes = random.Next(100) < 30 ? $"Special instructions for order {i}" : string.Empty
            });

            orderDetails.AddRange(orderDetailsList);
        }

        return (orders, orderDetails);
    }
}
