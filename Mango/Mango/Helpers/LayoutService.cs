using Mango.ViewModels.Products;
using Newtonsoft.Json;

namespace Mango.Helpers
{
    public class LayoutService
    {
        private readonly IHttpContextAccessor _accessor;

        public LayoutService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public int GetBasketItemsCount()
        {
            List<BasketItemViewModel> basketItems = new List<BasketItemViewModel>();
            string basketItemsStr = _accessor.HttpContext.Request.Cookies["BasketItems"];

            if (basketItemsStr != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketItemViewModel>>(basketItemsStr);
            }

            return basketItems.Count;
        }

        public int GetFavouriteItemsCount()
        {
            List<FavouriteItemVM> favouriteItems = new List<FavouriteItemVM>();
            string favouriteItemsStr = _accessor.HttpContext.Request.Cookies["FavouriteItems"];

            if (favouriteItemsStr != null)
            {
                favouriteItems = JsonConvert.DeserializeObject<List<FavouriteItemVM>>(favouriteItemsStr);
            }

            return favouriteItems.Count;
        }

    }
}
