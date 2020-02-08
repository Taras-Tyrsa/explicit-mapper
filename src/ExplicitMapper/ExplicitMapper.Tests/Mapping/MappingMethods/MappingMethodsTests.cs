using FluentAssertions;
using System;
using Xunit;

namespace ExplicitMapper.Tests.Mapping.MappingMethods
{
    [Collection("Integration tests")]
    [Trait("Mapping", "Mapping methods")]
    public class MappingToCollectionsTests : IDisposable
    {
        [Fact(DisplayName = "static TDest Map<TDest>(object source)")]
        public void MapStaticMethod1()
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

            IMapper mapper = new MapperInstance();
            var y = mapper.Map<Y>(x);

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
        }

        [Fact(DisplayName = "static TDest Map<TSource, TDest>(TSource source)")]
        public void MapStaticMethod2()
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

            IMapper mapper = new MapperInstance();
            var y = mapper.Map<X, Y>(x);

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
        }

        [Fact(DisplayName = "static object Map(object source, Type sourceType, Type destType)")]
        public void MapStaticMethod3()
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

            IMapper mapper = new MapperInstance();
            var y = (Y)mapper.Map(x, typeof(X), typeof(Y));

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
        }

        [Fact(DisplayName = "static void Map<TSource, TDest>(TSource source, TDest dest)")]
        public void MapStaticMethod4()
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

            IMapper mapper = new MapperInstance();
            mapper.Map(x, y);

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
        }

        [Fact(DisplayName = "static void Map(object source, object dest, Type sourceType, Type destType)")]
        public void MapStaticMethod5()
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

            IMapper mapper = new MapperInstance();
            mapper.Map(x, y, typeof(X), typeof(Y));

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2);
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
