
using DemoMvcApp.command;
using DemoMvcApp.Models;
using DemoMvcApp.Repositories;

namespace DemoMvcApp.Handler
{
    public class CreateProductCommandHandler
    {
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void Handle(CreateProductCommand command)
        {
            var product = new Product
            {
                Name = command.Name,
                Price = command.Price,
                ProductImage = command.ProductImage,
                ImagePath = command.ImagePath


            };

            _productRepository.AddProduct(product);  // Assuming AddProduct is a synchronous method
        }
    }
}
