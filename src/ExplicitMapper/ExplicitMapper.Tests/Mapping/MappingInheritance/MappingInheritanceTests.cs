using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ExplicitMapper.Tests.Mapping.MappingInheritance
{
    [Collection("Integration tests")]
    [Trait("Mapping", "Mapping inheritance")]
    public class MappingInheritanceTests : IDisposable
    {
        [Fact(DisplayName = "mapping inheritance hierarchy X to hierarchy Y")]
        public void MapHierarchyToHierarchy()
        {
            MappingConfiguration.Add<Configuration>();
            MappingConfiguration.Build();

            var x = new ChildX2()
            {
                X1 = 1,
                X2 = 2,
                X3 = 3,
                X4 = 4,
                X5 = 5,
                X6 = 6
            };

            var y = Mapper.Map<ChildY2>(x);

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
            y.Y3.Should().Be(x.X3);
            y.Y4.Should().Be(x.X4);
            y.Y5.Should().Be(x.X5);
            y.Y6.Should().Be(x.X6);
            y.Y.Should().Be(x.X5 + x.X6);
        }

        [Fact(DisplayName = "mapping inheritance hierarchy X to single class Z")]
        public void MapHierarchyToSingleClass()
        {
            MappingConfiguration.Add<Configuration>();
            MappingConfiguration.Build();

            var x = new ChildX2()
            {
                X1 = 1,
                X2 = 2,
                X3 = 3,
                X4 = 4,
                X5 = 5,
                X6 = 6
            };

            var z = Mapper.Map<Z>(x);

            z.Z1.Should().Be(x.X1);
            z.Z2.Should().Be(x.X2);
            z.Z3.Should().Be(x.X3);
            z.Z4.Should().Be(x.X4);
            z.Z5.Should().Be(x.X5);
            z.Z6.Should().Be(x.X6);
            z.ZZ.Should().Be(x.X5 + x.X6);
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
