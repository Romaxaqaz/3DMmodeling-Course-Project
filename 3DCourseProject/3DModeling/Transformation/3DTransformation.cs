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


        #region Parallel Projection

        public IEnumerable<IFacet> ProjectionZ(IEnumerable<IFacet> facets)
        {
            var projection = DenseMatrix.OfArray(new double[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, 0, 0},
                { 0, 0, 0, 1 }
            });

            return Transformation(facets, projection);
        }

        public IEnumerable<IFacet> ProjectionX(IEnumerable<IFacet> facets)
        {
            var projection = DenseMatrix.OfArray(new double[,] {
                { 0, 0, 0, 0},
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
                { 0, 0, 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1 }
            });
            return Transformation(facets, projection);
        }

        #endregion


        public IEnumerable<IFacet> OrthogonalProjection(IEnumerable<IFacet> facets, double psi, double fi)
        {
            var anglePsi = RadiansFromAngle(psi);
            var angleFi = RadiansFromAngle(fi);

            var projection = DenseMatrix.OfArray(new double[,] {
                { Math.Cos(anglePsi), Math.Sin(anglePsi)*Math.Sin(angleFi), 0, 0},
                { 0, Math.Cos(angleFi), 0, 0},
                { Math.Sin(anglePsi), -Math.Sin(angleFi)*Math.Cos(anglePsi), 0, 0},
                { 0, 0, 0, 1 }
            });

            return Transformation(facets, projection);
        }

        public IEnumerable<IFacet> ObliqueProjection(IEnumerable<IFacet> facets, double alpha, double l)
        {
            var angleAlpha = RadiansFromAngle(alpha);

            var projection = DenseMatrix.OfArray(new double[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { l*Math.Cos(angleAlpha), l*Math.Sin(angleAlpha), 0, 0},
                { 0, 0, 0, 1 }
            });

            return Transformation(facets, projection);
        }

        public IEnumerable<IFacet> ViewTransformation(IEnumerable<IFacet> facets, double fi, double teta, double ro, double distance)
        {
            var fiAngle = RadiansFromAngle(fi);
            var tetaAngle = RadiansFromAngle(teta);

            var xE = (float)(ro * Math.Sin(fiAngle) * Math.Cos(tetaAngle));
            var yE = (float)(ro * Math.Sin(fiAngle) * Math.Sin(tetaAngle));
            var zE = (float)(ro * Math.Cos(fiAngle));

            var tMatrix = DenseMatrix.OfArray(new double[,] {
                { 1, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, 1, 0},
                { -xE, -yE, -zE, 1 }
            });

            var rzMatrix = DenseMatrix.OfArray(new double[,] {
                { Math.Cos(Math.PI/2 - tetaAngle), Math.Sin(Math.PI/2 - tetaAngle), 0, 0},
                { -Math.Sin(Math.PI/2 - tetaAngle), Math.Cos(Math.PI/2 - tetaAngle), 0, 0},
                { 0, 0, 1, 0},
                { 0, 0, 0, 1 }
            });

            var rxMatrix = DenseMatrix.OfArray(new double[,] {
                { 1, 0, 0, 0},
                { 0, Math.Cos(fiAngle - Math.PI), Math.Sin(fiAngle - Math.PI), 0},
                { 0, -Math.Sin(fiAngle - Math.PI), Math.Cos(fiAngle - Math.PI), 0},
                { 0, 0, 0, 1 }
            });

            var sMatrix = DenseMatrix.OfArray(new double[,] {
                { 1.0f, 0, 0, 0},
                { 0, 1, 0, 0},
                { 0, 0, -1, 0},
                { 0, 0, 0, 1 }
            });

            var vMatrix = DenseMatrix.OfArray(new double[,] {
                { -Math.Sin(tetaAngle), -Math.Cos(fiAngle)*Math.Cos(tetaAngle), -Math.Sin(fiAngle)*Math.Cos(tetaAngle), 0},
                { Math.Cos(tetaAngle), -Math.Cos(fiAngle)*Math.Sin(tetaAngle), -Math.Sin(fiAngle)*Math.Sin(tetaAngle), 0},
                { 0, Math.Sin(fiAngle), -Math.Cos(fiAngle), 0},
                { 0, 0, ro, 1 }
            });
            //var vMatrix = tMatrix * rzMatrix * rxMatrix * sMatrix;

            return CentralProjection(Transformation(facets, vMatrix), distance);
        }

        public IEnumerable<IFacet> CentralProjection(IEnumerable<IFacet> facets, double distance)
        {

            foreach (var item in facets)
            {
                foreach (var arris in item.ArristCollection)
                {
                    arris.FirstVertex = CentralPoints(arris.FirstVertex, distance);
                    arris.SecondVertex = CentralPoints(arris.SecondVertex, distance);


                }
            }
            return facets;
        }

        private IVertex CentralPoints(IVertex point, double distance)
        {
            var point1 = new Vertex();
            var param = 0.1f;
            point1.Z = Math.Abs(point.Z) <= 0.1f ? param : point.Z;
            point1.X = point.X * distance / point1.Z;
            point1.Y = point.Y * distance / point1.Z;
            point1.Z = distance;
            return point1;
        }


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
            return (angle * (Math.PI / 180.0));
        }
    }
}
