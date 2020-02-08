using ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice;

namespace ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice.ObjectInitializerConfiguration
{
    public class XtoYObjectInitializerConfiguration2 : MappingConfiguration
    {
        public XtoYObjectInitializerConfiguration2()
        {
            CreateMap<X, Y>(
                x => new Y()
                {
                    Y1 = x.X1,
                });
        }
    }
}
