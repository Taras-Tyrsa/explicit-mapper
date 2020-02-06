using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper
{
    public interface IMapper
    {
        TDest Map<TDest>(object source);

        TDest Map<TSource, TDest>(TSource source);

        object Map(object source, Type sourceType, Type destType);

        void Map<TSource, TDest>(TSource source, TDest dest);

        void Map(object source, object dest, Type sourceType, Type destType);
    }
}
