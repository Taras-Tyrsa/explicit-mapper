using FluentAssertions;
using System;
using Xunit;

namespace ExplicitMapper.Tests.Integration.SimpleClassMapping
{
    [Collection("Integration tests")]
    public class SimpleClassMappingTests : IDisposable
    {
        [Fact]
        public void FieldsMapped()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var x = new X()
            {
                X1 = 1,
                X2 = 2
            };

            var y = Mapper.Map<Y>(x);

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2.ToString());
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
