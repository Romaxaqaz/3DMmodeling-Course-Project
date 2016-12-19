using System;
using System.Collections.Generic;
using System.Linq;
using _3DModeling.Extension;
using _3DModeling.Model;

namespace _3DModeling.Transformation
{
    public class FaceParameter
    {
        public static double GetCos(IEnumerable<double> normal, IEnumerable<double> viewVector)
        {
            var multiply = MultiplyVectors(normal, viewVector);
            var normalLength = GetVectorLength(normal);
            var viewVectorLength = GetVectorLength(viewVector);
            return multiply / (normalLength * viewVectorLength);
        }

        private static double MultiplyVectors(IEnumerable<double> first, IEnumerable<double> second)
        {
            double result = 0;
            for (var i = 0; i < first.Count(); i++)
            {
                result += first.ElementAt(i) * second.ElementAt(i);
            }
            return result;
        }

        private static double GetVectorLength(IEnumerable<double> vector)
        {
            var sum = vector.Aggregate(0f, (res, f) => res + (float)Math.Pow(f, 2));
            return (float)Math.Sqrt(sum);
        }

        public static IEnumerable<double> GetViewVector(IVertex viewPoint, IEnumerable<double> center)
        {
            var vector = new List<double>();
            var viePointVector = viewPoint.ConvertToDoubleCollection().ToList();

            for (var i = 0; i < viePointVector.Count; i++)
            {
                vector.Add(viePointVector.ElementAt(i) - center.ElementAt(i));
            }
            return vector;
        }

        public static IEnumerable<double> GetCenter(IFacet faces)
        {
            var points = faces.ArristCollection.ToList();
            var xPoints = points.Sum(x => x.FirstVertex.X) / points.Count;
            var yPoints = points.Sum(x => x.FirstVertex.Y) / points.Count;
            var zPoints = points.Sum(x => x.FirstVertex.Z) / points.Count;

            return new[] { xPoints, yPoints, zPoints };
        }

        public static IEnumerable<double> GetNormal(IFacet facet)
        {
            var arris = facet.ArristCollection.ToList();

            var first = arris[0].FirstVertex;
            var second = arris[1].FirstVertex;
            var third = arris[2].FirstVertex;

            var x = first.Y * second.Z + second.Y * third.Z + third.Y * first.Z -
                    second.Y * first.Z - third.Y * second.Z - first.Y * third.Z;
            var y = first.Z * second.X + second.Z * third.X + third.Z * first.X -
                    second.Z * first.X - third.Z * second.X - first.Z * third.X;
            var z = first.X * second.Y + second.X * third.Y + third.X * first.Y -
                    second.X * first.Y - third.X * second.Y - first.X * third.Y;

            return facet.ReverseNormal ? new[] { -x, -y, -z } : new[] { x, y, z };
        }
    }
}
