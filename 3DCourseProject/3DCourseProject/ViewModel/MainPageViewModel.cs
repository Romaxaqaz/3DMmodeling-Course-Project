using System.Collections.ObjectModel;
using System.Windows;
using _3DCourseProject.Common;
using _3DModeling.Figure;
using _3DModeling.Transformation;

namespace _3DCourseProject.ViewModel
{
    internal class MainPageViewModel : BindableBase
    {
        private Cylinder _cylinder;
        private readonly _3DTransformation _transformation = new _3DTransformation();
        private ObservableCollection<UIElement> _uiElementsCollection = new ObservableCollection<UIElement>();

        public MainPageViewModel()
        {
            DrawFigure = new DelegateCommand(DrawFacet);
            ClearCanvas = new DelegateCommand(ClearAllValue);
        }

        private void DrawFacet()
        {
            _cylinder = new Cylinder(CylinderRadius, CylinderHeigth, ApproksimationValue);
            _cylinder.StartInitialization();
            UiElementsCollection = (ObservableCollection<UIElement>) _cylinder.DrawFacet(_cylinder.FacetsList);
        }

        private void ClearAllValue()
        {
            _cylinder.Clear();
        }

        #region Property

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

        private double _parallelepipedWidth;
        public double ParallelepipedWidth
        {
            get { return _parallelepipedWidth; }
            set { Set(ref _parallelepipedWidth, value); }
        }

        private double _parallelepipedLength;
        public double ParallelepipedLength
        {
            get { return _parallelepipedLength; }
            set { Set(ref _parallelepipedLength, value); }
        }

        public ObservableCollection<UIElement> UiElementsCollection
        {
            get { return _uiElementsCollection; }
            set { Set(ref _uiElementsCollection, value);}
        }
        #endregion

        #region Commands
        public DelegateCommand StartInitialization { get; private set; }
        public DelegateCommand DrawFigure { get; private set; }
        public DelegateCommand ClearCanvas { get; private set; }
        #endregion
    }
}
