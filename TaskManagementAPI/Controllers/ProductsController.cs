using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private static List<Product> products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99, Stock = 10 },
            new Product { Id = 2, Name = "Smartphone", Price = 499.99, Stock = 25 },
            new Product { Id = 3, Name = "Tablet", Price = 299.99, Stock = 15 }
        };

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = products.Find(p => p.Id == id);

            if(product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public IActionResult CreateProduct(CreateProductDto createProduct)
        {
            var newId = products.Any() ? products.Max(p => p.Id) : 0;
            newId++;

            var newProduct = new Product
            {
                Id = newId,
                Name = createProduct.Name,
                Price = createProduct.Price,
                Stock = createProduct.Stock,
            };

            products.Add(newProduct);

            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, CreateProductDto updateProduct)
        {
            var product = products.Find(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            product.Name = updateProduct.Name ?? product.Name;
            product.Price = updateProduct.Price ?? product.Price;
            product.Stock = updateProduct.Stock ?? product.Stock;

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = products.Find(t => t.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            products.Remove(product);

            return NoContent();
        }
    }
}
