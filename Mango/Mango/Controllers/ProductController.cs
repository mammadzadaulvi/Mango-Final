using Mango.DAL;
using Mango.Models;
using Mango.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;

namespace Mango.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AddToBasket(int productId)
        {
            User user = null;

            if (User.Identity.IsAuthenticated)
            {
                user = _context.Users.FirstOrDefault(x => x.UserName.ToLower() == User.Identity.Name.ToLower());
            }
            if(user == null)
            {
                return StatusCode(401);
            }
            if (!_context.Products.Any(x => x.Id == productId)) return NotFound();

            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            BasketItemViewModel basketItem = null;

            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

                if (basketItemsStr != null)
                {
                    basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                    basketItem = basketItems.FirstOrDefault(x => x.ProductId == productId);

                    if (basketItem != null) basketItem.Count++;
                    else
                    {
                        basketItem = new BasketItemViewModel
                        {
                            ProductId = productId,
                            Count = 1
                        };
                        basketItems.Add(basketItem);
                    }
                }
                else
                {
                    basketItem = new BasketItemViewModel
                    {
                        ProductId = productId,
                        Count = 1
                    };

                    basketItems.Add(basketItem);
                }
                basketItemsStr = JsonConvert.SerializeObject(basketItems);

                HttpContext.Response.Cookies.Append("BasketItems", basketItemsStr);
          

            return Ok(); // 200
        }

        public IActionResult RemoveFromBasket(int productId)
        {
            if (!_context.Products.Any(x => x.Id == productId)) return NotFound();

            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            BasketItemViewModel basketItem = null;

            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

            if (basketItemsStr != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                basketItem = basketItems.FirstOrDefault(x => x.ProductId == productId);


                if (basketItem != null && basketItem.Count > 1) basketItem.Count--;
                else if (basketItem == null) return NotFound();
                else
                {
                    basketItems.Remove(basketItem);
                }
            }
            else
            {
                return NotFound();
            }
            
            basketItemsStr = JsonConvert.SerializeObject(basketItems);

            HttpContext.Response.Cookies.Append("BasketItems", basketItemsStr);


            return Ok();
        }

        public IActionResult AddToFavourites(int productId)
        {
            if (!_context.Products.Any(x => x.Id == productId)) return NotFound();

            List<FavouriteItemVM> favouriteItems = new List<FavouriteItemVM>();
            FavouriteItemVM favouriteItem = null;

            string favouriteItemsStr = HttpContext.Request.Cookies["FavouriteItems"];

            if (favouriteItemsStr != null)
            {
                favouriteItems = JsonConvert.DeserializeObject<List<FavouriteItemVM>>(favouriteItemsStr);

                favouriteItem = favouriteItems.FirstOrDefault(x => x.ProductId == productId);

                if (favouriteItem != null) favouriteItems.Remove(favouriteItem);
                else
                {
                    favouriteItem = new FavouriteItemVM
                    {
                        ProductId = productId,
                    };
                    favouriteItems.Add(favouriteItem);
                }
            }
            else
            {
                favouriteItem = new FavouriteItemVM
                {
                    ProductId = productId,
                };

                favouriteItems.Add(favouriteItem);
            }
            favouriteItemsStr = JsonConvert.SerializeObject(favouriteItems);

            HttpContext.Response.Cookies.Append("FavouriteItems", favouriteItemsStr);


            return Ok(); // 200
        }

        public IActionResult GetFavouriteItems()
        {
            List<FavouriteItemVM> favouriteItems = new List<FavouriteItemVM>();
            string favouriteItemsStr = HttpContext.Request.Cookies["FavouriteItems"];

            if (favouriteItemsStr != null)
            {
                favouriteItems = JsonConvert.DeserializeObject<List<FavouriteItemVM>>(favouriteItemsStr);
            }

            return Json(favouriteItems);
        }
        public IActionResult GetBasketItems()
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

            if (basketItemsStr != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);
            }

            return Json(basketItems);
        }

        public IActionResult Checkout()
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            List<CheckoutItemVM> checkoutItems = new List<CheckoutItemVM>();
            CheckoutItemVM checkoutItem = null;
            string basketItemsStr = HttpContext.Request.Cookies["BasketItems"];

            if (basketItemsStr != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);

                foreach (var item in basketItems)
                {
                    var product = _context.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    if (product != null)
                    {

                        checkoutItem = new CheckoutItemVM
                        {
                            Product = _context.Products.Include(p => p.ProductPhotos).FirstOrDefault(x => x.Id == item.ProductId),
                            Count = item.Count
                        };
                        checkoutItems.Add(checkoutItem);
                    }
                }
            }

            return View(checkoutItems);
        }

        public IActionResult Favorites()
        {
            List<FavouriteItemVM> favouriteItems = new List<FavouriteItemVM>();
            List<FavouriteItemDetailVM> favouriteItemDetailVMs = new List<FavouriteItemDetailVM>();
            FavouriteItemDetailVM favouriteItemDetailVM = null;
            string favouriteItemsStr = HttpContext.Request.Cookies["FavouriteItems"];

            if (favouriteItemsStr != null)
            {
                favouriteItems = JsonConvert.DeserializeObject<List<FavouriteItemVM>>(favouriteItemsStr);

                foreach (var item in favouriteItems)
                {
                    favouriteItemDetailVM = new FavouriteItemDetailVM
                    {
                        Product = _context.Products.Include(p => p.ProductPhotos).FirstOrDefault(x => x.Id == item.ProductId),
                    };
                    favouriteItemDetailVMs.Add(favouriteItemDetailVM);
                }
            }

            return View(favouriteItemDetailVMs);
        }
    }
}
