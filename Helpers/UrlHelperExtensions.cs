using MVCPlayWithMe.General;
using MVCPlayWithMe.Models.ItemModel;
using System.Web.Mvc;

namespace MVCPlayWithMe.Helpers
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Generate URL cho trang chi tiết item
        /// VD: /item/doraemon-tap-1-123
        /// </summary>
        public static string ItemUrl(this UrlHelper urlHelper, int itemId, string itemName)
        {
            if (itemId <= 0 || string.IsNullOrWhiteSpace(itemName))
                return "#";

            string slug = Common.GenerateSlug(itemName);
            string slugId = slug + "-" + itemId;

            return urlHelper.RouteUrl("ItemDetail", new { slugId = slugId });
        }

        /// <summary>
        /// Generate URL cho trang chi tiết item từ object Item
        /// temporary comment
        /// </summary>
        //public static string ItemUrl(this UrlHelper urlHelper, Item item)
        //{
        //    if (item == null)
        //        return "#";

        //    return ItemUrl(urlHelper, item.id, item.name);
        //}
    }
}
