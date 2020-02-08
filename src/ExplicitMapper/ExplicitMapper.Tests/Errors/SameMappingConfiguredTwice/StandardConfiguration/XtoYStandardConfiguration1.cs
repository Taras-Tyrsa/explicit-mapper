using ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice;

namespace ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice.StandardConfiguration
{
    public class XtoYStandardConfiguration1 : MappingConfiguration
    {
        public XtoYStandardConfiguration1()
        {
            CreateMap<X, Y>()
                .For(y => y.Y1, x => x.X1)
                .For(y => y.Y2, x => x.X2);
        }
    }
}
