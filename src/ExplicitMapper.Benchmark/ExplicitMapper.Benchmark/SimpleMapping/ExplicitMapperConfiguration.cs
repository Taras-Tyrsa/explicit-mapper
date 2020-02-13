namespace ExplicitMapper.Benchmark.SimpleMapping
{
    class ExplicitMapperConfiguration : MappingConfiguration
    {
        public ExplicitMapperConfiguration()
        {
            CreateMap<X, Y>(x =>
                new Y()
                {
                    P1 = x.P1,
                    P2 = x.P2,
                    P3 = x.P3,
                    P4 = x.P4,
                    P5 = x.P5,
                    P6 = x.P6,
                    P7 = x.P7,
                    P8 = x.P8
                });
        }
    }
}
