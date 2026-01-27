using System;

namespace Restaurant.Domain.Entities
{
    public class MenuItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;

        // FK мора да биде Guid, затоа што Category.Id е Guid
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? ImageUrl { get; set; }   

    }
}
