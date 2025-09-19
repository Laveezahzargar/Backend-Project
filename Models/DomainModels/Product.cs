using System.ComponentModel.DataAnnotations;
using backendProject.Models.JunctionModels;
using backendProject.Types.Enums;

namespace backendProject.Models.DomainModels
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; } = Guid.NewGuid();
        public required string ProductName { get; set; }
        public required string ProductDescription { get; set; }
        public required string ProductImage { get; set; }
        public required decimal ProductPrice { get; set; }
        public required int ProductStock { get; set; }
        public ProductCategory Category { get; set; } = ProductCategory.All;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Weight { get; set; }
        public ICollection<CartProduct> ProductInCarts { get; set; } = []; 
        public ICollection<OrderProduct> ProductInOrders { get; set; } = []; 
        public bool IsAvailable { get; set; } = true;
        public bool IsArchived { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;



    }
}