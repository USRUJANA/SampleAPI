using SampleAPI.Entities;


namespace SampleAPI.Tests.Data
{
    public static class TestDataSeed
    {
        public static void Seed(SampleApiDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Orders.Any())
            {
                return;
            }

            var orders = new List<Order>
            {
                new() { Id = 1, Name = "Order 1", Description = "First Order", EntryDate = DateTime.UtcNow.AddDays(-2)},
                new() { Id = 2, Name = "Order 2", Description = "Second Order", EntryDate = DateTime.UtcNow}
            };

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }
    }
}
