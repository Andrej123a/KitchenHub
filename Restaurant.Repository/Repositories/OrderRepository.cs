using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Repository.Data;

namespace Restaurant.Repository.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (entity == null) return;

            _context.Orders.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
