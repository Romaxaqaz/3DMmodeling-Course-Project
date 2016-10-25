using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using _3DModeling.Model;

namespace _3DModeling.Drawing
{
    public static class DrawingFaces
    {
        /// <summary>
        /// Draw faces
        /// </summary>
        /// <param name="facetList"></param>
        /// <returns>Line collection</returns>
        public static IEnumerable<UIElement> DrawFacet(IEnumerable<IFacet> facetList)
        {
            var uiElementsCollection = new ObservableCollection<UIElement>();

            foreach (var item in facetList)
            {
                var vertexCollection = item.ArristCollection;

                foreach (var vertex in vertexCollection)
                {
                    var firstVertex = vertex.FirstVertex;
                    var secondVertex = vertex.SecondVertex;

                    uiElementsCollection.Add(CreateLine(firstVertex.X, firstVertex.Y, secondVertex.X, secondVertex.Y));
                }
            }
            return uiElementsCollection;
        }

        /// <summary>
        /// It creates a collection of faces of the vertex
        /// </summary>
        /// <param name="upListVertex"></param>
        /// <param name="downListVertex"></param>
        /// <returns></returns>
        public static IEnumerable<IFacet> GenerateFacets(IList<IVertex> upListVertex, IList<IVertex> downListVertex)
        {
            var facetLsit = new List<IFacet>();

            if (upListVertex == null) throw new NullReferenceException("List vertex empty");

            var number = 0;
            for (var i = 0; i < upListVertex.Count - 1; i++)
            {
                var arris = new List<Arris>
                {
                    new Arris
                    {
                        NumberFacet = number,
                        FirstVertex = new Vertex(number, upListVertex[i].X, upListVertex[i].Y, upListVertex[i].Z),
                        SecondVertex = new Vertex(number, upListVertex[(i + 1)].X, upListVertex[(i + 1)].Y, upListVertex[(i + 1)].Z)
                    },
                    new Arris
                    {
                        NumberFacet = number,
                        FirstVertex = new Vertex(number, upListVertex[(i + 1)].X, upListVertex[(i + 1)].Y, upListVertex[(i + 1)].Z),
                        SecondVertex = new Vertex(number, downListVertex[(i + 1)].X, downListVertex[(i + 1)].Y, downListVertex[(i + 1)].Z)
                    },
                    new Arris
                    {
                        NumberFacet = number,
                        FirstVertex = new Vertex(number, downListVertex[(i + 1)].X, downListVertex[(i + 1)].Y, downListVertex[(i + 1)].Z),
                        SecondVertex = new Vertex(number, downListVertex[i].X, downListVertex[i].Y, downListVertex[i].Z)
                    },
                    new Arris
                    {
                        NumberFacet = number,
                        FirstVertex = new Vertex(number, downListVertex[i].X, downListVertex[i].Y, downListVertex[i].Z),
                        SecondVertex = new Vertex(number, upListVertex[i].X, upListVertex[i].Y, upListVertex[i].Z)
                    }
                };

                var facet = new Facet
                {
                    FacetNumber = number,
                    ArristCollection = arris
                };

                facetLsit.Add(facet);
                number++;
            }
            return facetLsit;
        }

        private static Line CreateLine(double x1, double y1, double x2, double y2)
        {
            var line = new Line
            {
                Stroke = new SolidColorBrush(Color.FromRgb(0,0,154)),
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
            return line;
        }
    }
}
