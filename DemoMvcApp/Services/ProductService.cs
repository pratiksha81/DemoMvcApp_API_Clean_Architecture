using DemoMvcApp.Data;
using DemoMvcApp.Models;
using DemoMvcApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DemoMvcApp.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Find(id);
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            Save();
        }

        public void UpdateProduct(int id, Product product)
        {
            var existingProduct = _context.Products.Find(id);

            if (existingProduct != null) // Ensure the product exists
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                Save();
            }
            else
            {
                throw new Exception("Product not found.");
            }
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                Save();
            }
        }

        // Save changes to the database
        private void Save()
        {
            _context.SaveChanges();
        }
    }
}
