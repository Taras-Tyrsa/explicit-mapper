namespace ExplicitMapper.Tests.Mapping.SimpleClassMapping.ObjectInitializerConfiguration
{
    class XtoYObjectInitializerConfiguration : MappingConfiguration
    {
        public XtoYObjectInitializerConfiguration()
        {
            CreateMap<X, Y>(
                x => new Y()
                {
                    Y1 = x.X1,
                    Y2 = x.X2.ToString(),
                    Y3 = int.Parse(x.X3) + x.X2
                });
        }
    }
}
