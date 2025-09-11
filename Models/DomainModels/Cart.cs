using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backendProject.Models.JunctionModels;

namespace backendProject.Models.DomainModels
{
    public class Cart
    {
        [Key]
        public Guid CartId { get; set; } = Guid.NewGuid();
        public required Guid UserId { get; set; }  
        [ForeignKey("UserId")]
        public User? Buyer { get; set; } 


        public ICollection<CartProduct> CartProducts { get; set; } = []; 

        public ICollection<Product> Products { get; set; } = [];



        public required decimal CartTotal { get; set; } = 0; 
    }
}