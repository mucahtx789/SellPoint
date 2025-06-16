using SellPoint.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SellPoint.Models.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SellPoint.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public PurchaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("GetAddress")]
        public async Task<IActionResult> GetAddress()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FindAsync(int.Parse(userId));
            return Ok(new { address = user?.Address });
        }

        [Authorize]
        [HttpPut("UpdateAddress")]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressUpdateDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FindAsync(int.Parse(userId));
            user.Address = dto.Address;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Adres güncellendi" });
        }

        [Authorize]
        [HttpPut("createOrder")]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            // Kullanıcının sepetindeki ürünleri al
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerId == userId)
                .ToListAsync();


            if (!cartItems.Any())
            {
                return BadRequest(new { message = "Sepet boş." });
            }

            // Yeni bir sipariş 
            var order = new Order
            {
                CustomerId = userId,
                OrderDate = DateTime.Now
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            // Siparişe ait ürünleri ekle
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,//?? birden fazla üründe fiyat ne o
                    SellerId = item.Product.SellerId
                };
                _context.OrderItems.Add(orderItem);
            }



            // Sepeti temizle
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Sipariş başarıyla oluşturuldu." });
        }

        [Authorize]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetSellerOrders()
        {
            var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var orders = await _context.Orders
                .Include(o => o.OrderItems.Where(oi => oi.SellerId == sellerId))
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderItems.Any(oi => oi.SellerId == sellerId))
                .ToListAsync();

            var result = orders.Select(o => new {
                o.Id,
                o.OrderDate,
                Items = o.OrderItems.Select(oi => new {
                    oi.ProductId,
                    ProductName = oi.Product.Name,
                    oi.Quantity,
                    oi.UnitPrice
                })
            });

            return Ok(result);
        }

    }
}