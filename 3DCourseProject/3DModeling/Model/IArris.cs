namespace _3DModeling.Model
{
    public interface IArris
    {
        int NumberFacet { get; set; }
        IVertex FirstVertex { get; set; }
        IVertex SecondVertex { get; set; }
    }
}
