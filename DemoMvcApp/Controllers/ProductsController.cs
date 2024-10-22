using Microsoft.AspNetCore.Mvc;
using DemoMvcApp.Services;
using DemoMvcApp.Models;
using System;
using DemoMvcApp.Handler;
using DemoMvcApp.command;
using DemoMvcApp.Mapper;
using DemoMvcApp.Repositories;
using DemoMvcApp.DTOs;

namespace DemoMvcApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IErrorHandlingService<string> _errorHandlingService;
        private readonly CreateProductCommandHandler _commandHandler;
        private readonly IMapper _mapper;//mapper
        private readonly IProductRepository _productRepository;


        // Dependency Injection
        public ProductsController(IProductService productService, IErrorHandlingService<string> errorHandlingService, CreateProductCommandHandler commandHandler, IMapper mapper, IProductRepository productRepository)
        {
            _productService = productService;

            _errorHandlingService = errorHandlingService;
            _commandHandler = commandHandler;
            _productRepository = productRepository;

            _mapper = mapper;

        }

        // GET: api/<ProductsController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var products = _productService.GetAllProducts();
                return Ok(products);
            }
            catch (Exception)
            {
                var errorMessage = _errorHandlingService.GetError();
                return StatusCode(500, errorMessage);
            }
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var product = _productService.GetProductById(id);
                if (product == null)
                {
                    return NotFound("Product not found.");
                }

                return Ok(product);
            }
            catch (Exception)
            {
                var errorMessage = _errorHandlingService.GetError();
                return StatusCode(500, errorMessage);
            }
        }

        // POST api/<ProductsController>
        [HttpPost]
        public IActionResult Post([FromBody] CreateProductCommand command)
        {
            try
            {
                _commandHandler.Handle(command);  // Synchronously handle the command
                return RedirectToAction("Index"); // Redirect to Get action after successful creation
            }
            catch (Exception)
            {
                var errorMessage = _errorHandlingService.GetError();
                return StatusCode(500, errorMessage);
            }
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile(IFormFile productImage, string Name, decimal Price)
        {
            if (productImage != null && productImage.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(productImage.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\ecs\\source\\repos\\DemoMvcApp\\DemoMvcApp\\Image", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await productImage.CopyToAsync(stream);
                }

                // Save product info to the database
                var command = new CreateProductCommand
                {
                    Name = Name,
                    Price = Price,
                    ProductImage = fileName,
                    ImagePath = filePath
                };

                _commandHandler.Handle(command);
                return Ok(new { message = "File uploaded successfully" });
            }
            return BadRequest(new { message = "No file uploaded" });
        }
        
                // PUT api/<ProductsController>/5
                [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product value)
        {
            try
            {
                _productService.UpdateProduct(id, value);
                return NoContent();
            }
            catch (Exception)
            {
                var errorMessage = _errorHandlingService.GetError();
                return StatusCode(500, errorMessage);
            }
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _productService.DeleteProduct(id);
                return NoContent();
            }
            catch (Exception)
            {
                var errorMessage = _errorHandlingService.GetError();
                return StatusCode(500, errorMessage);
            }
        }
        




        // mapper 

        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product =  _productRepository.GetProductById(id);
            if (product == null)
                return NotFound();

            var productDto = _mapper.MapToDto(product);
            return Ok(productDto);
        }

        [HttpPost("mapper")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto productDto)
        {
            var product = _mapper.MapToEntity(productDto);
             _productRepository.AddProduct(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }

    }
}
