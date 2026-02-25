using OrderService;

namespace OrderServiceBadTest
{
    public class OrderServiceBadTests : IDisposable
    {
        private const string OrdersFilePath = "orders.json";
        private const string LogFilePath = "log.txt";

        public void Dispose()
        {
            if (File.Exists(OrdersFilePath))
                File.Delete(OrdersFilePath);

            if (File.Exists(LogFilePath))
                File.Delete(LogFilePath);
        }

        private static Order CreateValidOrder(string customerType = "Regular", List<OrderItem>? items = null)
        {
            return new Order
            {
                CustomerName = "Test User",
                Email = "test@example.com",
                CustomerType = customerType,
                Items = items ?? [new OrderItem { ProductName = "Item1", Price = 100m, Quantity = 2 }]
            };
        }

        // --- Validación ---

        [Fact]
        public void Constructor_OrderWithNullItems_ThrowsException()
        {
            var order = new Order
            {
                CustomerName = "Test",
                Email = "test@example.com",
                CustomerType = "Regular",
                Items = null!
            };

            var ex = Assert.Throws<Exception>(() => new OrderServiceBad(order));
            Assert.Equal("Order must contain at least one item", ex.Message);
        }

        [Fact]
        public void Constructor_OrderWithEmptyItems_ThrowsException()
        {
            var order = new Order
            {
                CustomerName = "Test",
                Email = "test@example.com",
                CustomerType = "Regular",
                Items = []
            };

            var ex = Assert.Throws<Exception>(() => new OrderServiceBad(order));
            Assert.Equal("Order must contain at least one item", ex.Message);
        }

        // --- Cálculo de total ---

        [Fact]
        public void Constructor_SingleItem_CalculatesTotalCorrectly()
        {
            var order = CreateValidOrder("Other", [
                new OrderItem { ProductName = "A", Price = 50m, Quantity = 3 }
            ]);

            _ = new OrderServiceBad(order);

            Assert.Equal(150m, order.Total);
        }

        [Fact]
        public void Constructor_MultipleItems_CalculatesTotalCorrectly()
        {
            var order = CreateValidOrder("Other", [
                new OrderItem { ProductName = "A", Price = 10m, Quantity = 2 },
                new OrderItem { ProductName = "B", Price = 25m, Quantity = 4 }
            ]);

            _ = new OrderServiceBad(order);

            Assert.Equal(120m, order.Total); // (10*2) + (25*4) = 20 + 100 = 120
        }

        // --- Descuentos ---

        [Fact]
        public void Constructor_PremiumCustomer_Applies20PercentDiscount()
        {
            var order = CreateValidOrder("Premium", [
                new OrderItem { ProductName = "A", Price = 100m, Quantity = 1 }
            ]);

            _ = new OrderServiceBad(order);

            Assert.Equal(80m, order.Total); // 100 * 0.8
        }

        [Fact]
        public void Constructor_RegularCustomer_Applies10PercentDiscount()
        {
            var order = CreateValidOrder("Regular", [
                new OrderItem { ProductName = "A", Price = 100m, Quantity = 1 }
            ]);

            _ = new OrderServiceBad(order);

            Assert.Equal(90m, order.Total); // 100 * 0.9
        }

        [Fact]
        public void Constructor_OtherCustomerType_NoDiscount()
        {
            var order = CreateValidOrder("Other", [
                new OrderItem { ProductName = "A", Price = 100m, Quantity = 1 }
            ]);

            _ = new OrderServiceBad(order);

            Assert.Equal(100m, order.Total);
        }

        // --- Persistencia en archivo JSON ---

        [Fact]
        public void Constructor_ValidOrder_CreatesOrdersJsonFile()
        {
            var order = CreateValidOrder();

            _ = new OrderServiceBad(order);

            Assert.True(File.Exists(OrdersFilePath));
        }

        [Fact]
        public void Constructor_MultipleOrders_AppendsToOrdersJsonFile()
        {
            var order1 = CreateValidOrder();
            var order2 = CreateValidOrder("Premium");

            _ = new OrderServiceBad(order1);
            _ = new OrderServiceBad(order2);

            var json = File.ReadAllText(OrdersFilePath);
            var orders = System.Text.Json.JsonSerializer.Deserialize<List<Order>>(json);

            Assert.NotNull(orders);
            Assert.Equal(2, orders.Count);
        }

        // --- Log ---

        [Fact]
        public void Constructor_ValidOrder_CreatesLogFile()
        {
            var order = CreateValidOrder();

            _ = new OrderServiceBad(order);

            Assert.True(File.Exists(LogFilePath));
            var logContent = File.ReadAllText(LogFilePath);
            Assert.Contains("Order processed at", logContent);
        }
    }
}
