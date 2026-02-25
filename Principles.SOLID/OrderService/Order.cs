using System;
using System.Collections.Generic;
using System.Text;

namespace OrderService
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string CustomerType { get; set; }
        public decimal Total { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
    public class OrderItem
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
