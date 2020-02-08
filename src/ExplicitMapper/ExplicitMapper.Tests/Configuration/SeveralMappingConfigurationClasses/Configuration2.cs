namespace ExplicitMapper.Tests.Configuration.SeveralMappingConfigurationClasses
{
    class Configuration2 : MappingConfiguration
    {
        public Configuration2()
        {
            CreateMap<Size, SizeViewModel>(
                s => new SizeViewModel()
                {
                    Width = s.Width.ToString() + "m",
                    Height = s.Height.ToString() + "m",
                    Depth = s.Depth.ToString() + "m"
                });
        }
    }
}
