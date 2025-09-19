using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using backendProject.Models.DomainModels;

namespace backendProject.Models.JunctionModels
{
    public class CartProduct
    {
        [Key]
        public Guid CartProductId { get; set; } = Guid.NewGuid();

        public required Guid CartId { get; set; }
        [ForeignKey("CartId")]

        [JsonIgnore]
        public Cart? Cart { get; set; }


        public required Guid ProductId { get; set; }
        [ForeignKey("ProductId")]

        [JsonIgnore]
        public Product? Product { get; set; }


        public required int Quantity { get; set; } = 1;
        public required decimal ProductPrice { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? Weight { get; set; }


        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;
    }
}