using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Restaurant.Domain.Entities;
using Restaurant.Service.Services;

namespace Restaurant.Web.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly IMenuItemService _menuItemService;
        private readonly ICategoryService _categoryService;

        // ✅ External API (TheMealDB) - used for Dashboard "Use" -> auto-fill Create form
        private readonly IMealRecommendationService _mealRecService;

        public MenuItemsController(
            IMenuItemService menuItemService,
            ICategoryService categoryService,
            IMealRecommendationService mealRecService)
        {
            _menuItemService = menuItemService;
            _categoryService = categoryService;
            _mealRecService = mealRecService;
        }

        // ✅ Filter by category when categoryId is provided
        public async Task<IActionResult> Index(Guid? categoryId)
        {
            var items = await _menuItemService.GetAllAsync();

            if (categoryId.HasValue)
            {
                items = items.Where(x => x.CategoryId == categoryId.Value).ToList();

                var category = await _categoryService.GetByIdAsync(categoryId.Value);
                ViewBag.FilterCategoryName = category?.Name;
                ViewBag.FilterCategoryId = categoryId.Value;
            }
            else
            {
                ViewBag.FilterCategoryName = null;
                ViewBag.FilterCategoryId = null;
            }

            return View(items);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var item = await _menuItemService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // ✅ Create (GET)
        // - categoryId: ако дојдеме од конкретна категорија -> lock category
        // - prefillName: ако дојдеме од Dashboard "Chef’s Picks" -> auto-fill Name/Image/Description from external API
        public async Task<IActionResult> Create(Guid? categoryId, string? prefillName)
        {
            await LoadCategories(categoryId);

            var item = new MenuItem
            {
                IsAvailable = true,
                Name = prefillName ?? string.Empty
            };

            // ✅ NEW (IMPORTANT): Auto-fill from TheMealDB using the picked name
            // This satisfies "external API integration + transformed data".
            if (!string.IsNullOrWhiteSpace(prefillName))
            {
                var details = await _mealRecService.GetMealDetailsByNameAsync(prefillName);
                if (details != null)
                {
                    item.Name = details.Name;
                    item.Description = details.Description ?? string.Empty;
                    item.ImageUrl = details.ImageUrl;
                }
            }

            if (categoryId.HasValue)
            {
                item.CategoryId = categoryId.Value;

                // Keep locked category name for readonly display in Create.cshtml
                if (ViewBag.Categories is SelectList list)
                {
                    var selected = list.FirstOrDefault(x => x.Value == categoryId.Value.ToString());
                    ViewBag.LockedCategoryName = selected?.Text;
                }
            }

            ViewBag.LockCategory = categoryId.HasValue;
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuItem item, Guid? categoryId, string? prefillName)
        {
            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();

            item.Description ??= string.Empty;

            // 🔒 If we came from category page, force the category on POST too
            if (categoryId.HasValue)
                item.CategoryId = categoryId.Value;

            // ✅ Validate category
            if (item.CategoryId == Guid.Empty)
                ModelState.AddModelError(nameof(item.CategoryId), "Category is required.");

            if (!ModelState.IsValid)
            {
                await LoadCategories(item.CategoryId);

                bool lockCategory = categoryId.HasValue || item.CategoryId != Guid.Empty;
                ViewBag.LockCategory = lockCategory;

                if (lockCategory && ViewBag.Categories is SelectList list)
                {
                    var selected = list.FirstOrDefault(x => x.Value == item.CategoryId.ToString());
                    ViewBag.LockedCategoryName = selected?.Text;
                }

                // ✅ Keep prefilled API fields if user got here from Dashboard and validation failed
                // (prevents losing image/description on re-render)
                if (!string.IsNullOrWhiteSpace(prefillName) && string.IsNullOrWhiteSpace(item.ImageUrl))
                {
                    var details = await _mealRecService.GetMealDetailsByNameAsync(prefillName);
                    if (details != null)
                    {
                        item.Name = string.IsNullOrWhiteSpace(item.Name) ? details.Name : item.Name;
                        item.Description = string.IsNullOrWhiteSpace(item.Description) ? (details.Description ?? "") : item.Description;
                        item.ImageUrl = details.ImageUrl ?? item.ImageUrl;
                    }
                }

                return View(item);
            }

            await _menuItemService.CreateAsync(item);
            return RedirectToAction(nameof(Index), new { categoryId = item.CategoryId });
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _menuItemService.GetByIdAsync(id);
            if (item == null) return NotFound();

            await LoadCategories(item.CategoryId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MenuItem item)
        {
            if (id != item.Id) return BadRequest();
            item.Description ??= string.Empty;

            if (item.CategoryId == Guid.Empty)
                ModelState.AddModelError(nameof(item.CategoryId), "Category is required.");

            if (!ModelState.IsValid)
            {
                await LoadCategories(item.CategoryId);
                return View(item);
            }

            await _menuItemService.UpdateAsync(item);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _menuItemService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _menuItemService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategories(Guid? selectedId = null)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedId);
        }
    }
}
