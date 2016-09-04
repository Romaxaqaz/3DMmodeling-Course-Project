using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using _3DModeling.Model;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _3DModeling.Figure
{
    public class Cylinder
    {
        #region Variables
        private const double UpCentexPosition = 100;
        private const double LeftCentexPosition = 200;

        public double Alpha { get; set; }
        public double CylinderRadius { get; set; }   
        public double CylinderHeigth { get; set; }
        public double ApproksimationValue { get; set; }
        #endregion

        #region Lists
        private List<Vertex> _upListVertex = new List<Vertex>();
        private List<Vertex> _downListVertex = new List<Vertex>();
        private List<Facet> _facetLsit = new List<Facet>();
        #endregion

        #region Constructor
        public Cylinder(double cylinderRadius, double cylinderHeigth, double approksimationValue)
        {
            Alpha = 360.0 / approksimationValue;
            CylinderRadius = cylinderRadius;
            CylinderHeigth = cylinderHeigth;
            ApproksimationValue = approksimationValue;
        }
        #endregion

        #region Methods

        private void CreateUpVertexCollection()
        {
            double angle = Alpha;
            var cylinderRadius = CylinderRadius;
            for (var i = 0; i < ApproksimationValue; i++)
            {
                var angleR = angle * (Math.PI / 180.0);
                var x = cylinderRadius * Math.Cos(angleR);
                var z = cylinderRadius * Math.Sin(angleR);
                var y = UpCentexPosition;
                _upListVertex.Add(new Vertex(i, x + LeftCentexPosition, y, z));
                angle += Alpha;
            }
        }

        private void CreateDownVertexCollection()
        {
            var index = _upListVertex.Count - 1;
            foreach (var item in _upListVertex)
            {
                _downListVertex.Add(new Vertex(index += 1, item.X, item.Y + CylinderHeigth, item.Z));
            }
        }

        private void FacetCollection()
        {
            if (_upListVertex == null) throw new NullReferenceException("List vertex empty");

            var number = 0;
            for (var i = 0; i < _upListVertex.Count - 1; i++)
            {
                var arris = new List<Arris>
                {
                    new Arris
                    {
                        NumberFacet = number,
                        FirstVertex = new Vertex(number, _upListVertex[i].X, _upListVertex[i].Y, _upListVertex[i].Z),
                        SecondVertex = new Vertex(number, _upListVertex[(i + 1)].X, _upListVertex[(i + 1)].Y, _upListVertex[(i + 1)].Z)
                    },
                    new Arris
                    {
                        NumberFacet = number,
                        FirstVertex = new Vertex(number, _upListVertex[(i + 1)].X, _upListVertex[(i + 1)].Y, _upListVertex[(i + 1)].Z),
                        SecondVertex = new Vertex(number, _downListVertex[(i + 1)].X, _downListVertex[(i + 1)].Y, _downListVertex[(i + 1)].Z)
                    },
                    new Arris
                    {
                        NumberFacet = number,
                        FirstVertex = new Vertex(number, _downListVertex[(i + 1)].X, _downListVertex[(i + 1)].Y, _downListVertex[(i + 1)].Z),
                        SecondVertex = new Vertex(number, _downListVertex[i].X, _downListVertex[i].Y, _downListVertex[i].Z)
                    },
                    new Arris
                    {
                        NumberFacet = number,
                        FirstVertex = new Vertex(number, _downListVertex[i].X, _downListVertex[i].Y, _downListVertex[i].Z),
                        SecondVertex = new Vertex(number, _upListVertex[i].X, _upListVertex[i].Y, _upListVertex[i].Z)
                    }
                };

                var facet = new Facet
                {
                    FacetNumber = number,
                    ArristCollection = arris
                };

                _facetLsit.Add(facet);
                number++;
            }
        }

        public void StartInitialization()
        {
            CreateUpVertexCollection();
            CreateDownVertexCollection();
            FacetCollection();
        }

        public IEnumerable<UIElement> DrawFacet(IEnumerable<IFacet> facetList)
        {
            
            if (UiElementsCollection == null)
                UiElementsCollection = new ObservableCollection<UIElement>();

            foreach (var item in facetList)
            {
                var vertexCollection = item.ArristCollection;

                foreach (var item2 in vertexCollection)
                {
                    var firstVertex = item2.FirstVertex;
                    var secondVertex = item2.SecondVertex;

                    UiElementsCollection.Add(CreateLine(firstVertex.X, firstVertex.Y, secondVertex.X, secondVertex.Y));
                }
            }
            return UiElementsCollection;
        }

        public void Clear()
        {
            _upListVertex = new List<Vertex>();
            _downListVertex = new List<Vertex>();
            _facetLsit = new List<Facet>();
            UiElementsCollection = null;
        }

        private static Line CreateLine(double x1, double y1, double x2, double y2)
        {
            var line = new Line
            {
                Stroke = new SolidColorBrush(Color.FromArgb(255, 213, 122, 212)),
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
            return line;
        }
        #endregion

        #region Propertys
         
        public ObservableCollection<UIElement> UiElementsCollection { get; set; } = new ObservableCollection<UIElement>();

        public List<Vertex> UpVertexCollection
        {
            get { return _upListVertex; }
        }

        public List<Vertex> DownVertexCollection
        {
            get { return _downListVertex; }
        }

        public List<Facet> FacetsList
        {
            get { return _facetLsit; }
        }
        #endregion
    }
}
