using BackendNETAPI.Data;
using BackendNETAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly MyDbContext _context;
  

        public ProductsController(MyDbContext context)
        {
            _context = context;
        }
    

        // GET: api/Products
        [HttpGet]   
        public async Task<ActionResult<IEnumerable<Products>>> GetEmployees()
        {
            return await _context.Products.ToListAsync();
        }
       // Products/GetInventoryPendingProducts

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Products>> GetEmployee(int id)
        {
            var employee = await _context.Products.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee); // Wrap the employee object in an Ok() result to ensure it returns as JSON
        }

        [HttpGet("GetProductByImageName/{filename}")]
        public async Task<ActionResult<Products>> GetUserByUsername(string filename)
        {
            // Find the user by their username
            var pr = await _context.Products
                                     .FirstOrDefaultAsync(u => u.ProductName == filename);
            if (pr == null)
            {
                return NotFound($"Product with username '{filename}' not found.");
            }
            return Ok(pr);
        }


        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] Products product) // Accepts JSON input
        {
            if (id != product.Id)
            {
                return BadRequest("Product ID mismatch.");
            }
            // Check if the employee exists in the database
            var pd = await _context.Products.FindAsync(id);
            if (pd == null)
            {
                return NotFound("Product not found.");
            }
            _context.Entry(pd).CurrentValues.SetValues(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExist(id))
                {
                    return NotFound("Product not found.");
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Products successfully updated." });
        }
        private bool ProductExist(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // POST: api/Products
        [HttpPost("SavedEmployees")]
        public async Task<ActionResult<Products>> PostEmployee([FromBody] Products products)
        {
            if (products == null)
            {
                return BadRequest("Invalid employee data.");
            }

            try
            {
                products.InventoryStatus = "pending";
                 _context.Products.Add(products);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Successfully Saved." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while saving the employee." + ex.Message);
            }

        }


        [HttpDelete("{empID}")]
        public async Task<IActionResult> DeleteEmployee(string empID)
        {
            var employee = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == empID);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found." });
            }
            _context.Products.Remove(employee);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Employee with ID {empID} deleted successfully." });
        }

        // POST: api/Products/SavedEmployees
        //[HttpPost("SavedProducts")]
        //public async Task<IActionResult> SaveEmployee([FromForm] Products products, [FromForm] IFormFile image)
        //{
        //    if (image != null && image.Length > 0)
        //    {
        //        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //        if (!Directory.Exists(uploadDir))
        //            Directory.CreateDirectory(uploadDir);

        //        // Save image file to server
        //        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        //        var filePath = Path.Combine(uploadDir, fileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await image.CopyToAsync(stream);
        //        }

        //        var data = new Products
        //        {
        //            ProductId = products.ProductId,
        //            ProductName = products.ProductName,
        //            Category = products.Category,
        //            Price = products.Price,
        //            ImagePath = fileName,
        //            AlertQty = products.AlertQty,
        //            Quantity = products.Quantity,
        //            InventoryStatus = "pending",
        //        };
        //        _context.Products.Add(data);
        //        await _context.SaveChangesAsync();

        //        return Ok(new { Message = "Product saved successfully" });

        //    }

        //    return BadRequest("Image upload failed.");
        //}

        // POST: api/Products/SaveProduct
        [HttpPost("SaveProducts")]
        public async Task<IActionResult> SaveProduct([FromForm] Products products, [FromForm] IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                // Dynamically get the upload directory (could be from appsettings.json or environment-specific settings)
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir); // Create directory if it doesn't exist

                // Generate a unique file name for the uploaded image
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadDir, fileName);

                // Save the image file to the server
                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }

                // Construct the public URL for the uploaded image
                var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

                // Create a new product object with the image path
                var data = new Products
                {
                    ProductId = products.ProductId,
                    ProductName = products.ProductName,
                    Category = products.Category,
                    Price = products.Price,
                    ImagePath = imageUrl, // Store the image URL
                    AlertQty = products.AlertQty,
                    Quantity = products.Quantity,
                    InventoryStatus = "pending"
                };

                // Save product data to the database
                _context.Products.Add(data);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Product saved successfully", ImageUrl = imageUrl });
            }

            return BadRequest("No image uploaded or image upload failed.");
        }


        [HttpPost("update-inventory-status/{id}")]
        public IActionResult UpdateInventoryStatus(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id is required and must be greater than 0.");
            }

            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    return NotFound(new { message = "Product not found." });
                }
                 product.InventoryStatus = "ongoing";
                _context.SaveChanges();
                return Ok(new { message = "Inventory status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the inventory status.", error = ex.Message });
            }
        }


        [HttpGet("GetProductByOngoing")]
        public async Task<IActionResult> GetProductByOngoing()
        {
            try
            {
                var ongoingProducts = await _context.Products
                    .Where(x => x.InventoryStatus == "ongoing") 
                    .ToListAsync(); 

                if (ongoingProducts == null || !ongoingProducts.Any())
                {
                    return NotFound(new { message = "No ongoing products found." });
                }

                return Ok(ongoingProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching ongoing products.", error = ex.Message });
            }
        }
        //

        [HttpGet("GetInventoryPendingProducts")]
        public async Task<IActionResult> GetInventoryPendingProducts()
        {
            try
            {
                var ongoingProducts = await _context.Products
                    .Where(x => x.InventoryStatus == "pending")
                    .ToListAsync();

                if (ongoingProducts == null)
                {
                    return NotFound(new { message = "No pending products found." });
                }

                return Ok(ongoingProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching ongoing products.", error = ex.Message });
            }
        }


    }
}