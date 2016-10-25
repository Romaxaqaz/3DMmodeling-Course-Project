using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using _3DCourseProject.Extensions;
using _3DModeling;
using _3DModeling.Drawing;
using _3DModeling.Model;
using _3DModeling.Transformation;
using System.Windows.Controls;

namespace _3DCourseProject.UserControls
{
    /// <summary>
    /// Interaction logic for CustomCanvas.xaml
    /// </summary>
    public partial class CustomCanvas : UserControl
    {


        private IEnumerable<IFacet> _resultTransformationFacets = new List<IFacet>();
        private readonly _3DTransformation _transformation = new _3DTransformation();
        Point currentPoint = new Point();

        public static readonly DependencyProperty ParamOneProperty =
               DependencyProperty.Register("ParamOne", typeof(double), typeof(CustomCanvas),
                   new PropertyMetadata(default(Double)));

        public static readonly DependencyProperty ParamTwoProperty =
               DependencyProperty.Register("ParamTwo", typeof(double), typeof(CustomCanvas),
                   new PropertyMetadata(default(Double)));

        public static readonly DependencyProperty ParamThreeProperty =
               DependencyProperty.Register("ParamThree", typeof(double), typeof(CustomCanvas),
                   new PropertyMetadata(default(Double)));

        public static readonly DependencyProperty UIElementsCollectionProperty =
              DependencyProperty.Register("UIElementsCollection", typeof(ObservableCollection<UIElement>), typeof(CustomCanvas),
                  new PropertyMetadata(default(ObservableCollection<UIElement>)));

        public static readonly DependencyProperty TransformationTypeProperty =
              DependencyProperty.Register("TransformationType", typeof(Enum), typeof(CustomCanvas),
                  new PropertyMetadata(default(Enum)));



        public double ParamOne
        {
            get { return (double)GetValue(ParamOneProperty); }
            set { SetValue(ParamOneProperty, value); }
        }

        public double ParamTwo
        {
            get { return (double)GetValue(ParamTwoProperty); }
            set { SetValue(ParamTwoProperty, value); }
        }

        public double ParamThree
        {
            get { return (double)GetValue(ParamThreeProperty); }
            set { SetValue(ParamThreeProperty, value); }
        }

        public ObservableCollection<UIElement> UIElementsCollection
        {
            get { return (ObservableCollection<UIElement>)GetValue(UIElementsCollectionProperty); }
            set { SetValue(UIElementsCollectionProperty, value); }
        }

        public Enum TransformationType
        {
            get { return (Enum)GetValue(TransformationTypeProperty); }
            set { SetValue(TransformationTypeProperty, value); }
        }


        private void TransformationStart()
        {
            var transformationType = (TransformationType)TransformationType;

            switch (transformationType)
            {
                case _3DModeling.TransformationType.Move:
                    Transform(_transformation.GetMoveFacets(_resultTransformationFacets, ParamOne, ParamTwo, ParamThree));
                    break;
                case _3DModeling.TransformationType.Scale:
                    Transform(_transformation.GetScaleFacets(_resultTransformationFacets, ParamOne, ParamTwo, ParamThree));
                    break;
                case _3DModeling.TransformationType.Rotate:
                    Transform(_transformation.GetRotateFacets(_resultTransformationFacets, ParamOne, ParamTwo, ParamThree));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Transform(IEnumerable<IFacet> facets)
        {

            var newColl = (IEnumerable<IFacet>)UIElementsCollection.DeepClone();

            foreach (var item in newColl)
            {
                foreach (var arris in item.ArristCollection)
                {
                    arris.FirstVertex.X += 300;
                    arris.SecondVertex.X += 300;

                    arris.FirstVertex.Y += 300;
                    arris.SecondVertex.Y += 300;

                }
            }

            UIElementsCollection = (ObservableCollection<UIElement>)DrawingFaces.DrawFacet(newColl);

            foreach (var item in UIElementsCollection)
            {
                MainCanvas.Children.Add(item);
            }
        }


        public CustomCanvas()
        {
            InitializeComponent();
        }

        private void MainCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(this);
        }

        private void MainCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                
                ParamOne = e.GetPosition(this).X;
                ParamTwo = e.GetPosition(this).Y;

                TransformationStart();
            }
        }
    }
}
