using System.Collections.Generic;
using System.Windows.Media;

namespace _3DModeling.Model
{
    public interface IFacet
    {
        int FacetNumber { get; set; }
        string NameFigure { get; set; }
        int Light { get; set; }
        bool IsHidden { get; set; }
        SolidColorBrush FaceColor { get; set; }
        bool ReverseNormal { get; set; }
        IEnumerable<double> Normal { get; set; }
        IEnumerable<double> Center { get; set; }
        IEnumerable<double> ViewVector { get; set; }
        IEnumerable<IArris> ArristCollection { get; set; }
    }
}
