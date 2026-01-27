using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Repository.Data;

namespace Restaurant.Repository.Repositories
{
    // Реална имплементација на ICategoryRepository со EF Core (AppDbContext)
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        // DbContext доаѓа преку DI
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            // AsNoTracking = побрзо за read-only листи
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        //public async Task AddAsync(Category category)
        //{
        //    _context.Categories.Add(category);
        //    await _context.SaveChangesAsync();
        //}
        public async Task AddAsync(Category category)
        {
            Console.WriteLine(">>> ADDING CATEGORY: " + category.Name + " | ID=" + category.Id);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            Console.WriteLine(">>> SAVED. COUNT=" + await _context.Categories.CountAsync());
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (entity == null) return;

            _context.Categories.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
