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
        public async Task<ActionResult<List<ProductsOrder>>> UserOrderByID(int userId)
        {
            // Fetch user orders asynchronously
            var userOrders = await _context.ProductsOrder
                                           .Where(x => x.UserId == userId)
                                           .ToListAsync();

            // Check if the user has any orders
            if (userOrders == null || userOrders.Count == 0)
            {
                return NotFound(new { message = "No orders found for this user." });
            }

            // Return the orders
            return Ok(userOrders);
        }

        [HttpGet("GetCountsOrderById/{userId}")]
        public async Task<ActionResult<int>> GetCountsOrderById(int userId)
        {
            // Count the number of orders asynchronously
            var orderCount = await _context.ProductsOrder
                                            .Where(x => x.UserId == userId)
                                            .CountAsync();

            // Return the count of orders
            return Ok(orderCount);
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
