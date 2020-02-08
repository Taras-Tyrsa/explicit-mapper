using System.Collections.Generic;

namespace ExplicitMapper.Tests.Mapping.NestedCollectionsMapping
{
    class XtoYConfiguration : MappingConfiguration
    {
        public XtoYConfiguration()
        {
            CreateMap<NestedX, NestedY>()
                .For(y => y.Y1, x => x.X1)
                .For(y => y.Y2, x => x.X2);


            CreateMap<X_IEnumerable, Y_IEnumerable>()
                .For(y => y.Y, x => Map<IEnumerable<NestedY>>(x.X));

            CreateMap<X_IEnumerable, Y_ICollection>(
                x => new Y_ICollection()
                {
                    Y = Map<ICollection<NestedY>>(x.X)
                });

            CreateMap<X_IEnumerable, Y_IList>()
                .For(y => y.Y, x => Map<IList<NestedY>>(x.X));

            CreateMap<X_IEnumerable, Y_List>(
                x => new Y_List()
                {
                    Y = Map<List<NestedY>>(x.X)
                });

            CreateMap<X_IEnumerable, Y_TypedArray>()
                .For(y => y.Y, x => Map<NestedY[]>(x.X));


            CreateMap<X_TypedArray, Y_IEnumerable>(
                x => new Y_IEnumerable()
                {
                    Y = Map<IEnumerable<NestedY>>(x.X)
                });

            CreateMap<X_TypedArray, Y_ICollection>()
                .For(y => y.Y, x => Map<ICollection<NestedY>>(x.X));

            CreateMap<X_TypedArray, Y_IList>(
                x => new Y_IList()
                {
                    Y = Map<IList<NestedY>>(x.X)
                });

            CreateMap<X_TypedArray, Y_List>()
                .For(y => y.Y, x => Map<List<NestedY>>(x.X));

            CreateMap<X_TypedArray, Y_TypedArray>(
                x => new Y_TypedArray()
                {
                    Y = Map<NestedY[]>(x.X)
                });
        }
    }
}
