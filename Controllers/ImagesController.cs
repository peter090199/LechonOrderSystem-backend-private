using BackendNETAPI.Model;
using Microsoft.AspNetCore.Mvc;


namespace BackendNETAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ImagesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult GetImages()
        {
            // Replace with your logic to get image data
            var images = new List<ImageModel>
        {
            new ImageModel { Url = "/uploads/" },
     
        };

            return Ok(images);
        }

        [HttpGet("{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            // Construct the path to the image
            var filePath = Path.Combine(_env.ContentRootPath, "wwwroot/uploads", fileName);

            // Log the path for debugging purposes
            Console.WriteLine($"Looking for file at: {filePath}");

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(); // Return 404 if the file does not exist
            }

            // Read and return the file
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "image/jpeg"); // Adjust the content type as necessary
        }

        //public IActionResult GetImage(string fileName)
        //{
        //    // Construct the path to the image
        //    var filePath = Path.Combine(_env.ContentRootPath, "wwwroot/uploads", fileName);

        //    // Log the path for debugging purposes
        //    Console.WriteLine($"Looking for file at: {filePath}");

        //    // Check if the file exists
        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        return NotFound(); // Return 404 if the file does not exist
        //    }

        //    // Get the file extension to determine the content type
        //    var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
        //    string contentType;

        //    switch (fileExtension)
        //    {
        //        case ".jpeg":
        //            contentType = "image/jpeg";
        //            break;
        //        case ".png":
        //            contentType = "image/png";
        //            break;
        //        // Add other image formats as needed
        //        default:
        //            return NotFound(); // Return 404 if the file type is unsupported
        //    }

        //    // Read and return the file
        //    var fileBytes = System.IO.File.ReadAllBytes(filePath);
        //    return File(fileBytes, contentType); // Use the determined content type
        //}




        // POST api/<ImagesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ImagesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ImagesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
