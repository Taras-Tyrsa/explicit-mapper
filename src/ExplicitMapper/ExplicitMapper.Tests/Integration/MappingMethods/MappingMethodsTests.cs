using FluentAssertions;
using System;
using Xunit;

namespace ExplicitMapper.Tests.Integration.MappingMethods
{
    [Collection("Integration tests")]
    [Trait("Integration", "Mapping methods")]
    public class MappingMethodsTests : IDisposable
    {
        [Fact(DisplayName = "TDest Map<TDest>(object source)")]
        public void MapMethod1()
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
            y.Y2.Should().Be(x.X2);
        }

        [Fact(DisplayName = "TDest Map<TSource, TDest>(TSource source)")]
        public void MapMethod2()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var x = new X()
            {
                X1 = 1,
                X2 = 2
            };

            var y = Mapper.Map<X, Y>(x);

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
        }

        [Fact(DisplayName = "object Map(object source, Type sourceType, Type destType)")]
        public void MapMethod3()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var x = new X()
            {
                X1 = 1,
                X2 = 2
            };

            var y = (Y) Mapper.Map(x, typeof(X), typeof(Y));

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
        }

        [Fact(DisplayName = "void Map<TSource, TDest>(TSource source, TDest dest)")]
        public void MapMethod4()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var x = new X()
            {
                X1 = 1,
                X2 = 2
            };

            var y = new Y();
            Mapper.Map(x, y);

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
        }

        [Fact(DisplayName = "void Map(object source, object dest, Type sourceType, Type destType)")]
        public void MapMethod5()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var x = new X()
            {
                X1 = 1,
                X2 = 2
            };

            var y = new Y();
            Mapper.Map(x, y, typeof(X), typeof(Y));

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
