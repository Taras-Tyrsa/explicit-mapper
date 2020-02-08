namespace ExplicitMapper.Tests.Mapping.MappingInheritance
{
    class Configuration : MappingConfiguration
    {
        public Configuration()
        {
            CreateMap<BaseX, BaseY>()
                .For(y => y.Y, x => x.X1 + x.X2)
                .For(y => y.Y1, x => x.X1)
                .For(y => y.Y2, x => x.X2);

            CreateMap<ChildX1, ChildY1>()
                .Inherits<BaseX, BaseY>()
                .For(y => y.Y, x => x.X3 + x.X4)
                .For(y => y.Y3, x => x.X3)
                .For(y => y.Y4, x => x.X4);

            CreateMap<ChildX2, ChildY2>()
                .Inherits<ChildX1, ChildY1>()
                .For(y => y.Y, x => x.X5 + x.X6)
                .For(y => y.Y5, x => x.X5)
                .For(y => y.Y6, x => x.X6);

            CreateMap<BaseX, Z>(x =>
                new Z()
                {
                    ZZ = x.X1 + x.X2,
                    Z1 = x.X1,
                    Z2 = x.X2
                });

            CreateMap<ChildX1, Z>()
                .Inherits<BaseX, Z>()
                .For(z => z.ZZ, x => x.X3 + x.X4)
                .For(z => z.Z3, x => x.X3)
                .For(z => z.Z4, x => x.X4);

            CreateMap<ChildX2, Z>(x =>
                new Z()
                {
                    ZZ = x.X5 + x.X6,
                    Z5 = x.X5,
                    Z6 = x.X6
                })
                .Inherits<ChildX1, Z>();
        }
    }
}
