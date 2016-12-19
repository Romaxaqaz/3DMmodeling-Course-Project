using _3DModeling.Enums;

namespace _3DModeling.Model
{
    public interface IVertex
    {
        int Number { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
        PointsType PointType { get; set; }
    }
}
