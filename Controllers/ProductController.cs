using System.Threading.Tasks;
using backendProject.Data.SqlDbContext;
using backendProject.Middlewares;
using backendProject.Models.DomainModels;
using backendProject.Models.JunctionModels;
using backendProject.Types.Enums;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P0_ClassLibrary;
using P0_ClassLibrary.Interfaces;

namespace backendProject.Controllers.ProductController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly SqlDbContext dbContext;
        public ProductController(SqlDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpPost("createproduct")]//admin
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
        [HttpGet("archiveproduct")]//admin
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
        [HttpGet("unarchiveproduct")]//admin
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
        [HttpPut("updateproduct")]//admin
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
        [HttpGet("getallproducts")]//not logged in user
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
        [HttpGet("getproductbyid")]//admin
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



        //--------------------------------------------------by myself------------------------------------------------------//
        [HttpGet("getproductsbycategory")]//not logged in user
        public async Task<ActionResult> GetProductsByCategory([FromQuery] string category)
        {
            if (!Enum.TryParse(typeof(ProductCategory), category, true, out var parsedCategory))
            {
                return BadRequest(new { message = "Invalid category value." });
            }

            var typedCategory = (ProductCategory)parsedCategory;

            try
            {
                List<Product> products;
                if (typedCategory == ProductCategory.All)
                {
                    products = await dbContext.Products.Where(p => p.IsAvailable).ToListAsync();
                }
                else
                {
                    products = await dbContext.Products.Where(p => p.IsAvailable && p.Category == typedCategory).ToListAsync();
                }
                return Ok(new { message = $"{products.Count} Products Found !", payload = products });
                //  return Ok(new { received = category.ToString() });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }

        [HttpPost("deleteproduct")]//admin
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            try
            {
                var product = await dbContext.Products.FindAsync(productId);
                if (product == null)
                {
                    return NotFound(new { message = "Item Not Found !" });
                }

                var remove = dbContext.Products.Remove(product);
                if (remove == null)
                {
                    return BadRequest(new { message = "Something Went Wrong !" });
                }
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Product Deleted Sucessfully !", payload = product });

            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }

    }
}