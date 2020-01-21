namespace ExplicitMapper.Tests.Integration.SimpleClassMapping
{
    public class X
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
    }

    public class Y
    {
        public int Y1 { get; set; }
        public string Y2 { get; set; }
    }

    class XtoYConfiguration : MappingConfiguration
    {
        public XtoYConfiguration()
        {
            CreateMap<X, Y>()
                .For(y => y.Y1, x => x.X1)
                .For(y => y.Y2, x => x.X2.ToString());
        }
    }
}
