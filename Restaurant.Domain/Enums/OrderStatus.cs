namespace Restaurant.Domain.Enums
{
    // OrderStatus го дефинира статусот на една нарачка
    public enum OrderStatus
    {
        Pending,        // Нарачката е креирана
        Preparing,      // Се подготвува
        OutForDelivery, // Излезена е за достава
        Delivered,      // Доставена
        Cancelled       // Откажана
    }
}
