using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backendProject.Models.DomainModels;

namespace backendProject.Models.JunctionModels
{
    public class CartProduct
    {
        [Key]
        public Guid CartProductId { get; set; } = Guid.NewGuid();   

        public required Guid CartId { get; set; } 
        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }


        public required Guid ProductId { get; set; } 
        [ForeignKey("ProductId")]
        public Product? Product { get; set; } 


        public required int Quantity { get; set; } = 1;

        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Weight { get; set; }


        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}