using System.Threading.Tasks;
using backendProject.Data.SqlDbContext;
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
    public class ProductController : ControllerBase
    {
        private readonly SqlDbContext dbContext;
        private readonly ITokenService tokenService;
        public ProductController(SqlDbContext dbContext, ITokenService tokenService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
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
        [HttpPost("addtocart")]
        public async Task<IActionResult> AddToCart(Guid productId, int qty)
        {
            try
            {
                var token = HttpContext.Request.Cookies["Backend_Auth_Token"];
                if (token == null)
                {
                    return StatusCode(401, new { message = "Session Expired ! Kindly Login Again !" });
                }
                var userId = tokenService.VerifyTokenAndGetId(token);

                var product = await dbContext.Products.FindAsync(productId);
                if (product == null)
                {
                    return StatusCode(404, new { message = "Item not Found !" });
                }
                var cart = await dbContext.Carts.Include(c => c.CartProducts).FirstOrDefaultAsync(cart => cart.UserId == userId);
                if (cart == null)
                {
                    var newCart = new Cart
                    {
                        UserId = userId,
                        CartTotal = 0
                    };
                    var cartProduct = new CartProduct
                    {
                        CartId = newCart.CartId,
                        ProductId = productId,
                        Quantity = qty,
                        ProductPrice = product.ProductPrice
                    };
                    newCart.CartTotal = product.ProductPrice * qty;
                    await dbContext.Carts.AddAsync(newCart);
                    await dbContext.CartProducts.AddAsync(cartProduct);

                }
                else
                {
                    var existingCartProduct = await dbContext.CartProducts.FirstOrDefaultAsync(cp => cp.CartId == cart.CartId && cp.ProductId == productId);
                    if (existingCartProduct == null)
                    {
                        var cartProduct = new CartProduct
                        {
                            CartId = cart.CartId,
                            ProductId = productId,
                            Quantity = qty,
                            ProductPrice = product.ProductPrice
                        };
                        await dbContext.CartProducts.AddAsync(cartProduct);
                    }
                    else
                    {
                        existingCartProduct.Quantity += qty;
                    }
                    cart.CartTotal += product.ProductPrice * qty;
                }
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Product Added To Cart Sucessfully !", payload = cart });
                // var user = await sqlDb.Users.Include(user => user.Cart).ThenInclude(cart => cart.CartProducts).FirstOrDefaultAsync(user => user.UserId == userId);
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
        [HttpPost("removefromcart")]
        public async Task<IActionResult> RemoveFromCart(Guid productId)
        {
            try
            {
                var token = HttpContext.Request.Cookies["Backend_Auth_Token"];
                if (token == null)
                {
                    return BadRequest(new { message = "Session Expired ! Kindly Login Again ..." });
                }
                var userId = tokenService.VerifyTokenAndGetId(token);
                var cart = await dbContext.Carts.Include(cart => cart.CartProducts).FirstOrDefaultAsync(c => c.UserId == userId);
                if (cart == null)
                {
                    return BadRequest(new { message = "Something went wrong !" });
                }
                var cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.CartId == cart.CartId && cp.ProductId == productId);
                if (cartProduct == null)
                {
                    return NotFound(new { message = "Item Not Found !" });
                }
                var remove = dbContext.CartProducts.Remove(cartProduct);
                if (remove != null)
                {
                    cart.CartTotal -= cartProduct.ProductPrice * cartProduct.Quantity;
                    await dbContext.SaveChangesAsync();
                }
                return Ok(new { message = "Product Removed From Cart Sucessfully !", payload = cart });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
        [HttpPost("increasecartquantity")]
        public async Task<IActionResult> IncreaseCartQuantity(Guid productId)
        {
            try
            {
                var token = HttpContext.Request.Cookies["Backend_Auth_Token"];
                if (token == null)
                {
                    return StatusCode(401, new { message = "Session Expired ! Kindly Login Again !" });
                }
                var userId = tokenService.VerifyTokenAndGetId(token);

                var product = await dbContext.Products.FindAsync(productId);
                if (product == null)
                {
                    return StatusCode(404, new { message = "Item not Found !" });
                }
                var cart = await dbContext.Carts.Include(cart => cart.CartProducts).FirstOrDefaultAsync(cart => cart.UserId == userId);
                if (cart == null)
                {
                    return StatusCode(404, new { message = "Item not Found !" });
                }
                var existingCartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);
                if (existingCartProduct == null)
                {
                    var newCartProduct = new CartProduct
                    {
                        CartId = cart.CartId,
                        ProductId = productId,
                        Quantity = 1,
                        ProductPrice = product.ProductPrice
                    };
                    await dbContext.CartProducts.AddAsync(newCartProduct);
                }
                else
                {
                    existingCartProduct.Quantity += 1;
                }
                cart.CartTotal += product.ProductPrice;
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Product Quantity Increased In Cart Sucessfully !", payload = cart });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }

        [HttpPost("decreasecartquantity")]
        public async Task<IActionResult> DecreaseCartQuantity(Guid productId)
        {
            try
            {
                var token = HttpContext.Request.Cookies["Backend_Auth_Token"];
                if (token == null)
                {
                    return StatusCode(401, new { message = "Session Expired ! Kindly Login Again !" });
                }
                var userId = tokenService.VerifyTokenAndGetId(token);

                var product = await dbContext.Products.FindAsync(productId);
                if (product == null)
                {
                    return StatusCode(404, new { message = "Item not Found !" });
                }
                var cart = await dbContext.Carts.Include(cart => cart.CartProducts).FirstOrDefaultAsync(cart => cart.UserId == userId);
                if (cart == null)
                {
                    return StatusCode(404, new { message = "Item not Found !" });
                }
                var cartProduct = cart.CartProducts.FirstOrDefault(cp => cp.ProductId == productId);
                if (cartProduct == null)
                {
                    return NotFound(new { message = "Item Not Found !" });
                }
                if (cartProduct.Quantity <= 1)
                {
                    dbContext.CartProducts.Remove(cartProduct);
                }
                else
                {
                    cartProduct.Quantity -= 1;
                }


                cart.CartTotal -= product.ProductPrice;
                if (cart.CartTotal < 0) cart.CartTotal = 0;
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Product Quantity Decreased In Cart Sucessfully !", payload = cart });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }



        //--------------------------------------------------by myself------------------------------------------------------//
        [HttpGet("getproductsbycategory")]
        public async Task<ActionResult> GetProductsByCategory([FromQuery] ProductCategory category)
        {
            try
            {
                // if (category == ProductCategory.All)
                // {
                //     var products = await dbContext.Products.Where(p => p.IsAvailable).ToListAsync();

                //     return Ok(new { message = $"{products.Count} Products Found !", payload = products });
                // }
                // else
                // {
                //     var catProducts = await dbContext.Products.Where(p => p.IsAvailable && p.Category == category).ToListAsync();

                //     return Ok(new { message = $"{catProducts.Count} Products Found !", payload = catProducts });
                // }
                return Ok(new { received = category.ToString() });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
    }
}