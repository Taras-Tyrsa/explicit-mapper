using System;

namespace ExplicitMapper
{
    public class MapperInstance : IMapper
    {
        public TDest Map<TDest>(object source)
        {
            return Mapper.Map<TDest>(source);
        }

        public TDest Map<TSource, TDest>(TSource source)
        {
            return Mapper.Map<TSource, TDest>(source);
        }

        public object Map(object source, Type sourceType, Type destType)
        {
            return Mapper.Map(source, sourceType, destType);
        }

        public void Map<TSource, TDest>(TSource source, TDest dest)
        {
            Mapper.Map<TSource, TDest>(source, dest);
        }

        public void Map(object source, object dest, Type sourceType, Type destType)
        {
            Mapper.Map(source, dest, sourceType, destType);
        }
    }
}
