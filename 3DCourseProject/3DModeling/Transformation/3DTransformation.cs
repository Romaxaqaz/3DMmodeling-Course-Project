using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using _3DModeling.Model;

namespace _3DModeling.Transformation
{
    public class _3DTransformation
    {
        /// <summary>
        /// Scale transformation
        /// </summary>
        /// <param name="facets">collection facets</param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <returns>A new collection of faces with new peaks vertices</returns>
        public IEnumerable<IFacet> GetScaleFacets(IEnumerable<IFacet> facets, double scaleX, double scaleY, double scaleZ)
        {
            var scaleMatrix = DenseMatrix.OfArray(new[,] {
                                                    {scaleX,0,0,0},
                                                    {0,scaleY,0,0},
                                                    {0,0,scaleZ,0},
                                                    {0,0,0,0 }
                                                 });
            return Transformation(facets, scaleMatrix);
        }

        /// <summary>
        /// Rotate transformation
        /// </summary>
        /// <param name="facets">collection facets</param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <returns>A new collection of faces with new peaks vertices</returns>
        public IEnumerable<IFacet> GetRotateFacets(IEnumerable<IFacet> facets, double angleX, double angleY, double angleZ)
        {
            var angleRx = (angleX * (Math.PI / 180.0));
            var angleRy = (angleY * (Math.PI / 180.0));
            var angleRz = (angleZ * (Math.PI / 180.0));


            var rotateZ = DenseMatrix.OfArray(new[,] {
                                                    { Math.Cos(angleRz), Math.Sin(angleRz), 0, 0},
                                                    {-Math.Sin(angleRz), Math.Cos(angleRz), 0, 0},
                                                    { 0, 0, 1, 0},
                                                    { 0, 0, 0, 1 }
            });

            var rotateX = DenseMatrix.OfArray(new[,] {
                                                    { 1, 0, 0, 0 },
                                                    { 0, Math.Cos(angleRx), Math.Sin(angleRx), 0 },
                                                    { 0, -Math.Sin(angleRx), Math.Cos(angleRx), 0 },
                                                    { 0, 0, 0, 1 }
            });

            var rotateY = DenseMatrix.OfArray(new[,] {
                                                    { Math.Cos(angleRy), 0, -Math.Sin(angleRy), 0 },
                                                    { 0, 1, 0, 0 },
                                                    { Math.Sin(angleRy),0, Math.Cos(angleRy),0 },
                                                    { 0, 0, 0, 1 }
            });

            return Transformation(facets, (rotateX * rotateY * rotateZ));
        }

        /// <summary>
        /// Move transformation
        /// </summary>
        /// <param name="facets">collection facets</param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <returns>A new collection of faces with new peaks vertices</returns>
        public IEnumerable<IFacet> GetMoveFacets(IEnumerable<IFacet> facets, double moveX, double moveY, double moveZ)
        {
            var moveMatrix = DenseMatrix.OfArray(new[,] {
                                                    { 1, 0, 0, 0},
                                                    { 0, 1, 0, 0},
                                                    { 0, 0, 1, 0},
                                                    { moveX, moveY, moveZ, 1}
                                                  });

            return Transformation(facets, moveMatrix);
        }

        private IEnumerable<IFacet> Transformation(IEnumerable<IFacet> facets, DenseMatrix matrix)
        {
            var transformation = facets as IList<IFacet> ?? facets.ToList();

            foreach (var item in transformation)
            {
                foreach (var arris in item.ArristCollection)
                {
                    arris.FirstVertex =  PointOutVector(Vector(arris.FirstVertex) * matrix);
                    arris.SecondVertex = PointOutVector(Vector(arris.SecondVertex) * matrix);
                }
            }
            return transformation;
        }

        private Vector<double> Vector(IVertex vertex)
        {
            if (vertex == null) throw new ArgumentNullException(nameof(vertex));
            return Vector<double>.Build.DenseOfArray(new[] { vertex.X, vertex.Y, vertex.Z, 1 });
        }

        private static IVertex PointOutVector(IList<double> vector)
        {
            return new Vertex
            {
                X = vector[0],
                Y = vector[1],
                Z = vector[2]
            };
        }
    }
}
