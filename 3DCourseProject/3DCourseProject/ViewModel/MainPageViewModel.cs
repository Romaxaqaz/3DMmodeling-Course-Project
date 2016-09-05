using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using _3DCourseProject.Common;
using _3DModeling;
using _3DModeling.Abstract;
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
        }
        #endregion

        #region Methods
        /// <summary>
        /// Draws facets figures
        /// </summary>
        private void DrawFacet()
        {
            PreviewPathVisibility = false;

            Detail detail = null;
            detail = new Cylinder(detail, CylinderRadius, CylinderHeigth, ApproksimationValue, CenterX, CenterY);
            detail = new Parallelepiped(detail, ParallelepipedWidth, CylinderHeigth, ParallelepipedLength, CenterX, CenterY);

            _resultTransformationFacets = detail.FacetCollection();
            UiElementsCollection = (ObservableCollection<UIElement>)DrawingFaces.DrawFacet(_resultTransformationFacets);
        }

        /// <summary>
        /// Start transformation
        /// </summary>
        /// <param name="parameter">Type transformation: Move, Scale, Rotate</param>
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

        /// <summary>
        /// Draws figure after transformation
        /// </summary>
        /// <param name="facets"></param>
        private void Transform(IList<IFacet> facets)
        {
            _resultTransformationFacets = facets;
            UiElementsCollection = (ObservableCollection<UIElement>)DrawingFaces.DrawFacet(_resultTransformationFacets);
        }

        private void AutoParallelepipedCenter()
        {
            CenterParallelepipedX = CenterX - (ParallelepipedWidth / 2);
            CenterParallelepipedY = CenterY - (ParallelepipedLength / 2);
        }

        private void ShowPreviewFigure()
        {
            PreviewPathVisibility = true;

            HoleRectanglePreview = new HoleRectangle()
            {
                X = CenterParallelepipedX,
                Y = CenterParallelepipedY,
                Width = _parallelepipedWidth,
                Heigth = _parallelepipedLength,
                Z = 0
            };
        }

        private void ClearAllValue()
        {
            UiElementsCollection.Clear();
        }
        #endregion

        #region Property

        #region StartParams

        private double _centerX = 100;
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
        private double _scaleX;
        public double ScaleX
        {
            get { return _scaleX; }
            set { Set(ref _scaleX, value); }
        }

        private double _scaleY;
        public double ScaleY
        {
            get { return _scaleY; }
            set { Set(ref _scaleY, value); }
        }

        private double _scaleZ;
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
        #endregion

        #endregion

        #region Commands
        public DelegateCommand StartInitialization { get; private set; }
        public DelegateCommand PreviewFigure { get; private set; }
        public DelegateCommand CenterParallelepiped { get; private set; }
        public DelegateCommand DrawFigure { get; private set; }
        public DelegateCommand ClearCanvas { get; private set; }

        public DelegateCommand<object> TransformationCommand { get; private set; }
        #endregion
    }
}
