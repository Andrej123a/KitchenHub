using Restaurant.Domain.Entities;

namespace Restaurant.Service.Services
{
    // Интерфејс = договор
    // Овде дефинираме ШТО може да се прави со Category
    // (НЕ како – тоа е работа на имплементацијата)
    public interface ICategoryService
    {
        // Ги враќа сите категории
        Task<List<Category>> GetAllAsync();

        // Враќа една категорија по Id
        Task<Category?> GetByIdAsync(Guid id);

        // Креира нова категорија
        Task CreateAsync(Category category);

        // Ажурира постоечка категорија
        Task UpdateAsync(Category category);

        // Брише категорија
        Task DeleteAsync(Guid id);
    }
}
