using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DModeling.Model
{
    public interface IFacet
    {
        int FacetNumber { get; set; }
        IEnumerable<IArris> ArristCollection { get; set; }
    }
}
