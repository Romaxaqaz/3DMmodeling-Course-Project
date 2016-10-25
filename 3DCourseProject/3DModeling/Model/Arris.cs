using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _3DModeling.Model
{
    [Serializable]
    public class Arris : IArris
    {
        public int NumberFacet { get; set; }
        public IVertex FirstVertex { get; set; }
        public IVertex SecondVertex { get; set; }
        //color
    }
}
