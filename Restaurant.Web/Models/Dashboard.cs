using Restaurant.Domain.Enums;

namespace Restaurant.Web.Models
{
    public class DashboardVm
    {
        public int CategoriesCount { get; set; }
        public int MenuItemsCount { get; set; }
        public int AvailableMenuItemsCount { get; set; }

        public int OrdersCount { get; set; }
        public decimal TotalRevenue { get; set; }

        public Dictionary<OrderStatus, int> OrdersByStatus { get; set; } = new();
        public string? TopItemName { get; set; }
        public string? TopItemImageUrl { get; set; }
        public int TopItemQuantity { get; set; }

    }
}
