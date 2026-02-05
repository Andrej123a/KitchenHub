using Microsoft.AspNetCore.Mvc;
using Restaurant.Domain.Enums;
using Restaurant.Service.Services;
using Restaurant.Web.Models;

namespace Restaurant.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMenuItemService _menuItemService;
        private readonly IOrderService _orderService;

        private readonly IMealRecommendationService _mealRecService;

        public HomeController(
            ICategoryService categoryService,
            IMenuItemService menuItemService,
            IOrderService orderService,
            IMealRecommendationService mealRecService) // ✅ NEW injection
        {
            _categoryService = categoryService;
            _menuItemService = menuItemService;
            _orderService = orderService;

            _mealRecService = mealRecService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            var menuItems = await _menuItemService.GetAllAsync();
            var orders = await _orderService.GetAllAsync();

            // Total revenue 
            var totalRevenue = orders.Sum(o =>
                (o.Items?.Sum(i => i.UnitPrice * i.Quantity) ?? 0m)
            );

            var vm = new DashboardVm
            {
                CategoriesCount = categories.Count,
                MenuItemsCount = menuItems.Count,
                AvailableMenuItemsCount = menuItems.Count(m => m.IsAvailable),
                OrdersCount = orders.Count,
                TotalRevenue = totalRevenue,
                OrdersByStatus = orders
                    .GroupBy(o => o.Status)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            foreach (OrderStatus s in Enum.GetValues(typeof(OrderStatus)))
                if (!vm.OrdersByStatus.ContainsKey(s))
                    vm.OrdersByStatus[s] = 0;

            // Top selling item
            var topItem = await _orderService.GetTopSellingItemAsync();
            if (topItem != null)
            {
                vm.TopItemName = topItem.Value.name;
                vm.TopItemImageUrl = topItem.Value.imageUrl;
                vm.TopItemQuantity = topItem.Value.qty;
            }

            var picks = await _mealRecService.GetChefPicksAsync(3);

            ViewBag.ChefPicks = picks;

            return View(vm);
        }
    }
}
