using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExplicitMapper.Tests.Integration.MappingToCollections
{
    [Collection("Integration tests")]
    [Trait("Integration", "Mapping to collections")]
    public class MappingFromIEnumerableToCollectionsTests : IDisposable
    {
        private IEnumerable<X> _xcollection = new X[]
        {
            new X() { X1 = 11, X2 = 12 },
            new X() { X1 = 21, X2 = 22 },
            new X() { X1 = 31, X2 = 32 }
        }.Select(x => x);

        [Fact(DisplayName = "Map from IEnumerable<> to List<>")]
        public void IEnumerableToList()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var ycollection = Mapper.Map<List<Y>>(_xcollection);

            ycollection.Should().NotBeNull();
            ycollection.Should().Equal(_xcollection, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to IList<>")]
        public void IEnumerableToIList()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var ycollection = Mapper.Map<IList<Y>>(_xcollection);

            ycollection.Should().NotBeNull();
            ycollection.Should().Equal(_xcollection, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to ICollection<>")]
        public void IEnumerableToICollection()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var ycollection = Mapper.Map<ICollection<Y>>(_xcollection);

            ycollection.Should().NotBeNull();
            ycollection.Should().Equal(_xcollection, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to IEnumerable<>")]
        public void IEnumerableToIEnumerable()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var ycollection = Mapper.Map<IEnumerable<Y>>(_xcollection);

            ycollection.Should().NotBeNull();
            ycollection.Should().Equal(_xcollection, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to typed array")]
        public void IEnumerableToTypedArray()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var ycollection = Mapper.Map<Y[]>(_xcollection);

            ycollection.Should().NotBeNull();
            ycollection.Should().Equal(_xcollection, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
