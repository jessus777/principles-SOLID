using System.Text.Json;

namespace OrderService
{
    public class OrderServiceBad
    {
        private const string FilePath = "orders.json";
        public OrderServiceBad(Order order)
        {
            // 1. Validación
            if (order.Items == null || order.Items.Count == 0)
                throw new Exception("Order must contain at least one item");

            // 2. Calcular total
            decimal total = 0;
            foreach (var item in order.Items)
            {
                total += item.Price * item.Quantity;
            }

            // 3. Aplicar descuento
            if (order.CustomerType == "Premium")
                total *= 0.8m;
            else if (order.CustomerType == "Regular")
                total *= 0.9m;

            order.Total = total;

            // 4. Guardar en JSON (acoplamiento directo al sistema de archivos)
            List<Order> orders;

            if (File.Exists(FilePath))
            {
                var existingData = File.ReadAllText(FilePath);
                orders = JsonSerializer.Deserialize<List<Order>>(existingData) ?? new List<Order>();
            }
            else
            {
                orders = new List<Order>();
            }

            orders.Add(order);

            var json = JsonSerializer.Serialize(orders, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(FilePath, json);

            // 5. Simular notificación
            Console.WriteLine($"Email sent to {order.Email}");

            // 6. Simular log
            File.AppendAllText("log.txt", $"Order processed at {DateTime.Now}\n");
        }

    }
}
