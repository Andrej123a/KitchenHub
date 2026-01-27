using Restaurant.Domain.Entities;
using Restaurant.Repository.Repositories;

namespace Restaurant.Service.Services
{
    // Бизнис логика за MenuItem (засега само рутирање кон Repository)
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItemService(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public async Task<List<MenuItem>> GetAllAsync()
            => await _menuItemRepository.GetAllAsync();

        public async Task<MenuItem?> GetByIdAsync(Guid id)
            => await _menuItemRepository.GetByIdAsync(id);

        public async Task CreateAsync(MenuItem menuItem)
        {
            if (menuItem.Id == Guid.Empty)
                menuItem.Id = Guid.NewGuid();

            await _menuItemRepository.AddAsync(menuItem);
        }

        public async Task UpdateAsync(MenuItem menuItem)
            => await _menuItemRepository.UpdateAsync(menuItem);

        public async Task DeleteAsync(Guid id)
            => await _menuItemRepository.DeleteAsync(id);
    }
}
