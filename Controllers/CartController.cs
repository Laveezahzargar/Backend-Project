using backendProject.Data.SqlDbContext;
using backendProject.Middlewares;
using backendProject.Models.DomainModels;
using backendProject.Models.JunctionModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backendProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly SqlDbContext dbContext;
        public CartController(SqlDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpPost("addtocart")]//logged in user
        public async Task<IActionResult> AddToCart(Guid productId, int qty)
        {
            try
            {
                var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
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
        [HttpPost("removefromcart")]//logged in user
        public async Task<IActionResult> RemoveFromCart(Guid productId)
        {
            try
            {
                Guid? userId = HttpContext.Items["userId"] as Guid?;
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



        //------------------------------------------------------by myself------------------------------------------------------------
        [HttpPost("increasecartquantity")]//logged in user
        public async Task<IActionResult> IncreaseCartQuantity(Guid productId)
        {
            try
            {
                Guid? userId = HttpContext.Items["userId"] as Guid?;
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

        [HttpPost("decreasecartquantity")]//logged in user
        public async Task<IActionResult> DecreaseCartQuantity(Guid productId)
        {
            try
            {
                Guid? userId = HttpContext.Items["userId"] as Guid?;
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
        [HttpPost("deletecart")]//logged in user
        public async Task<IActionResult> DeleteCart(Guid cartId)
        {
            try
            {
                var cart = await dbContext.Carts.FindAsync(cartId);
                if (cart == null)
                {
                    return NotFound(new { message = "Item Not Found !" });
                }
                var remove = dbContext.Carts.Remove(cart);
                if (remove == null)
                {
                    return BadRequest(new { message = "Something Went Wrong !" });
                }
                await dbContext.SaveChangesAsync();
                return Ok(new { message = "Cart Deleted Sucessfully !", payload = cart });

            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
        [HttpGet("getcartbyid")]//admin
        public async Task<IActionResult> GetCartById(Guid cartId)
        {
            try
            {
                var cart = await dbContext.Carts.FindAsync(cartId);
                if (cart == null)
                {
                    return NotFound(new { message = "Item Not Found !" });
                }
                return Ok(new { message = "Cart Founded Sucessfully !", payload = cart });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
        [HttpGet("getallcarts")]//admin
        public async Task<IActionResult> GetAllCarts()
        {
            try
            {
                var carts = await dbContext.Carts.Where(c => c.CartProducts != null).ToListAsync();
                return Ok(new { message = $"{carts.Count} carts Found !", payload = carts });
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"{error}" });
            }
        }
    }
}
