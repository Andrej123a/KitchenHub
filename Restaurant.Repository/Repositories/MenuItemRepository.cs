using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Repository.Data;

namespace Restaurant.Repository.Repositories
{
    // Реална имплементација со EF Core
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly AppDbContext _context;

        public MenuItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetAllAsync()
        {
            // Include(Category) за да се вчитува и категоријата при листање
            return await _context.MenuItems
                .Include(m => m.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MenuItem?> GetByIdAsync(Guid id)
        {
            return await _context.MenuItems
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MenuItem menuItem)
        {
            _context.MenuItems.Update(menuItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return;

            _context.MenuItems.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
