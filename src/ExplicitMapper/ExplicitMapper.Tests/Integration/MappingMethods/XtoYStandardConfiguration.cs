namespace ExplicitMapper.Tests.Integration.MappingMethods
{
    class XtoYConfiguration : MappingConfiguration
    {
        public XtoYConfiguration()
        {
            CreateMap<X, Y>()
                .For(y => y.Y1, x => x.X1)
                .For(y => y.Y2, x => x.X2);
        }
    }
}
