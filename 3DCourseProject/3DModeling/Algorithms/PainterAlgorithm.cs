using System;
using System.Collections.Generic;
using System.Linq;
using _3DModeling.Model;

namespace _3DModeling.Algorithms
{
    public class PainterAlgorithm
    {
        private readonly Dictionary<double, IFacet> _facetDictioanary = new Dictionary<double, IFacet>();
        private readonly List<Facet> _resultFacest = new List<Facet>();

        public IEnumerable<IFacet> ComptletedFacet(IEnumerable<IFacet> facetCollection)
        {
            try
            {
                foreach (var item in facetCollection)
                {
                    var zParameter = item.ArristCollection.Sum(ar => ar.FirstVertex.Z + ar.SecondVertex.Z);
                    var averageZ = zParameter / (item.ArristCollection.Count()*2);
                    var inLi = _facetDictioanary.FirstOrDefault(x => x.Key == averageZ).Value;
                    if (inLi != null) continue;
                    _facetDictioanary.Add(averageZ, item);
                }
            }
            catch (ArgumentException)
            {}

            var keys = _facetDictioanary.Select(face => face.Key).ToList();
            keys.Sort();
            keys.Reverse();

            foreach (var key in keys)
            {
                var face = _facetDictioanary.FirstOrDefault(x => x.Key == key).Value;
                _resultFacest.Add((Facet)face);
            }
            return _resultFacest;
        }

    }
}
