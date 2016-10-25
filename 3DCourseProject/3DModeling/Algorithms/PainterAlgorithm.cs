using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    var averageZ = zParameter / 8;
                    var inLi = _facetDictioanary.FirstOrDefault(x => x.Key == averageZ).Value;
                    if (inLi != null) continue;
                    _facetDictioanary.Add(averageZ, item);
                }
            }

            catch (ArgumentException ex)
            {
                var ms = ex.Message;
            }

            var keys = _facetDictioanary.Select(face => face.Key).ToList();
            keys.Sort();

            foreach (var key in keys)
            {
                var f = _facetDictioanary.FirstOrDefault(x => x.Key == key).Value;
                _resultFacest.Add((Facet)f);
            }
            return _resultFacest;
        }

    }
}
