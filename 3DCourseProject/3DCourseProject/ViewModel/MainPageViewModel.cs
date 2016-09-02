using _3DCourseProject.Common;

namespace _3DCourseProject.ViewModel
{
    internal class MainPageViewModel : BindableBase
    {
        private double _cylinderRadius = 100;
        public double CylinderRadius
        {
            get { return _cylinderRadius = 100; }
            set { Set(ref _cylinderRadius, value); }
        }

        private double _cylinderHeigth;
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
    }
}
