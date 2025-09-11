using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backendProject.Models.JunctionModels;
using backendProject.Types.OrderStatus;
using backendProject.Types.PaymentMode;
using backendProject.Types.PaymentStatus;

namespace backendProject.Models.DomainModels
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; } = Guid.NewGuid();

        public required Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User? Buyer { get; set; }


        public required Guid AddressId { get; set; }
        [ForeignKey("AddressId")]
        public Address? Address { get; set; }


        public required ICollection<OrderProduct> OrderProducts { get; set; } = [];


        public required decimal TotalPrice { get; set; } = 0;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public PaymentMode PaymentMode { get; set; } = PaymentMode.None;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime? ShippingDate { get; set; } = DateTime.UtcNow.AddDays(7);
    }
}