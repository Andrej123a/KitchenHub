using System;
using System.Collections.Generic;
using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string DeliveryAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public ICollection<OrderItem> Items { get; set; }
    }
}
