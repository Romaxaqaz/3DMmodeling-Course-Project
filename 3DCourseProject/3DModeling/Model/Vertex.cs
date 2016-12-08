using System;
using _3DModeling.Enums;

namespace _3DModeling.Model
{
    [Serializable]
    public class Vertex : IVertex
    {
        public int Number { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public PointsType PointType { get; set; }

        public Vertex(int number, double x, double y, double z, PointsType pointType) 
        {
            Number = number;
            X = x;
            Y = y;
            Z = z;
            PointType = pointType;
        }

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
