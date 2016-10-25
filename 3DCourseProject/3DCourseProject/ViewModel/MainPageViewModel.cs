using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private ObservableCollection<UIElement> _uiElementsCollection = new ObservableCollection<UIElement>();
        private IEnumerable<IFacet> _resultTransformationFacets = new List<IFacet>();

        #endregion

        #region Constructor
        public MainPageViewModel()
        {

            DrawFigure = new DelegateCommand(DrawFacet);
            ClearCanvas = new DelegateCommand(ClearAllValue);
            PreviewFigure = new DelegateCommand(ShowPreviewFigure);
            CenterParallelepiped = new DelegateCommand(AutoParallelepipedCenter);
            TransformationCommand = new DelegateCommand<object>(TransformationStart);

            WindowSize.Width = CenterX;
            WindowSize.Heigth = CenterY;

            ProectionY = new DelegateCommand(ProectioYStart);
            ProectionX = new DelegateCommand(ProectioXStart);
            ProectionZ = new DelegateCommand(ProectioZStart);

            OrtogonalCommand = new DelegateCommand(OrtogonalProjection);
            ObliqueCommand = new DelegateCommand(ObliqueProjection);
            ViewProjectionCommand = new DelegateCommand(ViewProjection);
            CentralPCommand = new DelegateCommand(CentralProjection);

            PainterDeleteLines = new DelegateCommand(DeleteLinePainter);
        }
        #endregion

        #region Methods

        private void DrawFacet()
        {
            PreviewPathVisibility = false;

            Detail detail = null;
            detail = new Cylinder(detail, CylinderRadius, CylinderHeigth, ApproksimationValue, 0, 0);
            detail = new Parallelepiped(detail, ParallelepipedWidth, CylinderHeigth, ParallelepipedLength, 0, 0);

            _resultTransformationFacets = detail.FacetCollection();       
            Transform(_transformation.GetMoveFacets(_resultTransformationFacets, 0, 0, 0));
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
                    Transform(_transformation.GetRotateFacets(_resultTransformationFacets, RotateX, RotateY, RotateZ));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DeleteLinePainter()
        {
            var painter = new PainterAlgorithm();
            var facets = painter.ComptletedFacet(_resultTransformationFacets);

            var uiCollection = new ObservableCollection<UIElement>();
            foreach (var item in facets)
            {
                var array = item.ArristCollection.ToList();
                // Create a polyline
                var yellowPolyline = new Polyline();
                yellowPolyline.Stroke = Brushes.Black;
                yellowPolyline.StrokeThickness = 1;
                yellowPolyline.Fill = Brushes.Green;

                var polygonPoints = new PointCollection();
                foreach (var t in array)
                {
                    var p = new Point(t.FirstVertex.X + CenterX, t.FirstVertex.Y + CenterY);
                    polygonPoints.Add(p);
                }

                yellowPolyline.Points = polygonPoints;
                uiCollection.Add(yellowPolyline);
            }
            UiElementsCollection = uiCollection;
        }

        public void MouseRotate(double x, double y)
        {
            Transform(_transformation.GetRotateFacets(_resultTransformationFacets, x, y, 0));
        }

        public void MouseMove(double x, double y)
        {
            Transform(_transformation.GetMoveFacets(_resultTransformationFacets, x, y, 0));
        }

        public void MouseScale(double scaleFactor)
        {
            Transform(_transformation.GetScaleFacets(_resultTransformationFacets, scaleFactor, scaleFactor, scaleFactor));
        }

        private void Transform(IEnumerable<IFacet> facets)
        {
            _resultTransformationFacets = facets;

            var newColl = (IEnumerable<IFacet>)_resultTransformationFacets.DeepClone();

            foreach (var item in newColl)
            {
                foreach (var arris in item.ArristCollection)
                {
                    arris.FirstVertex.X+=CenterX;
                    arris.SecondVertex.X+=CenterX;

                    arris.FirstVertex.Y += CenterY;
                    arris.SecondVertex.Y += CenterY;

                }
            }

            UiElementsCollection = (ObservableCollection<UIElement>)DrawingFaces.DrawFacet(newColl);
            if(AlwaysDeledeHiddenLines)
                DeleteLinePainter();
        }  

        private void AutoParallelepipedCenter()
        {
            CenterParallelepipedX = WindowSize.Width - (ParallelepipedWidth / 2);
            CenterParallelepipedY = WindowSize.Heigth - (ParallelepipedLength / 2);
        }

        private void ShowPreviewFigure()
        {
            //PreviewPathVisibility = true;

            //HoleRectanglePreview = new HoleRectangle()
            //{
            //    X = CenterParallelepipedX,
            //    Y = CenterParallelepipedY,
            //    Width = _parallelepipedWidth,
            //    Heigth = _parallelepipedLength,
            //    Z = 0
            //};
            Transform(_transformation.GetMoveFacets(_resultTransformationFacets, 0, 0, 0));
            var canvas = new Canvas();
            foreach (var item in UiElementsCollection)
            {
                canvas.Children.Add(item);
            }

            var sss = SaveAsWriteableBitmap(canvas);

        }

        public void CreateBitmap(Canvas canvas)
        {
            //var sss = SaveAsWriteableBitmap(canvas);
            //WriteableBitmap writeableBmp = sss;
            //var a = writeableBmp.GetPixel(345, 666);
        }

        public WriteableBitmap SaveAsWriteableBitmap(Canvas surface)
        {
            if (surface == null) return null;

            // Save current canvas transform
            Transform transform = surface.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(surface.ActualWidth, surface.ActualHeight);
            // Measure and arrange the surface
            // VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
              (int)size.Width,
              (int)size.Height,
              96d,
              96d,
              PixelFormats.Pbgra32);
            renderBitmap.Render(surface);


            //Restore previously saved layout
            surface.LayoutTransform = transform;

            //create and return a new WriteableBitmap using the RenderTargetBitmap
            return new WriteableBitmap(renderBitmap);

        }

        private void ClearAllValue()
        {
            UiElementsCollection.Clear();
        }

        #region Parallel projection

        private void ProectioZStart()
        {
            var ss = (IEnumerable<IFacet>)_resultTransformationFacets.DeepClone();
            var result = _transformation.ProjectionZ(ss);
            UiElementsCollection = (ObservableCollection<UIElement>)DrawingFaces.DrawFacet(result.DrawXZ());
        }

        private void ProectioXStart()
        {
            var ss = (IEnumerable<IFacet>)_resultTransformationFacets.DeepClone();
            var result = _transformation.ProjectionX(ss);
            UiElementsCollection = (ObservableCollection<UIElement>)DrawingFaces.DrawFacet(result.DrawXY());
        }

        private void ProectioYStart()
        {
            var ss = (IEnumerable<IFacet>)_resultTransformationFacets.DeepClone();
            var result = _transformation.ProjectionY(ss);
            UiElementsCollection = (ObservableCollection<UIElement>)DrawingFaces.DrawFacet(result.DrawYZ());
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
            Transform(_transformation.CentralProjection(_resultTransformationFacets, CentralDistance));
        }

        private void ViewProjection()
        {
            Transform(_transformation.ViewTransformation(_resultTransformationFacets, FiView, Teta, Ro, Distance));
        }

        private void ObliqueProjection()
        {
            Transform(_transformation.ObliqueProjection(_resultTransformationFacets, Alpha, L));
        }

        private void OrtogonalProjection()
        {
            Transform(_transformation.OrthogonalProjection(_resultTransformationFacets, Psi, Fi));
        }

        #endregion
        #endregion

        #region Property

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

        private double _cylinderRadius = 100;
        public double CylinderRadius
        {
            get { return _cylinderRadius; }
            set { Set(ref _cylinderRadius, value); }
        }

        private double _cylinderHeigth = 300;
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

        private double _parallelepipedWidth = 60;
        public double ParallelepipedWidth
        {
            get { return _parallelepipedWidth; }
            set { Set(ref _parallelepipedWidth, value); }
        }

        private double _parallelepipedLength = 60;
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

        public HoleRectangle HoleRectanglePreview
        {
            get { return _holeRectangle; }
            set { Set(ref _holeRectangle, value); }
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
        private HoleRectangle _holeRectangle;

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

        #endregion

        #region Commands
        public DelegateCommand StartInitialization { get; private set; }
        public DelegateCommand PreviewFigure { get; private set; }
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
        #endregion
    }
}
