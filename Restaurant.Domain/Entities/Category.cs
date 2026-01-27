using System.Collections.Generic;
using System;


namespace Restaurant.Domain.Entities
{
    // Category претставува група на јадења (пример: Пици, Паста, Десерти)
    public class Category
    {
        // Primary Key - уникатен идентификатор
        public Guid Id { get; set; }

        // Име на категоријата (задолжително)
        public string Name { get; set; }

        // Опис на категоријата (опционално)
        public string? Description { get; set; }

        // Дали категоријата е активна (може да се прикажува во мени)
        public bool IsActive { get; set; } = true;

        // Навигациско својство:
        // Една категорија може да има повеќе јадења
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public string? ImageUrl { get; set; }

    }
}
