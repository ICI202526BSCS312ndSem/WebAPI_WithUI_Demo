using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Authorize(Policy = "CanView")]
        public IActionResult Get()
        {
            return Ok(_productService.GetProductsForDisplay());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var product = _productService.GetSingleProduct(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        [Authorize(Policy = "CanAdd")]
        public IActionResult Post([FromBody] Product product)
        {
            _productService.CreateProduct(product);
            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CanEdit")]
        public IActionResult Put(int id, [FromBody] Product product)
        {
            product.Id = id;
            _productService.UpdateProduct(product);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDelete")]
        public IActionResult Delete(int id)
        {
            _productService.RemoveProduct(id);
            return Ok();
        }
    }
}