﻿namespace FronyToBack.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public string SKU { get; set; }

        public string Imageurl { get; set; }

        public int? CategoryId { get; set; }
        public Category ?Category { get; set; }

        public List<ProductImage> Images { get; set; }


    }
}
