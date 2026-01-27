using System.ComponentModel.DataAnnotations;

namespace Restaurant.Web.Models
{
    public class OrderItemCreateVm
    {
        [Required]
        public Guid MenuItemId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; } = 1;
    }
}
