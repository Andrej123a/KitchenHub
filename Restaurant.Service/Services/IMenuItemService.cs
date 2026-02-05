using Restaurant.Domain.Entities;

namespace Restaurant.Service.Services
{
    public interface IMenuItemService
    {
        Task<List<MenuItem>> GetAllAsync();
        Task<MenuItem?> GetByIdAsync(Guid id);

        Task CreateAsync(MenuItem menuItem);
        Task UpdateAsync(MenuItem menuItem);
        Task DeleteAsync(Guid id);
    }
}
