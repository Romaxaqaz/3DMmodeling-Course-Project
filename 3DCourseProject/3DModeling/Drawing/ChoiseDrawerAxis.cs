using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using _3DModeling.Model;

namespace _3DModeling.Drawing
{
    public static class ChoiseDrawerAxis
    {
        public static IEnumerable<IFacet> DrawXY(this IEnumerable<IFacet> collection)
        {
            var newColl = (IList<IFacet>)collection.DeepClone();

            foreach (var item in newColl)
            {
                foreach (var arris in item.ArristCollection)
                {
                    arris.FirstVertex.X = arris.FirstVertex.Z + WindowSize.Width;
                    arris.SecondVertex.X = arris.SecondVertex.Z + WindowSize.Width;

                    arris.FirstVertex.Y += WindowSize.Heigth;
                    arris.SecondVertex.Y += WindowSize.Heigth;

                    arris.FirstVertex.Z = 0;
                    arris.SecondVertex.Z = 0;
                }
            }
            return newColl;
        }

        public static IEnumerable<IFacet> DrawYZ(this IEnumerable<IFacet> collection)
        {
            var newColl = (IList<IFacet>)collection.DeepClone();

            foreach (var item in newColl)
            {
                foreach (var arris in item.ArristCollection)
                {
                    arris.FirstVertex.X += WindowSize.Width;
                    arris.SecondVertex.X += WindowSize.Width;

                    arris.FirstVertex.Y = arris.FirstVertex.Z + WindowSize.Heigth;
                    arris.SecondVertex.Y = arris.SecondVertex.Z + WindowSize.Heigth;

                    arris.FirstVertex.Z = 0;
                    arris.SecondVertex.Z = 0;
                }
            }
            return newColl;
        }

        public static IEnumerable<IFacet> DrawXZ(this IEnumerable<IFacet> collection)
        {
            var newColl = (IList<IFacet>)collection.DeepClone();

            foreach (var item in newColl)
            {
                foreach (var arris in item.ArristCollection)
                {
                    arris.FirstVertex.X +=WindowSize.Width;
                    arris.SecondVertex.X +=WindowSize.Width;

                    arris.FirstVertex.Y += WindowSize.Heigth;
                    arris.SecondVertex.Y += WindowSize.Heigth;
                }
            }
            return newColl;
        }



        private static object DeepClone(this object obj)
        {
            object objResult = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }
            return objResult;
        }
    }
}

