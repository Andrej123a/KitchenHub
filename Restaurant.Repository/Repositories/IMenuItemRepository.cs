using Restaurant.Domain.Entities;

namespace Restaurant.Repository.Repositories
{
    // Договор за работа со MenuItem во база
    public interface IMenuItemRepository
    {
        Task<List<MenuItem>> GetAllAsync();
        Task<MenuItem?> GetByIdAsync(Guid id);

        Task AddAsync(MenuItem menuItem);
        Task UpdateAsync(MenuItem menuItem);
        Task DeleteAsync(Guid id);
    }
}
