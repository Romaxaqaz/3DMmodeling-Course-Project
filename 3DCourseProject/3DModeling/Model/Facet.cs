using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DModeling.Model
{
    public class Facet : IFacet
    {
        public int FacetNumber { get; set; }
        public IEnumerable<IArris> ArristCollection { get; set; }

    }
}
