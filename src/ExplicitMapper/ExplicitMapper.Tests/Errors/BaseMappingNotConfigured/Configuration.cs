namespace ExplicitMapper.Tests.Errors.BaseMappingNotConfigured
{
    class Configuration : MappingConfiguration
    {
        public Configuration()
        {
            CreateMap<BaseX, BaseY>()
                .For(y => y.Y, x => x.X1 + x.X2)
                .For(y => y.Y1, x => x.X1)
                .For(y => y.Y2, x => x.X2);

            CreateMap<ChildX2, ChildY2>()
                .Inherits<ChildX1, ChildY1>()
                .For(y => y.Y, x => x.X5 + x.X6)
                .For(y => y.Y5, x => x.X5)
                .For(y => y.Y6, x => x.X6);
        }
    }
}
