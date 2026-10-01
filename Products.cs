using System;

namespace TechMartManager
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Category(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class Product
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImagePath { get; set; }

        public Product()
        {
            ProductId = string.Empty;
            ProductName = string.Empty;
            CategoryId = string.Empty;
            CategoryName = string.Empty;
            ImagePath = string.Empty;
        }
    }
}