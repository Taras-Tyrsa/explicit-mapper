using System.Collections.Generic;

namespace ExplicitMapper.Benchmark.NestedCollectionsMapping
{
    class ExplicitMapperConfiguration : MappingConfiguration
    {
        public ExplicitMapperConfiguration()
        {
            CreateMap<X, Y>(x =>
                new Y()
                {
                    ListP = Map<List<NestedY>>(x.ListP),
                    ArrayP = Map<NestedY[]>(x.ArrayP)
                });

            CreateMap<NestedX, NestedY>(x =>
                new NestedY()
                {
                    P1 = x.P1,
                    P2 = x.P2
                });
        }
    }
}
