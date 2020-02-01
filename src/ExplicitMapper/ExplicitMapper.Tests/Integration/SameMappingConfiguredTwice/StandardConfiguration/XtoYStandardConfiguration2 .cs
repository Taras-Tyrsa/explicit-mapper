namespace ExplicitMapper.Tests.Integration.SameMappingConfiguredTwice.StandardConfiguration
{
    public class XtoYStandardConfiguration2 : MappingConfiguration
    {
        public XtoYStandardConfiguration2()
        {
            CreateMap<X, Y>()
                .For(y => y.Y1, x => x.X1);
        }
    }
}
