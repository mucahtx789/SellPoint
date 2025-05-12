namespace SellPoint.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer"; // Default olarak Customer

        // Müşteri bilgileri
        public string? Address { get; set; }

        public ICollection<Product>? Products { get; set; } // Satıcı ürünleri
        public ICollection<Order>? Orders { get; set; } // Müşteri siparişleri
    }
    
}
