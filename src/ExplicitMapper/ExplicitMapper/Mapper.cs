using System;

namespace ExplicitMapper
{
    public static class Mapper
    {
        public static TDest Map<TDest>(object source)
            where TDest : new()
        {
            var dest = new TDest();
            Map(source, dest, source.GetType(), typeof(TDest));
            return dest;
        }

        public static TDest Map<TSource, TDest>(TSource source)
            where TDest : new()
        {
            var dest = new TDest();
            Map(source, dest, typeof(TSource), typeof(TDest));
            return dest;
        }

        public static object Map(object source, Type sourceType, Type destType)
        {
            var dest = Activator.CreateInstance(destType);
            Map(source, dest, sourceType, destType);
            return dest;
        }

        public static void Map<TSource, TDest>(TSource source, TDest dest)
            where TDest: new()
        {
            Map(source, dest, typeof(TSource), typeof(TDest));
        }

        public static void Map(object source, object dest, Type sourceType, Type destType)
        {
            if (MappingConfiguration.MapExpressions == null)
            {
                throw new ExplicitMapperException("Mapper not initialized");
            }

            if (!MappingConfiguration.MapExpressions.TryGetValue((sourceType, destType), out var func))
            {
                throw new ExplicitMapperException($"Missing mapping configuration for source type {sourceType} and destination type {destType}");
            }

            func.DynamicInvoke(source, dest);
        }
    }
}
