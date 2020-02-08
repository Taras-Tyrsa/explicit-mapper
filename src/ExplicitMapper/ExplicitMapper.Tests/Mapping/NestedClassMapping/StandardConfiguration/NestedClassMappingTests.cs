using FluentAssertions;
using System;
using Xunit;

namespace ExplicitMapper.Tests.Mapping.NestedClassMapping.StandardConfiguration
{
    [Collection("Integration tests")]
    [Trait("Mapping", "Nested class mapping")]
    public class NestedClassMappingTests : IDisposable
    {
        [Fact(DisplayName = "Use standard configuration")]
        public void UseStandardConfiguration_FieldsMapped()
        {
            MappingConfiguration.Add<ProductToProductViewModelStandardConfiguration>();
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
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
