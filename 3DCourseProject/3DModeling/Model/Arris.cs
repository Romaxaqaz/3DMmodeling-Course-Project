using System;

namespace _3DModeling.Model
{
    [Serializable]
    public class Arris : IArris
    {
        public int NumberFacet { get; set; }
        public IVertex FirstVertex { get; set; }
        public IVertex SecondVertex { get; set; }
    }
}
