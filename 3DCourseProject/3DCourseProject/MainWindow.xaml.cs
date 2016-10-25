using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using _3DCourseProject.ViewModel;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace _3DCourseProject
{
    public partial class MainWindow : Window
    {
        private readonly MainPageViewModel _viewModel = new MainPageViewModel();
        private bool move = false;
        private Point pointm = new Point();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            SizeChanged += MainWindow_SizeChanged;
            Loaded += MainWindow_Loaded;
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DrawAxis();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _viewModel.ActualizeSizeWindows(canGraph.ActualWidth/2, canGraph.ActualHeight/2);
            canGraph.Children.Clear();
            DrawAxis();
        }

        private void MainCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed || e.MiddleButton == MouseButtonState.Pressed)
            {

                pointm = e.GetPosition((Canvas)sender);
                move = true;
            }

        }

        private void MainCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (move && e.LeftButton== MouseButtonState.Pressed)
            {
                var dx = Math.Abs(e.GetPosition((Canvas)sender).X - pointm.X);
                var dy = Math.Abs(e.GetPosition((Canvas)sender).Y - pointm.Y);

                var angleRadians = Math.Atan2(dx, dy);
                var angleDegrees= (angleRadians * 180) / Math.PI;
                _viewModel.MouseRotate(angleDegrees/2, angleDegrees/2);
                
            }
            else if(e.MiddleButton == MouseButtonState.Pressed)
            {
                var dx = (e.GetPosition((Canvas)sender).X - pointm.X)/10;
                var dy = (e.GetPosition((Canvas)sender).Y - pointm.Y)/10;
                _viewModel.MouseMove(dx, dy);
            }
        }

        private void MainCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            move = false;
        }

        private void MainCanvas_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _viewModel.MouseScale(e.Delta > 0 ? 0.5 : 1.5);
            _viewModel.CreateBitmap(MainCanvas);
        }

        private void ShowPanel_OnChecked(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleButton;
            if ((bool) toggle.IsChecked)
            {
                Storyboard sb = this.FindResource("ButtomGridNotView") as Storyboard;
                sb.Begin();
                ShowToggleTextBlock.Text = "Показать панель действий";
            }
            else
            {
                Storyboard sb = this.FindResource("ButtomGridView") as Storyboard;
                sb.Begin();
                ShowToggleTextBlock.Text = "Спрятать панель действий";
            }
        }

        private void DrawAxis()
        {
            const double margin = 10;
            double xmin = 10;
            double xmax = canGraph.ActualWidth/2;
            double ymin = canGraph.ActualHeight/2;
            double ymax = canGraph.ActualHeight/2;
            const double step = 50;

            // Make the X axis.
            GeometryGroup xaxis_geom = new GeometryGroup();
            xaxis_geom.Children.Add(new LineGeometry(
                new Point(0, ymax), new Point(canGraph.ActualWidth, ymax)));
            for (double x = xmin + step;
                x <= canGraph.ActualWidth - step; x += step)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, ymax - margin / 2),
                    new Point(x, ymax + margin / 2)));
            }

            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 0.5;
            xaxis_path.Stroke = Brushes.Black;
            xaxis_path.Data = xaxis_geom;

            canGraph.Children.Add(xaxis_path);


            GeometryGroup yaxis_geom = new GeometryGroup();
            yaxis_geom.Children.Add(new LineGeometry(
                new Point(xmax, 0), new Point(xmax, canGraph.ActualHeight)));
            for (double y = step; y <= canGraph.ActualHeight - step; y += step)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmax - margin / 2, y),
                    new Point(xmax + margin / 2, y)));
            }

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 0.5;
            yaxis_path.Stroke = Brushes.Black;
            yaxis_path.Data = yaxis_geom;

            canGraph.Children.Add(yaxis_path);


        }

        private void AxixView_OnChecked(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleButton;
            if ((bool)toggle.IsChecked)
            {
                AxisToggleTextBlock.Text = "Показать оси";
                canGraph.Children.Clear();
            }
            else
            {
                AxisToggleTextBlock.Text = "Убрать оси";
                DrawAxis();
            }
        }
    }
}
