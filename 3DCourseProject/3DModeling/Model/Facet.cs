using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Xml.Serialization;

namespace _3DModeling.Model
{
    [Serializable]
    public class Facet : IFacet
    {
        public int FacetNumber { get; set; }
        public string NameFigure { get; set; }
        public bool IsHidden { get; set; }
        public bool ReverseNormal { get; set; }
        public int Light { get; set; }

        [NonSerialized]
        private SolidColorBrush _faceColor = new SolidColorBrush(Colors.Aqua);

        [XmlIgnore]
        public SolidColorBrush FaceColor
        {
            get { return _faceColor; }
            set { _faceColor = value; }
        }
        public IEnumerable<double> Normal { get; set; }
        public IEnumerable<double> Center { get; set; }
        public IEnumerable<double> ViewVector { get; set; }
        public IEnumerable<IArris> ArristCollection { get; set; }
    }
}
