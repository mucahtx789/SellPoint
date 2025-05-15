using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SellPoint.Models;
using System.Security.Claims;
using SellPoint.Models.Dtos;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

   



   
    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddToCart([FromBody] CartItemDto dto)
    {

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null) return NotFound("No product found.");
        if (product.Stock < dto.Quantity) return BadRequest("insufficient stock.");

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId && c.ProductId == dto.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            _context.CartItems.Add(new CartItem
            {
                CustomerId = dto.CustomerId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });
        }

        product.Stock -= dto.Quantity;

        await _context.SaveChangesAsync();
        return Ok("Ürün sepete eklendi.");
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateCartItem([FromBody] CartItemDto dto)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId && c.ProductId == dto.ProductId);

        if (cartItem == null)
            return NotFound("Cart item not found.");

        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null)
            return NotFound("Product not found.");

        if (dto.Quantity < 1)
            return BadRequest("Quantity must be at least 1.");

        if (dto.Quantity > product.Stock + cartItem.Quantity)
            return BadRequest("Insufficient stock.");

        int quantityDifference = dto.Quantity - cartItem.Quantity;
        product.Stock -= quantityDifference;
        cartItem.Quantity = dto.Quantity;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{customerId}/{productId}")]
    [Authorize]
    public async Task<IActionResult> RemoveFromCart(int customerId, int productId)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId);

        if (cartItem == null) return NotFound("Item not found.");

        var product = await _context.Products.FindAsync(productId);
        if (product != null)
        {
            product.Stock += cartItem.Quantity;
        }

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("{customerId}")]
    [Authorize]
    public async Task<IActionResult> GetCartItems(int customerId)
    {
        var items = await _context.CartItems
            .Where(c => c.CustomerId == customerId)
            .Select(c => new
            {
                c.Product.Name,
                c.Quantity,
                c.Product.Price,
                c.ProductId,
                c.Product.Stock
            })
            .ToListAsync();

        return Ok(items);
    }



}
