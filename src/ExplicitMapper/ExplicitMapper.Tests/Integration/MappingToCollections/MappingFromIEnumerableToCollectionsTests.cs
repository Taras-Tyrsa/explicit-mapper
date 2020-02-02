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
        private IEnumerable<X> xcollection = new X[]
        {
            new X() { X1 = 11, X2 = 12 },
            new X() { X1 = 21, X2 = 22 },
            new X() { X1 = 31, X2 = 32 }
        }.Select(x => x);

        [Fact(DisplayName = "Map from IEnumerable<> to List<>")]
        public void TypedArrayToList()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var ycollection = Mapper.Map<List<Y>>(xcollection);

            ycollection.Should().NotBeNull();
            ycollection.Should().Equal(xcollection, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to IList<>")]
        public void TypedArrayToIList()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var ycollection = Mapper.Map<IList<Y>>(xcollection);

            ycollection.Should().NotBeNull();
            ycollection.Should().Equal(xcollection, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to ICollection<>")]
        public void TypedArrayToICollection()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var ycollection = Mapper.Map<ICollection<Y>>(xcollection);

            ycollection.Should().NotBeNull();
            ycollection.Should().Equal(xcollection, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to IEnumerable<>")]
        public void TypedArrayToIEnumerable()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var ycollection = Mapper.Map<IEnumerable<Y>>(xcollection);

            ycollection.Should().NotBeNull();
            ycollection.Should().Equal(xcollection, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
