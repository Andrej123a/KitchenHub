using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;
using Restaurant.Service.Services;
using Restaurant.Web.Models;

namespace Restaurant.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMenuItemService _menuItemService;

        public OrdersController(IOrderService orderService, IMenuItemService menuItemService)
        {
            _orderService = orderService;
            _menuItemService = menuItemService;
        }

        // GET: /Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        // GET: /Orders/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        // GET: /Orders/Create
        public async Task<IActionResult> Create()
        {
            await LoadMenuItems();
            var vm = new OrderCreateVm();
            vm.Items.Add(new OrderItemCreateVm()); // старт со 1 ред
            return View(vm);
        }

        // POST: /Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateVm vm)
        {
            // исфрли празни редови
            vm.Items = (vm.Items ?? new List<OrderItemCreateVm>())
                .Where(i => i.MenuItemId != Guid.Empty && i.Quantity > 0)
                .ToList();

            if (vm.Items.Count == 0)
                ModelState.AddModelError("", "Мора да додадеш барем една ставка (јадење + количина).");

            if (!ModelState.IsValid)
            {
                await LoadMenuItems();
                return View(vm);
            }

            // земи мени од база за да ја земеме цената server-side
            var menuItems = await _menuItemService.GetAllAsync();
            var menuDict = menuItems.ToDictionary(m => m.Id, m => m);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = vm.CustomerName,
                Phone = vm.Phone,
                DeliveryAddress = vm.DeliveryAddress,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Items = new List<OrderItem>()
            };

            foreach (var i in vm.Items)
            {
                if (!menuDict.TryGetValue(i.MenuItemId, out var mi))
                {
                    ModelState.AddModelError("", "Избрано е непостоечко јадење.");
                    await LoadMenuItems();
                    return View(vm);
                }

                order.Items.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = mi.Id,
                    Quantity = i.Quantity,
                    UnitPrice = mi.Price
                });
            }

            await _orderService.CreateAsync(order);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Orders/EditStatus/{id}
        public async Task<IActionResult> EditStatus(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            var vm = new OrderStatusUpdateVm
            {
                Id = order.Id,
                Status = order.Status
            };

            return View(vm);
        }

        // POST: /Orders/EditStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(OrderStatusUpdateVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var order = await _orderService.GetByIdAsync(vm.Id);
            if (order == null)
                return NotFound();

            // веќе е завршена/откажана - не дозволуваме промена
            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
                return RedirectToAction(nameof(Details), new { id = vm.Id });

            order.Status = vm.Status;
            await _orderService.UpdateAsync(order);

            return RedirectToAction(nameof(Details), new { id = vm.Id });
        }


        // GET: /Orders/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            // 🚫 Не дозволуваме бришење на доставена нарачка
            if (order.Status == OrderStatus.Delivered)
                return RedirectToAction(nameof(Details), new { id });

            return View(order);
        }


        // POST: /Orders/Delete/{id}
        // POST: /Orders/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return RedirectToAction(nameof(Index));

            // 🚫 Забрана за бришење ако е доставена
            if (order.Status == OrderStatus.Delivered)
                return RedirectToAction(nameof(Details), new { id });

            await _orderService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


        private async Task LoadMenuItems()
        {
            var items = await _menuItemService.GetAllAsync();
            items = items.Where(i => i.IsAvailable).ToList();

            ViewBag.MenuItems = new SelectList(items, "Id", "Name");
        }
    }
}
