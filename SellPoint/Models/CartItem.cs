﻿namespace SellPoint.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public User Customer { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
    }

}
