using FluentAssertions;
using System;
using Xunit;

namespace ExplicitMapper.Tests.Integration.NestedClassMapping.ObjectInitializerConfiguration
{
    [Collection("Integration tests")]
    [Trait("Integration", "Nested class mapping")]
    public class NestedClassMappingTests : IDisposable
    {
        [Fact(DisplayName = "Use object initializer configuration")]
        public void UseObjectInitializerConfiguration_FieldsMapped()
        {
            MappingConfiguration.Add<ProductToProductViewModelObjectInitializerConfiguration>();
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
