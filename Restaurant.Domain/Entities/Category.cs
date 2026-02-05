using System.Collections.Generic;
using System;


namespace Restaurant.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public string? ImageUrl { get; set; }

    }
}
