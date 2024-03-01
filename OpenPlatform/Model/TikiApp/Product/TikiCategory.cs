using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    /// <summary>
    /// *Note:

    /// if product type is simple(only one variant) then option_attributes must be empty list instead of null value because option attributes is a required field.
    /// images do not accept null value, please put empty list if product don’t have any image.The avatar from image will be added to images later so you don’t need to add it 2 times.
    /// Even you do that, we will check duplicate image by url.

    /// * For the best user experience, TIKI only display image have size greater than 500×500 pixel in the media gallery and lower than 700 width pixel inside description
    /// </summary>
    public class TikiCategory
    {
        /// <summary>
        /// unique category ID
        /// </summary>
        public Int32 id { get; set; }

        /// <summary>
        /// category name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// path from root to this category
        /// </summary>
        public string path { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string url_key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool is_primary { get; set; }
    }
}
