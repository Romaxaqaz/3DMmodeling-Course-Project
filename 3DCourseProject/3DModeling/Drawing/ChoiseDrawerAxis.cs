using System.Collections.Generic;
using System.Linq;
using _3DModeling.Model;

namespace _3DModeling.Drawing
{
    public static class ChoiseDrawerAxis
    {
        public static IEnumerable<IFacet> DrawXy(this IEnumerable<IFacet> collection)
        {
            var drawXy = collection as IList<IFacet> ?? collection.ToList();
            foreach (var item in drawXy)
            {
                foreach (var arris in item.ArristCollection)
                {
                    var bufferfx = arris.FirstVertex.Z;
                    var buffersx = arris.SecondVertex.Z;


                    arris.FirstVertex.Z = arris.FirstVertex.X;
                    arris.SecondVertex.Z = arris.SecondVertex.X;

                    arris.FirstVertex.X = bufferfx;
                   arris.SecondVertex.X = buffersx;

                }
            }
            return drawXy;
        }

        public static IEnumerable<IFacet> DrawYz(this IEnumerable<IFacet> collection)
        {
            var drawYz = collection as IList<IFacet> ?? collection.ToList();
            foreach (var item in drawYz)
            {
                foreach (var arris in item.ArristCollection)
                {
                    var bufferfx = arris.FirstVertex.Y;
                    var buffersx = arris.SecondVertex.Y;

                    arris.FirstVertex.Y = arris.FirstVertex.Z;
                    arris.SecondVertex.Y = arris.SecondVertex.Z;

                    arris.FirstVertex.Z= bufferfx;
                    arris.SecondVertex.Z = buffersx;

                }
            }
            return drawYz;
        }

        public static IEnumerable<IFacet> DrawXz(this IEnumerable<IFacet> collection)
        {
            var drawXz = collection as IList<IFacet> ?? collection.ToList();
            foreach (var item in drawXz)
            {
                foreach (var arris in item.ArristCollection)
                {
                    var bufferfx = arris.FirstVertex.X;
                    var buffersx = arris.SecondVertex.X;

                    arris.FirstVertex.X = arris.FirstVertex.Z;
                    arris.SecondVertex.X = arris.SecondVertex.Z;

                    arris.FirstVertex.Z = bufferfx;
                    arris.SecondVertex.Z = buffersx;
                }
            }
            return drawXz;
        }
    }
}

