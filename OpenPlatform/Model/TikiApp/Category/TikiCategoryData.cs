using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Category
{
    public class TikiCategoryData
    {
        public List<TikiCategory> data { get; set; }

        public TikiCategoryData()
        {
            data = new List<TikiCategory>();
        }
    }
}