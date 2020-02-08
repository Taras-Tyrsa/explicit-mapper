using ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice;

namespace ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice.ObjectInitializerConfiguration
{
    public class XtoYObjectInitializerConfiguration1 : MappingConfiguration
    {
        public XtoYObjectInitializerConfiguration1()
        {
            CreateMap<X, Y>(
                x => new Y()
                {
                    Y1 = x.X1,
                    Y2 = x.X2
                });
        }
    }
}
