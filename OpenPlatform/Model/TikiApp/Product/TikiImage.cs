using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    /// <summary>
    /// {
    //    "id": 12348871,
    //    "url": "https://uat.tikicdn.com/ts/h/g/hgth_-_1_1.jpg",
    //    "path": "/h/g/hgth_-_1_1.jpg",
    //    "position": 1
    //  }
    /// </summary>
public class TikiImage
    {
        /// <summary>
        /// unique image ID
        /// </summary>
        public Int32 id { get; set; }

        /// <summary>
        /// image path
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// image URL
        /// </summary>
        public string url { get; set; }
        public Int32 position { get; set; }
        public string label { get; set; }
        public bool is_gallery { get; set; }
    }
}
