namespace ExplicitMapper.Tests.Configuration.SeveralMappingConfigurationClasses
{
    class Configuration3 : MappingConfiguration
    {
        public Configuration3()
        {
            CreateMap<X, Y>()
                .For(y => y.Y1, x => x.X1)
                .For(y => y.Y2, x => x.X2);
        }
    }
}
