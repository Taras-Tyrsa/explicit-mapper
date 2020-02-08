namespace ExplicitMapper.Tests.Mapping.SimpleClassMapping.StandardConfiguration
{
    class XtoYStandardConfiguration : MappingConfiguration
    {
        public XtoYStandardConfiguration()
        {
            CreateMap<X, Y>()
                .For(y => y.Y1, x => x.X1)
                .For(y => y.Y2, x => x.X2.ToString())
                .For(y => y.Y3, x => int.Parse(x.X3) + x.X2);
        }
    }
}
