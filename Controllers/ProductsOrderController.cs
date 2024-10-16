using BackendNETAPI.Data;
using BackendNETAPI.Model;
using BackendNETAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsOrderController : ControllerBase
    {
        private readonly MyDbContext _context;
        public ProductsOrderController(MyDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct([FromBody] ProductsOrder product)
        {
            if (product == null)
            {
                return BadRequest("Product data is invalid");
            }

            _context.ProductsOrder.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpGet("GetProductsOrder")]
        public async Task<ActionResult<IEnumerable<ProductsOrder>>> GetProductsOrder()
        {
            return await _context.ProductsOrder.ToListAsync();
        }

        [HttpGet("GetProductsOrderById/{userId}")]
        public async Task<ActionResult<ProductsOrder>> UserOrderByID(int userId)
        {
            var userOrders = _context.ProductsOrder
                                       .Where(x => x.UserId == userId)
                                       .ToList();
            if (userOrders == null)
            {
                return NotFound();
            }
            return Ok(userOrders); 
        }

        [HttpGet("GetCountsOrderById/{userId}")]
        public async Task<ActionResult<ProductsOrder>> GetCountsOrderById(int userId)
        {
            var userOrders = _context.ProductsOrder
                .Where(x => x.UserId == userId).Count();
                                       
            return Ok(userOrders);
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteEmployee(int orderId)
        {
            var order = await _context.ProductsOrder.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }
            _context.ProductsOrder.Remove(order);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Employee with ID {orderId} deleted successfully." });
        }
        [HttpGet("GetTotalAmountByUser/{userId}")]
        public async Task<ActionResult<decimal>> GetTotalAmountByUser(int userId)
        {
            // Calculate the total amount of orders for the specified user
            var totalAmount = await _context.ProductsOrder
                                             .Where(x => x.UserId == userId)
                                             .SumAsync(x => x.TotalAmount);

            return Ok(totalAmount);
        }

    }
}
