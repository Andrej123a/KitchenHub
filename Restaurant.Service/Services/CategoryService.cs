using Restaurant.Domain.Entities;
using Restaurant.Repository.Repositories;

namespace Restaurant.Service.Services
{
    // CategoryService е бизнис слојот за Category.
    // Овде НЕ работиме директно со база,
    // туку преку Repository (Onion архитектура).
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        // Repository се инјектира преку constructor (Dependency Injection)
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // Ги враќа сите категории
        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        // Враќа една категорија по Id
        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        // Креира нова категорија
        public async Task CreateAsync(Category category)
        {
            // Пример за место каде може да се додаде валидација
            // if (string.IsNullOrWhiteSpace(category.Name)) throw new Exception("Name is required");

            await _categoryRepository.AddAsync(category);
        }

        // Ажурира постоечка категорија
        public async Task UpdateAsync(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
        }

        // Брише категорија по Id
        public async Task DeleteAsync(Guid id)
        {
            await _categoryRepository.DeleteAsync(id);
        }
    }
}
