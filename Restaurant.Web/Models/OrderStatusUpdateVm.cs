using System.ComponentModel.DataAnnotations;
using Restaurant.Domain.Enums;

namespace Restaurant.Web.Models
{
    public class OrderStatusUpdateVm
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public OrderStatus Status { get; set; }
    }
}
