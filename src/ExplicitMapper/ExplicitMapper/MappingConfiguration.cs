using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExplicitMapper
{
    public class MappingConfiguration
    {
        private static List<RawMapping> _rawMappings = new List<RawMapping>();
        private static Dictionary<(Type source, Type dest), Delegate> _projectionExpressions;
        private static Dictionary<(Type source, Type dest), Delegate> _mapExpressions;

        internal static IReadOnlyDictionary<(Type source, Type dest), Delegate> ProjectionExpressions => _projectionExpressions;
        internal static IReadOnlyDictionary<(Type source, Type dest), Delegate> MapExpressions => _mapExpressions;

        protected TDest Map<TDest>(object source)
        {
            return default(TDest);
        }

        protected RawMapping<TSource, TDest> CreateMap<TSource, TDest>()
        {
            var mapping = new RawMapping<TSource, TDest>();
            _rawMappings.Add(mapping);
            return mapping;
        }

        public static void Add<T>()
            where T: MappingConfiguration, new()
        {
            new T();
        }

        public static void Build()
        {
            _projectionExpressions = new Dictionary<(Type source, Type dest), Delegate>(_rawMappings.Count);

            foreach (var mapping in _rawMappings)
            {
                var sourceParam = Expression.Parameter(mapping.SourceType, "x");

                var mappingExpression = ProjectionExpressionBuilder.BuildProjectionExpression(mapping.DestinationType, sourceParam, mapping.Expressions);

                var lambda = Expression.Lambda(mappingExpression, sourceParam);

                _projectionExpressions.Add((mapping.SourceType, mapping.DestinationType), lambda.Compile());
            }

            _rawMappings = null;
        }
    }
}
