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

        // ✅ NEW: External API service (TheMealDB) for “Chef’s Picks”
        // Idea: we fetch meals from an external API, then we show only 3 curated picks on the dashboard
        // (this is the "transformed" presentation, not raw API JSON).
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

            // ✅ NEW: save injected external API service
            _mealRecService = mealRecService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            var menuItems = await _menuItemService.GetAllAsync();
            var orders = await _orderService.GetAllAsync();

            // Total revenue = sum of (sum of line totals per order)
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

            // Make sure all statuses exist in dictionary (so UI won't crash)
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

            // ✅ NEW: External API integration (TheMealDB)
            // We fetch 3 "Chef’s Picks" from an external API and show them on the dashboard as recommendations.
            // This fulfills the requirement: external API -> data -> transformed UI output.
            var picks = await _mealRecService.GetChefPicksAsync(3);

            // We pass them to the view using ViewBag (quick and safe; no DB/model changes needed).
            ViewBag.ChefPicks = picks;

            return View(vm);
        }
    }
}
