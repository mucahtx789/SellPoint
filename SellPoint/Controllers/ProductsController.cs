using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SellPoint.Hubs;
using SellPoint.Models;
using System.Security.Claims;

namespace SellPoint.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IHubContext<ProductHub> _productHub;

        public ProductsController(ApplicationDbContext context, IHubContext<ProductHub> productHub)
        {
            _context = context;
            _productHub = productHub;
        }


    
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
           

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            await _productHub.Clients.All.SendAsync("ProductUpdated");

            return Ok(product);
        }

        // ürün detay sayfasına o ürünü getirme
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "Ürün bulunamadı." });

            return Ok(product);
        }

        // müşterilere tüm ürünleri getirme
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync(); // Tüm ürünleri al
            return Ok(products);
        }
        


        //satıcıya göre ürün getirme
        [HttpGet("my")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetMyProducts()
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var products = await _context.Products
                .Where(p => p.SellerId == sellerId)
                .ToListAsync();
            return Ok(products);
        }

        //ürün silme
        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
           
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId);
            if (product == null)
                return NotFound(new { message = "product not found." });

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            await _productHub.Clients.All.SendAsync("ProductUpdated");
            return NoContent();
        }

        //ürün güncelleme
        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.SellerId == sellerId);
            if (existingProduct == null)
                return NotFound(new { message = "Ürün bulunamadı veya yetkiniz yok." });

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Category = updatedProduct.Category;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Stock = updatedProduct.Stock;
            existingProduct.ImageUrl = updatedProduct.ImageUrl;

            await _context.SaveChangesAsync();
            await _productHub.Clients.All.SendAsync("ProductUpdated");

            return Ok(existingProduct);
        }

    }

}
