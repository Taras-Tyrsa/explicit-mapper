using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExplicitMapper.Tests.Mapping.NestedCollectionsMapping
{
    [Collection("Integration tests")]
    [Trait("Mapping", "Mapping nested collections")]
    public class MappingFromNestedIEnumerableToCollectionsTests : IDisposable
    {
        private X_IEnumerable _xObj = new X_IEnumerable()
        {
            X = new NestedX[]
            {
                new NestedX() { X1 = 11, X2 = 12 },
                new NestedX() { X1 = 21, X2 = 22 },
                new NestedX() { X1 = 31, X2 = 32 }
            }.Select(x => x)
        };

        [Fact(DisplayName = "Map from IEnumerable<> to List<>")]
        public void IEnumerableToList()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var yObj = Mapper.Map<Y_List>(_xObj);

            yObj.Should().NotBeNull();
            yObj.Y.Should().NotBeNull();
            yObj.Y.Should().Equal(_xObj.X, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to IList<>")]
        public void IEnumerableToIList()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var yObj = Mapper.Map<Y_IList>(_xObj);

            yObj.Should().NotBeNull();
            yObj.Y.Should().NotBeNull();
            yObj.Y.Should().Equal(_xObj.X, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to ICollection<>")]
        public void IEnumerableToICollection()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var yObj = Mapper.Map<Y_ICollection>(_xObj);

            yObj.Should().NotBeNull();
            yObj.Y.Should().NotBeNull();
            yObj.Y.Should().Equal(_xObj.X, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to IEnumerable<>")]
        public void IEnumerableToIEnumerable()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var yObj = Mapper.Map<Y_IEnumerable>(_xObj);

            yObj.Should().NotBeNull();
            yObj.Y.Should().NotBeNull();
            yObj.Y.Should().Equal(_xObj.X, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        [Fact(DisplayName = "Map from IEnumerable<> to typed array")]
        public void IEnumerableToTypedArray()
        {
            MappingConfiguration.Add<XtoYConfiguration>();
            MappingConfiguration.Build();

            var yObj = Mapper.Map<Y_TypedArray>(_xObj);

            yObj.Should().NotBeNull();
            yObj.Y.Should().NotBeNull();
            yObj.Y.Should().Equal(_xObj.X, (y, x) => y.Y1 == x.X1 && y.Y2 == x.X2);
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
