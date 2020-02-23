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
                var dest = InstanceFactory.CreateInstance(destType);
                MapSingleInstance(source, dest, sourceType, destType);
                return dest;
            }
        }

        private static object MapToList(object source, Type sourceType, Type destType)
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

            if (!(source is ICollection))
            {
                var sourceList = (IList)InstanceFactory.CreateList(sourceElementType, 0);

                foreach (var sourceItem in ((IEnumerable) source))
                {
                    sourceList.Add(sourceItem);
                }

                source = sourceList;
            }

            var count = (source as ICollection).Count;

            var destElementType = destType.GetGenericArguments()[0];
            var dest = (IList)InstanceFactory.CreateList(destElementType, count);

            var sourceCollectionType = typeof(ICollection<>).MakeGenericType(sourceElementType);
            var destListType = typeof(List<>).MakeGenericType(destElementType);

            MapSingleInstance(source, dest, sourceCollectionType, destListType);

            return dest;
        }

        private static object MapToArray(object source, Type sourceType, Type destType)
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

            if (!(source is ICollection))
            {
                var sourceList = (IList)InstanceFactory.CreateList(sourceElementType, 0);

                foreach (var sourceItem in ((IEnumerable)source))
                {
                    sourceList.Add(sourceItem);
                }

                source = sourceList;
            }

            var count = (source as ICollection).Count;

            var destElementType = destType.GetElementType();
            var dest = (Array)InstanceFactory.CreateArray(destElementType, count);

            var sourceCollectionType = typeof(ICollection<>).MakeGenericType(sourceElementType);
            var destArrayType = destElementType.MakeArrayType();

            MapSingleInstance(source, dest, sourceCollectionType, destArrayType);

            return dest;
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

        private static void MapSingleInstance(object source, object dest, Type sourceType, Type destType)
        {
            if (MappingConfiguration.MapExpressions == null)
            {
                throw new ExplicitMapperException("Mapper not initialized");
            }

            if (!MappingConfiguration.MapExpressions.TryGetValue((sourceType, destType), out var func))
            {
                throw new ExplicitMapperException($"Missing mapping configuration for source type {sourceType.FullName} and destination type {destType.FullName}");
            }

            func(source, dest);
        }
    }
}
