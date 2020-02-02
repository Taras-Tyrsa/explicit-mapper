using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExplicitMapper
{
    public static class Mapper
    {
        public static TDest Map<TDest>(object source)
        {
            return (TDest) MapToNewInstance(source, source.GetType(), typeof(TDest));
        }

        public static TDest Map<TSource, TDest>(TSource source)
        {
            return (TDest) MapToNewInstance(source, typeof(TSource), typeof(TDest));
        }

        public static object Map(object source, Type sourceType, Type destType)
        {
            return MapToNewInstance(source, sourceType, destType);
        }

        public static void Map<TSource, TDest>(TSource source, TDest dest)
        {
            MapSingleInstance(source, dest, typeof(TSource), typeof(TDest));
        }

        public static void Map(object source, object dest, Type sourceType, Type destType)
        {
            MapSingleInstance(source, dest, sourceType, destType);
        }

        private static object MapToNewInstance(object source, Type sourceType, Type destType)
        {
            if (IsAssignableFromList(destType))
            {
                var count = (source as ICollection)?.Count;
                var sourceElementType = IsAssignableFromList(sourceType) ?
                    sourceType.GetGenericArguments()[0] : null;
                var destElementType = destType.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(destElementType);
                var dest = (IList)(count.HasValue ? CreateInstance(listType, count) : CreateInstance(listType));

                foreach (var sourceElement in ((IEnumerable)source))
                {
                    var destElement = CreateInstance(destElementType);
                    MapSingleInstance(sourceElement, destElement, sourceElementType ?? sourceElement.GetType(), destElementType);
                    dest.Add(destElement);
                }

                return dest;
            }
            else if (IsAssignableFromArray(destType))
            {
                throw new NotImplementedException("TO DO");
            }
            else
            {
                var dest = CreateInstance(destType);
                MapSingleInstance(source, dest, sourceType, destType);
                return dest;
            }
        }

        private static bool IsAssignableFromArray(Type type)
        {
            return type.IsConstructedGenericType && type.IsArray;
        }

        private static bool IsAssignableFromList(Type type)
        {
            if (!type.IsConstructedGenericType)
            {
                return false;
            }

            var genericTypeDefinition = type.GetGenericTypeDefinition();

            return 
                genericTypeDefinition == typeof(ICollection<>) ||
                genericTypeDefinition == typeof(IList<>) ||
                genericTypeDefinition == typeof(IEnumerable<>) ||
                genericTypeDefinition == typeof(List<>);
        }

        private static object CreateInstance(Type destType, params object[] args)
        {
            return Activator.CreateInstance(destType, args);
        }

        private static void MapSingleInstance(object source, object dest, Type sourceType, Type destType)
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
