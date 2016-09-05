using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DModeling.Model
{
    public class Vertex : IVertex
    {
        public int Number { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vertex(int number, double x, double y, double z)
        {
            Number = number;
            X = x;
            Y = y;
            Z = z;       
        }

        public Vertex()
        {
          
        }
    }
}
