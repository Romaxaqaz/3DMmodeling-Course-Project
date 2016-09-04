using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DModeling.Model
{
    public class Arris : IArris
    {
        public int NumberFacet { get; set; }
        public IVertex FirstVertex { get; set; }
        public IVertex SecondVertex { get; set; }
    }
}
