using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using _3DModeling.Figure;
using _3DModeling.Model;
using _3DModeling.Transformation;

namespace _3DModeling.Drawing
{
    public class LightFace
    {
        public double Ia { get; set; } = 25;
        public double Ka { get; set; } = 1;
        public double Il { get; set; } = 230;
        public double Kd { get; set; } = 1;

        public LightFace()
        {
            
        }

        public LightFace(double ia, double ka, double il, double kd)
        {
            Ia = ia;
            Ka = ka;
            Il = il;
            Kd = kd;
        }        

        public SolidColorBrush RgbLigth(SolidColorBrush color, IFacet face, IEnumerable<double> viewVector, bool kostil = false)
        {
            var enumerable = viewVector as IList<double> ?? viewVector.ToList();
            var cos = FaceParameter.GetCos(FaceParameter.GetNormal(face),
                      FaceParameter.GetViewVector(new Vertex() { X = enumerable[0],
                                                                 Y = enumerable[1],
                                                                 Z = enumerable[2]
                                                               },
                                                  FaceParameter.GetCenter(face)));
            //if (face.NameFigure == nameof(Parallelepiped) && kostil)
            //{
            //    if (enumerable[1] == 5000)
            //    {
            //        return new SolidColorBrush(Color.FromRgb(0, 0, 0));
            //    }
            //}
            double light = Ia * Ka + Il * Kd * cos;
            light = light < 0 ? 0 : light > 255 ? 255 : light;
            if (enumerable[2] == 0)
            {
                light = 255 - light;
            }
            var res = light / 255;
            double r = color.Color.R * res;
            double g = color.Color.G * res;
            double b = color.Color.B * res;
            var newColor = new SolidColorBrush(Color.FromRgb((byte) r,
                    (byte)g,
                    (byte)b));
            return newColor;
        }

        private static byte NewIntensityColor(byte color, double light)
        {
            var factor = light/255.0;
            var resColor = factor*color;
            return (byte)resColor;
        }
    }
}
