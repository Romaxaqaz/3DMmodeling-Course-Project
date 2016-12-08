using System;
using System.Collections.Generic;
using System.Linq;
using _3DModeling.Extension;
using _3DModeling.Model;
using _3DModeling.Transformation;

namespace _3DModeling.Algorithms
{
    public class RobertsAlgorithm
    {
        /// <summary>
        /// Hide lines
        /// </summary>
        /// <param name="facets">Collection of facets</param>
        /// <param name="vertex">View point</param>
        /// <returns></returns>
        public IEnumerable<IFacet> HideLines(IEnumerable<IFacet> facets, IVertex vertex)
        {
            var collection = facets;
            var compositeEntities = collection as IList<IFacet> ?? collection.ToList();

            foreach (var compositeEntity in compositeEntities)
            {
                compositeEntity.Normal = FaceParameter.GetNormal(compositeEntity);
                compositeEntity.Center = FaceParameter.GetCenter(compositeEntity);
                compositeEntity.ViewVector = FaceParameter.GetViewVector(vertex, compositeEntity.Center);
                compositeEntity.IsHidden = IsHiddenCoord(compositeEntity.Normal, compositeEntity.ViewVector);
            }
            return compositeEntities;
        }

        private static bool IsHiddenCoord(IEnumerable<double> a, IEnumerable<double> b)
        {
            var cos = FaceParameter.GetCos(a, b);
            return cos < 0.001;
        }
    }
}
