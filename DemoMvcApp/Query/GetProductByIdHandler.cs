using DemoMvcApp.Models;
using DemoMvcApp.Repositories;
using MediatR;

namespace DemoMvcApp.Query
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return _productRepository.GetProductById(request.Id);
        }
    }

}
