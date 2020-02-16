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
            var count = (source as ICollection)?.Count;
            var sourceElementType = sourceType.IsConstructedGenericType ?
                sourceType.GetGenericArguments()[0] : null;
            var destElementType = destType.GetGenericArguments()[0];
            var dest = (IList)InstanceFactory.CreateList(destElementType, count ?? 0);

            if (source is IList)
            {
                var sourceList = (IList)source;

                for (int i = 0; i < sourceList.Count; i++)
                {
                    var destElement = InstanceFactory.CreateInstance(destElementType);
                    MapSingleInstance(sourceList[i], destElement, sourceElementType ?? sourceList[i].GetType(), destElementType);
                    dest.Add(destElement);
                }
            }
            else
            {
                foreach (var sourceElement in ((IEnumerable)source))
                {
                    var destElement = InstanceFactory.CreateInstance(destElementType);
                    MapSingleInstance(sourceElement, destElement, sourceElementType ?? sourceElement.GetType(), destElementType);
                    dest.Add(destElement);
                }
            }

            return dest;
        }

        private static object MapToArray(object source, Type sourceType, Type destType)
        {
            if (!(source is ICollection))
            {
                source = ((IEnumerable)source).Cast<object>().ToArray();
            }

            var count = (source as ICollection).Count;
            var sourceElementType = sourceType.GetElementType();
            var destElementType = destType.GetElementType();
            var dest = (Array)InstanceFactory.CreateArray(destElementType, count);

            if (source is IList)
            {
                var sourceList = (IList)source;

                for (int i = 0; i < sourceList.Count; i++)
                {
                    var destElement = InstanceFactory.CreateInstance(destElementType);
                    MapSingleInstance(sourceList[i], destElement, sourceElementType ?? sourceList[i].GetType(), destElementType);
                    dest.SetValue(destElement, i);
                }
            }
            else
            {
                int i = 0;

                foreach (var sourceElement in ((IEnumerable)source))
                {
                    var destElement = InstanceFactory.CreateInstance(destElementType);
                    MapSingleInstance(sourceElement, destElement, sourceElementType ?? sourceElement.GetType(), destElementType);
                    dest.SetValue(destElement, i);
                    i++;
                }
            }

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
