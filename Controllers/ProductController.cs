using System.Threading.Tasks;
using backendProject.Data.SqlDbContext;
using backendProject.Models.DomainModels;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backendProject.Controllers.ProductController
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly SqlDbContext dbContext;
        public ProductController(SqlDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpPost("createproduct")]
        public async Task<IActionResult> CreateProduct(Product req)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "All Details Are Required !" });
                }
                await dbContext.Products.AddAsync(req);
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Product Added Sucessfully !", payload = req });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
        [HttpGet("archiveproduct")]
        public async Task<IActionResult> ArchiveProduct(Guid productId)
        {
            try
            {
                var product = await dbContext.Products.FindAsync(productId);
                if (product == null)
                {
                    return BadRequest(new { message = "Product Not Found !" });
                }
                if (product.IsArchived == false && product.IsAvailable == true)
                {
                    product.IsArchived = true;
                    product.IsAvailable = false;
                }
                else
                {
                    return BadRequest(new { message = "Product is already in the archived list !" });
                }
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Product Archived Sucessfully !" });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
        [HttpGet("unarchiveproduct")]
        public async Task<IActionResult> UnarchiveProduct(Guid productId)
        {
            try
            {
                var product = await dbContext.Products.FindAsync(productId);
                if (product == null)
                {
                    return BadRequest(new { message = "Product Not Found !" });
                }
                if (product.IsArchived == true && product.IsAvailable == false)
                {
                    product.IsArchived = false;
                    product.IsAvailable = true;
                }
                else
                {
                    return BadRequest(new { message = "Product is already in the unarchived list !" });
                }
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Product UnArchived Sucessfully !" });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
        [HttpPut("updateproduct")]
        public async Task<IActionResult> UpdateProduct(Guid productId, Product req)
        {
            try
            {
                var existingproduct = await dbContext.Products.FindAsync(productId);
                if (existingproduct == null)
                {
                    return BadRequest(new { message = "Product Not Found !" });
                }
                existingproduct.ProductName = req.ProductName;
                existingproduct.ProductDescription = req.ProductDescription;
                existingproduct.ProductImage = req.ProductImage;
                existingproduct.ProductStock = req.ProductStock;
                existingproduct.ProductPrice = req.ProductPrice;
                existingproduct.Size = req.Size;
                existingproduct.Color = req.Color;
                existingproduct.Weight = req.Weight;
                existingproduct.Category = req.Category;
                existingproduct.UpdatedAt = DateTime.Now;

                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Product Updated Sucessfully !", payload = existingproduct });

            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
        [HttpGet("getallproducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await dbContext.Products.Where(p => p.IsAvailable == true).ToListAsync();
                return Ok(new { message = $"{products.Count} Products Found !", payload = products });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
        [HttpGet("getproductbyid")]
        public async Task<IActionResult> GetProductById(Guid productId)
        {
            try
            {
                var product = await dbContext.Products.FindAsync(productId);
                if (product == null)
                {
                    return BadRequest(new { message = "Product Not Found !" });
                }
                return Ok(new { message = "The Product Details Are :", payload = product });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }

    }
}