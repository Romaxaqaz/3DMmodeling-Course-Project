using System.Collections.Generic;
using _3DModeling.Model;

namespace _3DModeling.Extension
{
    public static class VertexToDoubleCollection
    {
        public static IEnumerable<double> ConvertToDoubleCollection(this IVertex vertex) => 
            new[] {vertex.X, vertex.Y, vertex.Z};
    }
}
