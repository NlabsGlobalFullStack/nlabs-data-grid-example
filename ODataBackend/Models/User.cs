using System.Text.Json.Serialization;

namespace ODataBackend.Models;

public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("age")]
    public int Age { get; set; }
    
    [JsonPropertyName("salary")]
    public decimal Salary { get; set; }
    
    [JsonPropertyName("department")]
    public string Department { get; set; } = string.Empty;
    
    [JsonPropertyName("jobTitle")]
    public string JobTitle { get; set; } = string.Empty;
    
    [JsonPropertyName("manager")]
    public string Manager { get; set; } = string.Empty;
    
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;
    
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
    
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;
    
    [JsonPropertyName("zipCode")]
    public string ZipCode { get; set; } = string.Empty;
    
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
    
    [JsonPropertyName("active")]
    public bool Active { get; set; }
    
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }
    
    [JsonPropertyName("registeredDate")]
    public DateTime RegisteredDate { get; set; }
    
    [JsonPropertyName("lastLogin")]
    public DateTime LastLogin { get; set; }
    
    [JsonPropertyName("notes")]
    public string Notes { get; set; } = string.Empty;
}
