using ExplicitMapper.Tests.Integration.SimpleClassMapping.StandardConfiguration;
using FluentAssertions;
using System;
using Xunit;

namespace ExplicitMapper.Tests.Integration.SimpleClassMapping.ObjectInitializerConfiguration
{
    [Collection("Integration tests")]
    public class SimpleClassMappingTests : IDisposable
    {
        [Fact]
        public void UseObjectInitializerConfiguration_FieldsMapped()
        {
            MappingConfiguration.Add<XtoYObjectInitializerConfiguration>();
            MappingConfiguration.Build();

            var x = new X()
            {
                X1 = 1,
                X2 = 2,
                X3 = "3"
            };

            var y = Mapper.Map<Y>(x);

            y.Y1.Should().Be(1);
            y.Y2.Should().Be("2");
            y.Y3.Should().Be(5);
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
