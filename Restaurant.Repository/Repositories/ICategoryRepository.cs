using Restaurant.Domain.Entities;

namespace Restaurant.Repository.Repositories
{
    // Интерфејс (договор) за работа со Category во база
    // Service ќе го користи овој договор, без да знае за EF Core.
    public interface ICategoryRepository
    {
        // Читање
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(Guid id);

        // Запишување
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Guid id);
    }
}
