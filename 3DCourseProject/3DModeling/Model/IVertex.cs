using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DModeling.Model
{
    public interface IVertex
    {
        int Number { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
    }
}
