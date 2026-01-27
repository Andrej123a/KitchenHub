using System;
using System.Collections.Generic;
using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities
{
    // Order претставува една нарачка направена од корисник
    public class Order
    {
        public Guid Id { get; set; }

        // Податоци за клиентот
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string DeliveryAddress { get; set; }

        // Датум на креирање
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Статус на нарачката
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Ставки во нарачката (една нарачка има повеќе јадења)
        public ICollection<OrderItem> Items { get; set; }
    }
}
