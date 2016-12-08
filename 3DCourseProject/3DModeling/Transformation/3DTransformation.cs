using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using _3DModeling.Model;
using static System.Math;

namespace _3DModeling.Transformation
{
    public class _3DTransformation
    {
        #region Rotate / Move / Scale

        /// <summary>
        /// Scale transformation
        /// </summary>
        /// <param name="facets">collection facets</param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        /// <returns>A new collection of faces with new peaks vertices</returns>
        public IEnumerable<IFacet> GetScaleFacets(IEnumerable<IFacet> facets, double scaleX = 1, double scaleY = 1, double scaleZ = 1)
        {
            var scaleMatrix = DenseMatrix.OfArray(new[,] {
                                                    {scaleX,0,0,0},
                                                    {0,scaleY,0,0},
                                                    {0,0,scaleZ,0},
                                                    {0,0,0,1 }
                                                 });
            return Transformation(facets, scaleMatrix);
        }

        /// <summary>
        /// Rotate transformation
        /// </summary>
        /// <param name="facets">collection facets</param>
        /// <param name="angleX"></param>
        /// <param name="angleY"></param>
        /// <param name="angleZ"></param>
        /// <returns>A new collection of faces with new peaks vertices</returns>
        public IEnumerable<IFacet> GetRotateFacets(IEnumerable<IFacet> facets, double angleX, double angleY, double angleZ)
        {
            var angleRx = (angleX * (PI / 180.0));
            var angleRy = (angleY * (PI / 180.0));
            var angleRz = (angleZ * (PI / 180.0));


            var rotateZ = DenseMatrix.OfArray(new[,] {
                                                    { Cos(angleRz), Sin(angleRz), 0, 0},
                                                    {-Sin(angleRz), Cos(angleRz), 0, 0},
                                                    { 0, 0, 1, 0},
                                                    { 0, 0, 0, 1 }
            });

            var rotateX = DenseMatrix.OfArray(new[,] {
                                                    { 1, 0, 0, 0 },
                                                    { 0, Cos(angleRx), Sin(angleRx), 0 },
                                                    { 0, -Sin(angleRx), Cos(angleRx), 0 },
                                                    { 0, 0, 0, 1 }
            });

            var rotateY = DenseMatrix.OfArray(new[,] {
                                                    { Cos(angleRy), 0, -Sin(angleRy), 0 },
                                                    { 0, 1, 0, 0 },
                                                    { Sin(angleRy),0, Cos(angleRy),0 },
                                                    { 0, 0, 0, 1 }
            });

            return Transformation(facets, (rotateX * rotateY * rotateZ));
        }

        /// <summary>
        /// Move transformation
        /// </summary>
        /// <param name="facets">collection facets</param>
        /// <param name="moveX"></param>
        /// <param name="moveY"></param>
        /// <param name="moveZ"></param>
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

        #endregion

        #region Shadow

        public IEnumerable<IFacet> ShadowFacets(IEnumerable<IFacet> facets, IEnumerable<double> ligthVector)
        {
            var enumerable = ligthVector as IList<double> ?? ligthVector.ToList();
            var moveMatrix = DenseMatrix.OfArray(new[,] {
                                                    { -enumerable[2], 0, 0, 0},
                                                    { 0, -enumerable[2], 0, 0},
                                                    { enumerable[0], enumerable[1], 0, 1},
                                                    { 0, 0, 0, -enumerable[2]}
                                                  });

            return Transformation(facets, moveMatrix);
        }

        public IEnumerable<IFacet> GlobalShadowFacets(IEnumerable<IFacet> facets, IEnumerable<double> ligthVector)
        {
            var enumerable = ligthVector as IList<double> ?? ligthVector.ToList();
            var moveMatrix = DenseMatrix.OfArray(new[,] {
                                                    { 1, 0, 0, 0},
                                                    { 0, 1, 0, 0},
                                                    { -enumerable[0]/enumerable[2], -enumerable[1]/enumerable[2], 0, 0},
                                                    { 0, 0, 0, 1}
                                                  });

            return Transformation(facets, moveMatrix);
        }
        
        #endregion

        #region Parallel Projection

        public IEnumerable<IFacet> ProjectionZ(IEnumerable<IFacet> facets)
        {
            var projection = DenseMatrix.OfArray(new double[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1 }
            });

            return Transformation(facets, projection);
        }

        public IEnumerable<IFacet> ProjectionX(IEnumerable<IFacet> facets)
        {
            var projection = DenseMatrix.OfArray(new double[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1 }
            });

            return Transformation(facets, projection);
        }

        public IEnumerable<IFacet> ProjectionY(IEnumerable<IFacet> facets)
        {
            var projection = DenseMatrix.OfArray(new double[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1 }
            });
            return Transformation(facets, projection);
        }

        #endregion

        #region Projection

        public IEnumerable<IFacet> OrthogonalProjection(IEnumerable<IFacet> facets, double psi, double fi)
        {
            var anglePsi = RadiansFromAngle(psi);
            var angleFi = RadiansFromAngle(fi);

            var projection = DenseMatrix.OfArray(new[,] {
                { Cos(anglePsi), Sin(anglePsi)*Sin(angleFi), 0, 0},
                { 0, Cos(angleFi), 0, 0},
                { Sin(anglePsi), -Sin(angleFi)*Cos(anglePsi), 1, 0},
                { 0, 0, 0, 1 }
            });

            return Transformation(facets, projection);
        }

        public IEnumerable<IFacet> ObliqueProjection(IEnumerable<IFacet> facets, double alpha, double l)
        {
            var angleAlpha = RadiansFromAngle(alpha);

            var projection = DenseMatrix.OfArray(new[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { -l*Cos(angleAlpha), -l*Sin(angleAlpha), 1, 0},
                { 0, 0, 0, 1 }
            });

            return Transformation(facets, projection);
        }

        public IEnumerable<IFacet> ViewTransformation(IEnumerable<IFacet> facets, double fi, double teta, double ro, double distance)
        {
            var fiAngle = RadiansFromAngle(fi);
            var tetaAngle = RadiansFromAngle(teta);

            var vMatrix = DenseMatrix.OfArray(new[,] {
                { -Sin(tetaAngle), -Cos(fiAngle)*Cos(tetaAngle), -Sin(fiAngle)*Cos(tetaAngle), 0},
                { Cos(tetaAngle), -Cos(fiAngle)*Sin(tetaAngle), -Sin(fiAngle)*Sin(tetaAngle), 0},
                { 0, Sin(fiAngle), -Cos(fiAngle), 0},
                { 0, 0, ro, 1 }
            });

            return CentralProjection(Transformation(facets, vMatrix), distance);
        }

        public IEnumerable<IFacet> CentralProjection(IEnumerable<IFacet> facets, double distance)
        {
            var centralProjection = facets as IList<IFacet> ?? facets.ToList();
            foreach (var item in centralProjection)
            {
                foreach (var arris in item.ArristCollection)
                {
                    arris.FirstVertex = CentralPoints(arris.FirstVertex, distance);
                    arris.SecondVertex = CentralPoints(arris.SecondVertex, distance);

                }
            }
            return centralProjection;
        }

        private static IVertex CentralPoints(IVertex point, double distance)
        {
            var vertex = new Vertex();
            var param = 0.1f;
            vertex.Z = Abs(point.Z) <= 0.1 ? param : point.Z;
            vertex.X = point.X * distance / vertex.Z;
            vertex.Y = point.Y * distance / vertex.Z;
            vertex.Z = distance;
            return vertex;
        }

        #endregion

        private IEnumerable<IFacet> Transformation(IEnumerable<IFacet> facets, DenseMatrix matrix)
        {
            var transformation = facets as IList<IFacet> ?? facets.ToList();

            foreach (var item in transformation)
            {
                foreach (var arris in item.ArristCollection)
                {
                    arris.FirstVertex = PointOutVector(Vector(arris.FirstVertex) * matrix);
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

        private static double RadiansFromAngle(double angle)
        {
            return (angle * (PI / 180.0));
        }
    }
}
