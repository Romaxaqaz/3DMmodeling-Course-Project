using System.Collections.Generic;
using System.Linq;
using _3DModeling.Abstract;
using _3DModeling.Enums;
using _3DModeling.Model;

namespace _3DModeling.Figure
{
    public class DetailsCombine
    {
        public static IEnumerable<Facet> DoubleDetailFacet(Detail detail)
        {
            var upAndDownFace = new List<Facet>();

            var allFaces = detail.FacetCollection().ToList();
            var upFaces = GetsUpOrDownFaces(allFaces, PointsType.Up);
            var downFaces = GetsUpOrDownFaces(allFaces, PointsType.Down);

            upAndDownFace.Add(NewFacet(upFaces, 0));
            upAndDownFace.Add(NewFacet(downFaces, 1));
            return upAndDownFace;
        }

        private static Facet NewFacet(IEnumerable<IFacet> facets, int index)
        {
            var enumerable = facets as IList<IFacet> ?? facets.ToList();
            var circleFace = enumerable.Where(c => c.NameFigure == "Cylinder").ToList();
            var parFace = enumerable.Where(c => c.NameFigure == "Parallelepiped").ToList();

            var cylinderPoints = GetNewPoints(circleFace, circleFace.Count, index);
            var parallelepipedPoints = GetNewPoints(parFace, parFace.Count, index);
            parallelepipedPoints.Reverse();

            var detailCombine = GenerateArris(parallelepipedPoints);
            detailCombine.AddRange(GenerateArris(cylinderPoints));

            return new Facet
            {
                NameFigure = "Cover",
                FacetNumber = 100,
                ArristCollection = detailCombine,
                ReverseNormal = index != 1
            };
        }

        private static List<IVertex> GetNewPoints(IEnumerable<IFacet> faces, int countPoints, int index)
        {
            var vertColl = new List<IVertex>();
            var enumerable = faces as IList<IFacet> ?? faces.ToList();
            
            for (var i = 0; i < countPoints; i++)
            {
                var coll = enumerable.Where(w => w.FacetNumber == i).ToList();
                var itItem = coll.Select(v => v.ArristCollection).ToList();
                foreach (var aItem in itItem)
                {
                    var selectd = index == 0
                        ? aItem.ToList().FirstOrDefault()?.FirstVertex
                        : aItem.ToList().LastOrDefault()?.FirstVertex;

                    vertColl.Add(selectd);
                    break;
                }
            }
            return vertColl;
        }

        private static List<IArris> GenerateArris(IList<IVertex> vertices)
        {
            var newFaceColl = new List<IArris>();
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                newFaceColl.Add(new Arris
                {
                    FirstVertex = vertices[i],
                    SecondVertex = vertices[i + 1]
                });
            }
            newFaceColl.Add(new Arris
            {
                FirstVertex = vertices[vertices.Count - 1],
                SecondVertex = vertices[0],
            });
            newFaceColl.Add(new Arris
            {
                FirstVertex = vertices[0],
                SecondVertex = vertices[1],
            });
            return newFaceColl;
        }

        private static IEnumerable<IFacet> GetsUpOrDownFaces(IEnumerable<IFacet> faces, PointsType point)
        {
            return (from allFace in faces
                           from fItem in allFace.ArristCollection
                           where fItem.FirstVertex.PointType == point
                    select allFace).ToList();
        }

    }
}
