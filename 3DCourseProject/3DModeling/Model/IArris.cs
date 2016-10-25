using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DModeling.Model
{
    public interface IArris
    {
        int NumberFacet { get; set; }
        IVertex FirstVertex { get; set; }
        IVertex SecondVertex { get; set; }
    }
}
