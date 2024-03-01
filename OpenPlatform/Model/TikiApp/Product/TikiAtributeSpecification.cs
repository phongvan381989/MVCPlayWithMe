using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Product
{
    public class TikiAtributeSpecification
    {
        public List<TikiAtributeSpecificationAtribute> attributes { get; set; }

        public int id { get; set; }

        public string name { get; set; }

        public int sort_order { get; set; }
    }
}
