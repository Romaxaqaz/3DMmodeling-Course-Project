using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DModeling.Figure
{
    public interface IRectangle
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
        double Width { get; set; }
        double Heigth { get; set; }
    }
}
