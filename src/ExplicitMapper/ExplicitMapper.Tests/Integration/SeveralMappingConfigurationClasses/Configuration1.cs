namespace ExplicitMapper.Tests.Integration.SeveralMappingConfigurationClasses
{
    class Configuration1 : MappingConfiguration
    {
        public Configuration1()
        {
            CreateMap<Product, ProductViewModel>()
                .For(d => d.Description, s => s.Manufacturer + " - " + s.Name)
                .For(d => d.Size, s => Map<SizeViewModel>(s.Size));
        }
    }
}
