using Restaurant.Domain.Entities;

namespace Restaurant.Service.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(Guid id);
        Task CreateAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Guid id);
        Task<(string name, string? imageUrl, int qty)?> GetTopSellingItemAsync();

    }
}
