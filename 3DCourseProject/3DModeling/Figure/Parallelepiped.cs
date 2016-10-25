using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using _3DModeling.Abstract;
using _3DModeling.Drawing;
using _3DModeling.Model;

namespace _3DModeling.Figure
{
    public class Parallelepiped : Detail
    {
        #region Fields
        public double Width { get; set; }
        public double Heigth { get; set; }
        public double Length { get; set; }
        public double UpCenterPosition { get; set; }
        public double LeftCentexPosition { get; set; }
        #endregion

        #region Lists
        private IList<IVertex> _upListVertex;
        private readonly IList<IVertex> _downListVertex = new List<IVertex>();
        private IList<IFacet> _facetLsit = new List<IFacet>();
        #endregion

        #region Constructor
        public Parallelepiped(Detail detail, double width, double heigth, double length, double upCenterPosition, double leftCenterPosition) : base(detail)
        {
            Width = width;
            Heigth = heigth;
            Length = length;
            UpCenterPosition = upCenterPosition;
            LeftCentexPosition = leftCenterPosition;

            InitializationVertex();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Create parallelepiped faces
        /// </summary>
        private void InitializationVertex()
        {
            var startPointX = LeftCentexPosition - (Width / 2);
            var startPointY = UpCenterPosition;
            var startPointZ = -Length / 2;

            _upListVertex = new List<IVertex>()
            {
                new Vertex
                {
                    Number = 1,
                    X= startPointX,
                    Y= startPointY,
                    Z= startPointZ
                },
                new Vertex
                {
                    Number = 2,
                    X = startPointX,
                    Y = startPointY,
                    Z = startPointZ + Length
                },
                 new Vertex
                 {
                     Number = 3,
                     X = startPointX + Width,
                     Y = startPointY,
                     Z = startPointZ + Length
                 },
                new Vertex
                {
                    Number = 4,
                    X = startPointX + Width,
                    Y = startPointY,
                    Z = startPointZ
                },
                new Vertex
                {
                    Number = 5,
                    X= startPointX,
                    Y= startPointY,
                    Z= startPointZ
                }
            };

            var index = _upListVertex.Count - 1;
            foreach (var item in _upListVertex)
            {
                _downListVertex.Add(new Vertex(index += 1, item.X, item.Y + Heigth, item.Z));
            }

            _facetLsit = (IList<IFacet>)DrawingFaces.GenerateFacets(_upListVertex, _downListVertex);
        }

        /// <summary>
        /// Return figure faces
        /// </summary>
        /// <returns>Collection faces</returns>
        public override IEnumerable<IFacet> FacetCollection()
        {
            if (detail == null) return _facetLsit;
            var collection = detail.FacetCollection();
            return collection.Concat(_facetLsit);
        }
        #endregion
    }
}
