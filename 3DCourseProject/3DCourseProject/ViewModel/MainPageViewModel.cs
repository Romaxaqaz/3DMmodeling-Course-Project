using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using _3DCourseProject.Common;
using _3DCourseProject.Extensions;
using _3DModeling;
using _3DModeling.Abstract;
using _3DModeling.Algorithms;
using _3DModeling.Drawing;
using _3DModeling.Figure;
using _3DModeling.Model;
using _3DModeling.Transformation;

namespace _3DCourseProject.ViewModel
{
    internal class MainPageViewModel : BindableBase
    {
        #region Fields

        private readonly _3DTransformation _transformation = new _3DTransformation();
        private LightFace _lightFace = new LightFace();
        private ObservableCollection<UIElement> _uiElementsCollection = new ObservableCollection<UIElement>();
        private IEnumerable<IFacet> _resultTransformationFacets = new List<IFacet>();
        public List<double> LightViewVector = new List<double>() { 0, 0, 5000 };
        private Action _drawTheFirst;
        private Action _projectionTransform;
        private bool viewRotate = false;

        #endregion

        #region Constructor

        public MainPageViewModel()
        {
            WindowSize.Width = CenterX;
            WindowSize.Heigth = CenterY;
            DrawFigure = new DelegateCommand(DrawFacet);
            ClearCanvas = new DelegateCommand(ClearAllValue);
            CenterParallelepiped = new DelegateCommand(AutoParallelepipedCenter);
            TransformationCommand = new DelegateCommand<object>(TransformationStart);
            ProectionY = new DelegateCommand(ProectioYStart);
            ProectionX = new DelegateCommand(ProectioXStart);
            ProectionZ = new DelegateCommand(ProectioZStart);
            OrtogonalCommand = new DelegateCommand(OrtogonalProjection);
            ObliqueCommand = new DelegateCommand(ObliqueProjection);
            ViewProjectionCommand = new DelegateCommand(ViewProjection);
            CentralPCommand = new DelegateCommand(CentralProjection);
            SetLightParam = new DelegateCommand(SetLightParams);
        }

        #endregion

        #region Methods

        private void DrawFacet()
        {
            PreviewPathVisibility = false;

            _drawTheFirst = () =>
            {
                Detail detail = new Parallelepiped(null, ParallelepipedWidth, CylinderHeigth, ParallelepipedLength, -75, 0);
                detail = new Cylinder(detail, CylinderRadius, CylinderHeigth, ApproksimationValue, -75, 0);

                var facetsList = detail.FacetCollection().ToList();
                var resultDetail = DetailsCombine.DoubleDetailFacet(detail);
                facetsList.AddRange(resultDetail);
                _resultTransformationFacets = (IEnumerable<IFacet>)facetsList.DeepClone();
                Transform(_resultTransformationFacets);
            };
            _drawTheFirst();
        }

        private void TransformationStart(object parameter)
        {
            if (parameter == null) return;
            var transformationType = (TransformationType)Enum.Parse(typeof(TransformationType), parameter.ToString(), true);

            switch (transformationType)
            {
                case TransformationType.Move:
                    Transform(_transformation.GetMoveFacets(_resultTransformationFacets, MoveX, MoveY, MoveZ));
                    break;
                case TransformationType.Scale:
                    Transform(_transformation.GetScaleFacets(_resultTransformationFacets, ScaleX, ScaleY, ScaleZ));
                    break;
                case TransformationType.Rotate:
                    viewRotate = false;
                        Transform(_transformation.GetRotateFacets(_resultTransformationFacets, RotateX, RotateY, RotateZ));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Transform(IEnumerable<IFacet> facets, IEnumerable<double> lightVecor = null)
        {
            UiElementsCollection = null;
            var enumerable = facets as IList<IFacet> ?? facets.ToList();
            var roberts = new RobertsAlgorithm();
            var res = roberts.HideLines(enumerable, new Vertex(1000, 0, 0, 5000)).ToList();
            var resultFacet = res.Where(x => x.IsHidden != true).ToObservableCollection();
            // shadow facets
            var uiCollection = ShadowFasets(enumerable).ToObservableCollection();
            // detail faces
            uiCollection.InsertRange(FillFasets(resultFacet, lightVecor));
            UiElementsCollection = uiCollection;

            #region Draw Normal and Central points

            //for (int i = 0; i < res.Count - 1; i++)
            //{
            //    var collection = res[i].Normal.ToList();
            //    var arr = res[i].ArristCollection.ToList();
            //    UiElementsCollection.Add(CreateLine(collection[0], collection[1] + 100, arr[0].FirstVertex.X + CenterX, arr[0].FirstVertex.Y + CenterY, ""));
            //}
            //for (int i = 0; i < res.Count - 1; i++)
            //{
            //    var item = res[i].Center.ToList();
            //    UiElementsCollection.Add(CreateEllipse(item[0] + CenterX - 5, item[1] + CenterY - 5));
            //}

            #endregion
        }

        private IEnumerable<UIElement> ShadowFasets(IEnumerable<IFacet> facets)
        {
            var uiCollection = new ObservableCollection<UIElement>();
            if (!IsShadow) return uiCollection;
            var clone = (IEnumerable<IFacet>)facets.DeepClone();
            var shadowFaces = IsNanPointLight
                ? _transformation.ShadowFacets(clone, LightViewVector)
                : _transformation.GlobalShadowFacets(clone, LightViewVector);
            foreach (var item in shadowFaces)
            {
                var arrises = item.ArristCollection.ToList();
                var listPoint = arrises.Select(arris =>
                                new Point(arris.FirstVertex.X + CenterX, arris.FirstVertex.Y + CenterY)).ToList();

                var detailPath = new Path
                {
                    Fill = Brushes.Black,
                    Stroke = Brushes.Black,
                    Data = new CombinedGeometry(new PathGeometry(new[]
                                                 {
                                                    new PathFigure(listPoint[listPoint.Count - 1],
                                                    new[] {new PolyLineSegment(listPoint, false)}, true)
                                                 }), null)
                };
                uiCollection.Add(detailPath);
            }
            return uiCollection;
        }

        private IEnumerable<UIElement> FillFasets(IEnumerable<IFacet> facets, IEnumerable<double> lightVecor = null)
        {
            var uiCollection = new ObservableCollection<UIElement>();
            foreach (var item in facets)
            {
                var arrises = item.ArristCollection.ToList();
                var listPoint = arrises.Select(arris =>
                                               new Point(arris.FirstVertex.X + CenterX, arris.FirstVertex.Y + CenterY)).ToList();
                var hColor = IsBulb ? _lightFace.RgbLigth(MainColor, item, lightVecor ?? LightViewVector) : MainColor;
                item.FaceColor = hColor;
                var detailPath = new Path
                {
                    Fill = IsFaceDraw ? hColor ?? Brushes.SteelBlue : Brushes.Transparent,
                    Stroke = IsVertexDraw ? Brushes.Black : hColor,
                    Data = new CombinedGeometry(new PathGeometry(new[]
                                                {
                                                    new PathFigure(listPoint[listPoint.Count - 1],
                                                    new[]
                                                    {
                                                        new PolyLineSegment(listPoint, false)
                                                    }, true)
                                                }), null)
                };
                uiCollection.Add(detailPath);
            }
            return uiCollection;
        }

        private IEnumerable<UIElement> FillDefault(IEnumerable<IFacet> facets)
        {
            var uiCollection = new ObservableCollection<UIElement>();
            foreach (var item in facets)
            {
                var arrises = item.ArristCollection.ToList();
                var listPoint = arrises.Select(arris =>
                                               new Point(arris.FirstVertex.X + CenterX, arris.FirstVertex.Y + CenterY)).ToList();
                var detailPath = new Path
                {
                    Fill = item.FaceColor ?? Brushes.Black,
                    Stroke = Brushes.Black,
                    Data = new CombinedGeometry(new PathGeometry(new[]
                                                {
                                                    new PathFigure(listPoint[listPoint.Count - 1],
                                                    new[]
                                                    {
                                                        new PolyLineSegment(listPoint, false)
                                                    }, true)
                                                }), null)
                };
                uiCollection.Add(detailPath);
            }
            return uiCollection;
        }

        #region Mouse events

        public void MouseRotate(double x, double y)
        {
            if (viewRotate)
            {
                FiView = x * 100;
                Teta = y * 100;
                ViewProjection();
            }
            else
            {
                Transform(_transformation.GetRotateFacets(_resultTransformationFacets, x, y, 0));
            }

        }

        public void MouseMove(double x, double y) => Transform(_transformation.GetMoveFacets(_resultTransformationFacets, x, y, 0));

        public void MouseScale(double scaleFactor) => Transform(_transformation.GetScaleFacets(_resultTransformationFacets, scaleFactor, scaleFactor, scaleFactor));

        #endregion

        #region Additional methods

        private void AutoParallelepipedCenter()
        {
            CenterParallelepipedX = WindowSize.Width - (ParallelepipedWidth / 2);
            CenterParallelepipedY = WindowSize.Heigth - (ParallelepipedLength / 2);
        }

        private void ClearAllValue() => UiElementsCollection.Clear();

        private void SetLightParams() => _lightFace = new LightFace(Ia, Ka, Il, Kd);

        private void SetLightViewVector()
        {
            LightViewVector.Clear();
            LightViewVector.Add(LightVectorX);
            LightViewVector.Add(LightVectorY);
            LightViewVector.Add(LightVectorZ);
            Transform(_resultTransformationFacets);
            _projectionTransform();
        }

        private static Ellipse CreateEllipse(double x1, double y1)
        {
            var line = new Ellipse()
            {
                Fill = new SolidColorBrush(Color.FromRgb(154, 152, 0)),
                Width = 10,
                Height = 10
            };
            Canvas.SetTop(line, y1);
            Canvas.SetLeft(line, x1);
            return line;
        }

        private static Line CreateLine(double x1, double y1, double x2, double y2, string name)
        {
            var line = new Line
            {
                Stroke = name == nameof(Cylinder) ? new SolidColorBrush(Color.FromRgb(0, 0, 154)) : new SolidColorBrush(Color.FromRgb(154, 0, 0)),
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
            return line;
        }

        #endregion

        #region Parallel projection

        private void ProectioZStart()
        {
            var result = _transformation.ProjectionZ(_resultTransformationFacets);
            Transform(result.DrawXz());
        }

        private void ProectioXStart()
        {
            var result = _transformation.ProjectionX(_resultTransformationFacets);
            UiElementsCollection = FillDefault(result.DrawXy()).ToObservableCollection();
        }

        private void ProectioYStart()
        {
            var result = _transformation.ProjectionY(_resultTransformationFacets);
            UiElementsCollection = FillDefault(result.DrawYz()).ToObservableCollection();
        }

        private IEnumerable<IFacet> FillFaces(IEnumerable<IFacet> faces)
        {
            foreach (var item in faces)
            {
                item.FaceColor = IsBulb ? _lightFace.RgbLigth(MainColor, item, LightViewVector) : MainColor;
            }
            return faces;
        }

        private void DrawPArallelProjection(IEnumerable<IFacet> faces)
        {
            var uiCollection = new ObservableCollection<UIElement>();
            foreach (var item in faces)
            {
                var arrises = item.ArristCollection.ToList();

                var listPoint = arrises.Select(arris => new Point(arris.FirstVertex.X + CenterX, arris.FirstVertex.Y + CenterY)).ToList();

                var detailPath = new Path
                {
                    Fill = item.FaceColor,
                    Stroke = item.FaceColor,
                    Data = new CombinedGeometry(
                        new PathGeometry(new[]
                        {
                            new PathFigure(listPoint[listPoint.Count - 1], new[] {new PolyLineSegment(listPoint, false)}, true)
                        }), null)
                };
                uiCollection.Add(detailPath);
            }
            UiElementsCollection = uiCollection;
        }

        public void ActualizeSizeWindows(double x, double y)
        {
            CenterX = x;
            CenterY = y;
            WindowSize.Width = x;
            WindowSize.Heigth = y;
        }

        #endregion

        #region Projections

        private void CentralProjection()
        {
            viewRotate = false;
            _drawTheFirst();
            Transform(_transformation.GetRotateFacets(_resultTransformationFacets, RotateX, RotateY, RotateZ));
            Transform(_transformation.GetMoveFacets(_resultTransformationFacets, MoveX, MoveY, MoveZ));
            Transform(_transformation.CentralProjection(_resultTransformationFacets, CentralDistance));
        }

        private void ViewProjection()
        {
            viewRotate = true;
            _drawTheFirst();
            _projectionTransform = () =>
            {
                var coll = (IEnumerable<IFacet>)_resultTransformationFacets.DeepClone();
                var rotate = _transformation.GetRotateFacets(coll, FiView, Teta, 0);

                foreach (var item in rotate)
                {
                    var arrises = item.ArristCollection.ToList();
                    item.FaceColor = IsBulb
                        ? _lightFace.RgbLigth(MainColor, item, new List<double> { LightViewVector[1], -LightViewVector[0], -LightViewVector[2] }, Ro > 0 && LightViewVector[2] > 0)
                        : MainColor;
                }
                var resColl = _transformation.ViewTransformation(rotate, 0, 0, Ro, Distance);

                var roberts = new RobertsAlgorithm();
                var res =
                    roberts.HideLines(resColl,
                            new Vertex(1000, 0, 0, Ro > 0 ? -Math.Abs(LightViewVector[2]) : Math.Abs(LightViewVector[2])))
                        .ToList();
                var resultFacet = res.Where(x => x.IsHidden != true).ToObservableCollection();

                var uiCollection = new ObservableCollection<UIElement>();
                foreach (var item in resultFacet)
                {
                    var arrises = item.ArristCollection.ToList();
                    var listPoint = arrises.Select(arris => new Point(arris.FirstVertex.X + CenterX, arris.FirstVertex.Y + CenterY)).ToList();
                    var detailPath = new Path
                    {
                        Fill = item.FaceColor,
                        Stroke = item.FaceColor,
                        Data = new CombinedGeometry(new PathGeometry( new []
                        {
                            new PathFigure(listPoint[listPoint.Count - 1],
                                new []
                                {
                                    new PolyLineSegment(listPoint, false)
                                }, true)
                        }), null)
                    };
                    uiCollection.Add(detailPath);
                }
                UiElementsCollection = uiCollection;
            };
            _projectionTransform();
        }

        private void ObliqueProjection()
        {
            viewRotate = false;
            _drawTheFirst();
            Transform(_transformation.ObliqueProjection(_resultTransformationFacets, Alpha, L));
        }

        private void OrtogonalProjection()
        {
            viewRotate = false;
            _drawTheFirst();
            Transform(_transformation.OrthogonalProjection(_resultTransformationFacets, Psi, Fi));
        }

        //private void ViewProjection()
        //{
        //        viewRotate = true;
        //            _drawTheFirst();
        //        _projectionTransform = () =>
        //            {
        //                var coll = (IEnumerable<IFacet>)_resultTransformationFacets.DeepClone();
        //                foreach (var item in coll)
        //                {
        //                    var arrises = item.ArristCollection.ToList();
        //        item.FaceColor = IsBulb
        //            ? _lightFace.RgbLigth(MainColor, item, new List<double> {0, LightViewVector[2], 0},
        //                            Ro > 0 && LightViewVector[2] > 0)
        //                        : MainColor;
        //                }
        //var resColl = _transformation.ViewTransformation(coll, FiView, Teta, Ro, Distance);

        //var roberts = new RobertsAlgorithm();
        //var res =
        //    roberts.HideLines(resColl,
        //            new Vertex(1000, 0, 0, Ro > 0 ? -Math.Abs(LightViewVector[2]) : Math.Abs(LightViewVector[2])))
        //        .ToList();
        //var resultFacet = res.Where(x => x.IsHidden != true).ToObservableCollection();

        //var uiCollection = new ObservableCollection<UIElement>();
        //                foreach (var item in resultFacet)
        //                {
        //                    var arrises = item.ArristCollection.ToList();
        //var listPoint = arrises.Select(arris =>
        //        new Point(arris.FirstVertex.X + CenterX, arris.FirstVertex.Y + CenterY)).ToList();
        //var detailPath = new Path
        //{
        //    Fill = item.FaceColor,
        //    Stroke = item.FaceColor,
        //    Data = new CombinedGeometry(new PathGeometry(new[]
        //    {
        //                            new PathFigure(listPoint[listPoint.Count - 1],
        //                                new[]
        //                                {
        //                                    new PolyLineSegment(listPoint, false)
        //                                }, true)
        //                        }), null)
        //};
        //uiCollection.Add(detailPath);
        //                }
        //                UiElementsCollection = uiCollection;
        //            };
        //            _projectionTransform();
        //}
        #endregion

        #endregion

        #region Property


        private string _lightType = "ИС в бесконечности";
        public string LightType
        {
            get { return _lightType; }
            set { Set(ref _lightType, value); }
        }



        private bool _isNanPointLight;
        public bool IsNanPointLight
        {
            get { return _isNanPointLight; }
            set
            {
                Set(ref _isNanPointLight, value);
                if ((bool)value)
                {
                    LightType = "ИС в бесконечности";
                }
                else
                {
                    LightType = "ИС в конечной точке";
                }
            }
        }

        private bool _isShadow;
        public bool IsShadow
        {
            get { return _isShadow; }
            set { Set(ref _isShadow, value); }
        }

        private bool _alwaysDeledeHiddenLines;
        public bool AlwaysDeledeHiddenLines
        {
            get { return _alwaysDeledeHiddenLines; }
            set { Set(ref _alwaysDeledeHiddenLines, value); }
        }

        private double _psi;
        public double Psi
        {
            get { return _psi; }
            set { Set(ref _psi, value); }
        }

        private double _fi;
        public double Fi
        {
            get { return _fi; }
            set { Set(ref _fi, value); }
        }

        private double _alpha;
        public double Alpha
        {
            get { return _alpha; }
            set { Set(ref _alpha, value); }
        }

        private double _l;
        public double L
        {
            get { return _l; }
            set { Set(ref _l, value); }
        }


        private double _fiView;
        public double FiView
        {
            get { return _fiView; }
            set { Set(ref _fiView, value); }
        }

        private double _teta;
        public double Teta
        {
            get { return _teta; }
            set { Set(ref _teta, value); }
        }

        private double _ro;
        public double Ro
        {
            get { return _ro; }
            set { Set(ref _ro, value); }
        }

        private double _distance;
        public double Distance
        {
            get { return _distance; }
            set
            {
                Set(ref _distance, value);
            }
        }

        private double _centralDistance;
        public double CentralDistance
        {
            get { return _centralDistance; }
            set { Set(ref _centralDistance, value); }
        }


        #region StartParams

        private double _centerX = 300;
        public double CenterX
        {
            get { return _centerX; }
            set { Set(ref _centerX, value); }
        }

        private double _centerY = 200;
        public double CenterY
        {
            get { return _centerY; }
            set { Set(ref _centerY, value); }
        }

        private double _approksimationValue = 10;
        public double ApproksimationValue
        {
            get { return _approksimationValue; }
            set { Set(ref _approksimationValue, value); }
        }

        private double _cylinderRadius = 60;
        public double CylinderRadius
        {
            get { return _cylinderRadius; }
            set { Set(ref _cylinderRadius, value); }
        }

        private double _cylinderHeigth = 150;
        public double CylinderHeigth
        {
            get { return _cylinderHeigth; }
            set { Set(ref _cylinderHeigth, value); }
        }

        private double _centerParallelepipedX = 50;
        public double CenterParallelepipedX
        {
            get { return _centerParallelepipedX; }
            set { Set(ref _centerParallelepipedX, value); }
        }

        private double _centerParallelepipedY = 50;
        public double CenterParallelepipedY
        {
            get { return _centerParallelepipedY; }
            set { Set(ref _centerParallelepipedY, value); }
        }

        private double _parallelepipedWidth = 40;
        public double ParallelepipedWidth
        {
            get { return _parallelepipedWidth; }
            set { Set(ref _parallelepipedWidth, value); }
        }

        private double _parallelepipedLength = 40;
        public double ParallelepipedLength
        {
            get { return _parallelepipedLength; }
            set { Set(ref _parallelepipedLength, value); }
        }

        public ObservableCollection<UIElement> UiElementsCollection
        {
            get { return _uiElementsCollection; }
            set { Set(ref _uiElementsCollection, value); }
        }


        private bool _previewPathVisibility;
        public bool PreviewPathVisibility
        {
            get { return _previewPathVisibility; }
            set { Set(ref _previewPathVisibility, value); }
        }
        #endregion

        #region TransformationProperty
        // Move property
        private double _moveX;
        public double MoveX
        {
            get { return _moveX; }
            set { Set(ref _moveX, value); }
        }

        private double _moveY;
        public double MoveY
        {
            get { return _moveY; }
            set { Set(ref _moveY, value); }
        }

        private double _moveZ;
        public double MoveZ
        {
            get { return _moveZ; }
            set { Set(ref _moveZ, value); }
        }
        //Scale property
        private double _scaleX = 1;
        public double ScaleX
        {
            get { return _scaleX; }
            set { Set(ref _scaleX, value); }
        }

        private double _scaleY = 1;
        public double ScaleY
        {
            get { return _scaleY; }
            set { Set(ref _scaleY, value); }
        }

        private double _scaleZ = 1;
        public double ScaleZ
        {
            get { return _scaleZ; }
            set { Set(ref _scaleZ, value); }
        }
        //Rotate property
        private double _rotateX;
        public double RotateX
        {
            get { return _rotateX; }
            set { Set(ref _rotateX, value); }
        }

        private double _rotateY;
        public double RotateY
        {
            get { return _rotateY; }
            set { Set(ref _rotateY, value); }
        }

        private double _rotateZ;
        public double RotateZ
        {
            get { return _rotateZ; }
            set { Set(ref _rotateZ, value); }
        }

        private TransformationType _transType = TransformationType.Move;
        public TransformationType TransType
        {
            get { return _transType; }
            set { Set(ref _transType, value); }
        }

        #endregion

        #region AlgorithmIndex

        private bool _robertsOrPainterAlgorithm;
        public bool RobertsOrPainterAlgorithm
        {
            get
            {
                return _robertsOrPainterAlgorithm;
            }
            set
            {
                Set(ref _robertsOrPainterAlgorithm, value);
            }
        }

        #endregion

        #region LightParams

        private double _la = 127;
        public double Ia
        {
            get { return _la; }
            set { Set(ref _la, value); }
        }

        private double _ka = 1;
        public double Ka
        {
            get { return _ka; }
            set { Set(ref _ka, value); }
        }

        private double _il = 128;
        public double Il
        {
            get { return _il; }
            set { Set(ref _il, value); }
        }

        private double _kd = 1;
        public double Kd
        {
            get { return _kd; }
            set { Set(ref _kd, value); }
        }

        private bool _isBulb = true;
        public bool IsBulb
        {
            get { return _isBulb; }
            set { Set(ref _isBulb, value); }
        }

        private double _lightVectorX;
        public double LightVectorX
        {
            get { return _lightVectorX; }
            set
            {
                Set(ref _lightVectorX, value);
                SetLightViewVector();
            }
        }

        private double _lightVectorY;
        public double LightVectorY
        {
            get { return _lightVectorY; }
            set
            {
                Set(ref _lightVectorY, value);
                SetLightViewVector();
            }
        }

        private double _lightVectorZ = 5000;
        public double LightVectorZ
        {
            get { return _lightVectorZ; }
            set
            {
                Set(ref _lightVectorZ, value);
                SetLightViewVector();
            }
        }


        #endregion

        #region Draw property

        private bool _isVertexDraw = true;
        public bool IsVertexDraw
        {
            get { return _isVertexDraw; }
            set { Set(ref _isVertexDraw, value); }
        }

        private bool _isFaceDraw = true;
        public bool IsFaceDraw
        {
            get { return _isFaceDraw; }
            set { Set(ref _isFaceDraw, value); }
        }

        private SolidColorBrush _mainColor = Brushes.Blue;
        public SolidColorBrush MainColor
        {
            get { return _mainColor; }
            set { Set(ref _mainColor, value); }
        }


        #endregion

        #endregion

        #region Commands

        public DelegateCommand StartInitialization { get; private set; }
        public DelegateCommand CenterParallelepiped { get; private set; }
        public DelegateCommand DrawFigure { get; private set; }
        public DelegateCommand ClearCanvas { get; private set; }

        public DelegateCommand<object> TransformationCommand { get; private set; }

        public DelegateCommand ProectionY { get; private set; }
        public DelegateCommand ProectionX { get; private set; }
        public DelegateCommand ProectionZ { get; private set; }

        public DelegateCommand OrtogonalCommand { get; private set; }
        public DelegateCommand ObliqueCommand { get; private set; }
        public DelegateCommand CentralPCommand { get; private set; }

        public DelegateCommand ViewProjectionCommand { get; private set; }
        public DelegateCommand PainterDeleteLines { get; private set; }

        public DelegateCommand SetLightParam { get; private set; }
        #endregion

        //var cosAlp = FaceParameter.GetCos(new double[] { 0, -1000, 1000 }, new double[] { 0, 0, 1000 });
        //var intens = 1 * 1 * Math.Pow(Math.Cos(cosAlp), 2);

        //        for (int i = (int)listPoint[0].X; i< (int)listPoint[1].X; i++)
        //        {
        //            for (int j = (int)listPoint[0].Y; j< (int)listPoint[3].Y; j++)
        //            {
        //                Rectangle rec = new Rectangle();
        //Canvas.SetTop(rec, j);
        //                Canvas.SetLeft(rec, i);
        //                rec.Width = 1;
        //                rec.Height = 1;
        //                rec.Fill = hColor;
        //                uiCollection.Add(rec);
        //            }
    }
}

