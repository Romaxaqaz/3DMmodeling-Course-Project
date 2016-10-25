using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _3DModeling.Model
{
    [Serializable]
    public class Facet : IFacet
    {
        public int FacetNumber { get; set; }
        public IEnumerable<IArris> ArristCollection { get; set; }
    }
}
