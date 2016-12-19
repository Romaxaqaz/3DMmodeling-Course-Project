using System.Collections.Generic;
using _3DModeling.Model;

namespace _3DModeling.Abstract
{
    public abstract class Detail
    {
        protected Detail detail = null;

        protected Detail(Detail detail)
        {
            this.detail = detail;
        }

        public abstract IEnumerable<IFacet> FacetCollection();

    }
}
