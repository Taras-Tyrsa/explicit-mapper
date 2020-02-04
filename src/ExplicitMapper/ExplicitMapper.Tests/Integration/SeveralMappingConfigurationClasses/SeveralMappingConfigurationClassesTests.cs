using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ExplicitMapper.Tests.Integration.SeveralMappingConfigurationClasses
{
    [Collection("Integration tests")]
    [Trait("Integration", "Several mapping confguration classes")]
    public class SeveralMappingConfigurationClassesTests : IDisposable
    {
        [Fact(DisplayName = "Use three mapping configuration classes")]
        public void SeveralMappingConfigurationClasses_FieldsMapped()
        {
            MappingConfiguration.Add<Configuration1>();
            MappingConfiguration.Add<Configuration2>();
            MappingConfiguration.Add<Configuration3>();
            MappingConfiguration.Build();

            var product = new Product()
            {
                Manufacturer = "Company",
                Name = "Good product",
                Size = new Size()
                {
                    Width = 1,
                    Height = 2,
                    Depth = 3
                }
            };

            var productViewModel = Mapper.Map<ProductViewModel>(product);

            productViewModel.Should().NotBeNull();
            productViewModel.Description.Should().Be("Company - Good product");
            productViewModel.Size.Should().NotBeNull();
            productViewModel.Size.Width.Should().Be("1m");
            productViewModel.Size.Height.Should().Be("2m");
            productViewModel.Size.Depth.Should().Be("3m");

            var x = new X()
            {
                X1 = 1,
                X2 = 2
            };

            var y = Mapper.Map<Y>(x);

            y.Should().NotBeNull();
            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
