using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using _3DModeling.Model;
using System.Windows;
using _3DModeling.Abstract;
using _3DModeling.Drawing;
using _3DModeling.Enums;

namespace _3DModeling.Figure
{
    public class Cylinder : Detail
    {
        #region Variables
        public double Alpha { get; set; }
        public double CylinderRadius { get; set; }
        public double CylinderHeigth { get; set; }
        public double ApproksimationValue { get; set; }
        public double UpCentexPosition { get; set; }
        public double LeftCentexPosition { get; set; }
        #endregion

        #region Lists
        private readonly IList<IVertex> _upListVertex = new List<IVertex>();
        private readonly IList<IVertex> _downListVertex = new List<IVertex>();
        private IList<IFacet> _facetLsit = new List<IFacet>();
        #endregion

        #region Constructor
        public Cylinder(Detail detail, double cylinderRadius, 
            double cylinderHeigth, 
            double approksimationValue, 
            double upCentexPosition, 
            double leftCenterPosition) : base(detail)
        {
            Alpha = 360.0 / approksimationValue;
            CylinderRadius = cylinderRadius;
            CylinderHeigth = cylinderHeigth;
            ApproksimationValue = approksimationValue;
            UpCentexPosition = upCentexPosition;
            LeftCentexPosition = leftCenterPosition;

            GenerateCylinder();
        }
        #endregion

        #region Methods
        /// <summary>
        /// It creates a face of the cylinder
        /// </summary>
        private void GenerateCylinder()
        {
            var angle = Alpha;
            var cylinderRadius = CylinderRadius;
            // create up vertex
            for (var i = 0; i < ApproksimationValue; i++)
            {
                var angleR = angle * (Math.PI / 180.0);
                var x = cylinderRadius * Math.Cos(angleR);
                var z = cylinderRadius * Math.Sin(angleR);
                var y = UpCentexPosition;
                _upListVertex.Add(new Vertex(i, x + LeftCentexPosition, y, z, PointsType.Up));
                angle += Alpha;
            }

            var lastNumber = _upListVertex.Count - 1;
            _upListVertex.Add(new Vertex(lastNumber + 1, _upListVertex[0].X, _upListVertex[0].Y, _upListVertex[0].Z, PointsType.Up));

            // create down vertex
            var index = _upListVertex.Count - 1;
            foreach (var item in _upListVertex)
            {
                _downListVertex.Add(new Vertex(index += 1, item.X, item.Y + CylinderHeigth, item.Z, PointsType.Down));
            }

            _facetLsit = (IList<IFacet>)DrawingFaces.GenerateFacets(_upListVertex, _downListVertex, nameof(Cylinder));
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

        #region Propertys

        public ObservableCollection<UIElement> UiElementsCollection { get; set; } = new ObservableCollection<UIElement>();

        #endregion
    }
}
