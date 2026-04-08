using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Ensure the user is logged in for any action in this controller
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        [Authorize(Policy = "CanView")]
        public IActionResult GetAll()
        {
            return Ok(_productService.GetProductsForDisplay());
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CanView")]
        public IActionResult Get(int id)
        {
            var product = _productService.GetSingleProduct(id);
            return product == null ? NotFound() : Ok(product);
        }

        // POST: api/products
        [HttpPost]
        [Authorize(Policy = "CanAdd")] 
        public IActionResult Create([FromBody] Product product)
        {
            _productService.CreateProduct(product);
            return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        [Authorize(Policy = "CanEdit")] 
        public IActionResult Update(int id, [FromBody] Product product)
        {
            if (id == 0) return BadRequest("ID mismatch");

            _productService.UpdateProduct(product);
            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDelete")] 
        public IActionResult Delete(int id)
        {
            _productService.RemoveProduct(id);
            return Ok(new { message = "Product deleted" });
        }
    }
}