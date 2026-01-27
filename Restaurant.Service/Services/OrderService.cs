using Restaurant.Domain.Entities;
using Restaurant.Repository.Repositories;
using System.Linq;

namespace Restaurant.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;

        public OrderService(IOrderRepository repo)
        {
            _repo = repo;
        }
        public decimal CalculateTotal(Order order)
        {
            if (order.Items == null) return 0m;
            return order.Items.Sum(i => i.UnitPrice * i.Quantity);
        }
        public async Task<(string name, string? imageUrl, int qty)?> GetTopSellingItemAsync()
        {
            var orders = await _repo.GetAllAsync();

            var top = orders
                .SelectMany(o => o.Items ?? new List<OrderItem>())
                .Where(i => i.MenuItem != null)                 // safety
                .GroupBy(i => i.MenuItem!.Id)                   // group by MenuItem
                .Select(g => new
                {
                    Item = g.First().MenuItem!,
                    Qty = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.Qty)
                .FirstOrDefault();

            if (top == null) return null;

            return (top.Item.Name, top.Item.ImageUrl, top.Qty);
        }


        public Task<List<Order>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Order?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task CreateAsync(Order order) => _repo.AddAsync(order);
        public Task UpdateAsync(Order order) => _repo.UpdateAsync(order);
        public Task DeleteAsync(Guid id) => _repo.DeleteAsync(id);
    }


}
