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
            if (IsList(destType))
            {
                return MapToList(source, sourceType, destType);
            }
            else if (IsArray(destType))
            {
                return MapToArray(source, sourceType, destType);
            }
            else
            {
                return MapSingleInstance(source, null, sourceType, destType);
            }
        }

        private static object MapToList(object source, Type sourceType, Type destType)
        {
            Type sourceElementType = GetElementType(sourceType);

            if (!(source is ICollection))
            {
                source = CopyToICollection(source, sourceElementType);
            }

            var destElementType = destType.GetGenericArguments()[0];
            var sourceCollectionType = typeof(ICollection<>).MakeGenericType(sourceElementType);
            var destListType = typeof(List<>).MakeGenericType(destElementType);

            return MapSingleInstance(source, null, sourceCollectionType, destListType);
        }

        private static object MapToArray(object source, Type sourceType, Type destType)
        {
            Type sourceElementType = GetElementType(sourceType);

            if (!(source is ICollection))
            {
                source = CopyToICollection(source, sourceElementType);
            }

            var destElementType = destType.GetElementType();
            var sourceCollectionType = typeof(ICollection<>).MakeGenericType(sourceElementType);
            var destArrayType = destElementType.MakeArrayType();

            return MapSingleInstance(source, null, sourceCollectionType, destArrayType);
        }

        private static Type GetElementType(Type sourceType)
        {
            Type sourceElementType;

            if (sourceType.IsConstructedGenericType)
            {
                sourceElementType = sourceType.GetGenericArguments()[0];
            }
            else if (sourceType.IsArray)
            {
                sourceElementType = sourceType.GetElementType();
            }
            else
            {
                throw new ExplicitMapperException($"Source type {sourceType.FullName} is not supported");
            }

            return sourceElementType;
        }

        private static object CopyToICollection(object source, Type sourceElementType)
        {
            IList sourceList = (IList)InstanceFactory.CreateList(sourceElementType, 0);

            foreach (var sourceItem in ((IEnumerable)source))
            {
                sourceList.Add(sourceItem);
            }

            source = sourceList;
            return source;
        }

        private static bool IsArray(Type type)
        {
            return type.IsArray;
        }

        private static bool IsList(Type type)
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

        private static object MapSingleInstance(object source, object dest, Type sourceType, Type destType)
        {
            if (MappingConfiguration.MapExpressions == null)
            {
                throw new ExplicitMapperException("Mapper not initialized");
            }

            if (!MappingConfiguration.MapExpressions.TryGetValue((sourceType, destType), out var func))
            {
                throw new ExplicitMapperException($"Missing mapping configuration for source type {sourceType.FullName} and destination type {destType.FullName}");
            }

            return func(source, dest);
        }
    }
}
