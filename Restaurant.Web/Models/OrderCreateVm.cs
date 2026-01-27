using System.ComponentModel.DataAnnotations;

namespace Restaurant.Web.Models
{
    public class OrderCreateVm
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string DeliveryAddress { get; set; } = string.Empty;

        public List<OrderItemCreateVm> Items { get; set; } = new();
    }
}
