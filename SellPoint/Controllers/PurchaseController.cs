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

       
    }
}