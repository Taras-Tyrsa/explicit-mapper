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
            where TDest: new()
        {
            return Mapper.Map<TDest>(source);
        }

        protected RawMapping<TSource, TDest> CreateMap<TSource, TDest>()
        {
            if (_rawMappings == null)
            {
                _rawMappings = new List<RawMapping>();
            }

            var mapping = new RawMapping<TSource, TDest>();
            _rawMappings.Add(mapping);
            return mapping;
        }

        protected void CreateMap<TSource, TDest>(Expression<Func<TSource, TDest>> objectInitializer)
            where TSource: new()
        {
            if (objectInitializer == null)
            {
                throw new ArgumentNullException(nameof(objectInitializer));
            }

            if (_rawMappings == null)
            {
                _rawMappings = new List<RawMapping>();
            }

            var mapping = ObjectInitializerParser.Parse(objectInitializer);

            _rawMappings.Add(mapping);
        }

        public static void Add<T>()
            where T: MappingConfiguration, new()
        {
            new T();
        }

        public static void Build()
        {
            _projectionExpressions = new Dictionary<(Type source, Type dest), Delegate>(_rawMappings.Count);
            _mapExpressions = new Dictionary<(Type source, Type dest), Delegate>(_rawMappings.Count);

            foreach (var mapping in _rawMappings)
            {
                if (_mapExpressions.ContainsKey((mapping.SourceType, mapping.DestType)))
                {
                    throw new ExplicitMapperException(
                        $"Duplicate mapping configuration for source type '{mapping.SourceType.FullName}' and destination type '{mapping.DestType.FullName}'");
                }

                var sourceParam = Expression.Parameter(mapping.SourceType, "source");
                var destParam = Expression.Parameter(mapping.DestType, "dest");

                var projectionExpression = ProjectionExpressionBuilder.BuildProjectionExpression(mapping.DestType, sourceParam, mapping.Expressions);
                var projectionLambda = Expression.Lambda(projectionExpression, sourceParam);
                _projectionExpressions.Add((mapping.SourceType, mapping.DestType), projectionLambda.Compile());

                var mapExpression = MapExpressionBuilder.BuildMapExpression(sourceParam, destParam, mapping.Expressions);
                var mapLambda = Expression.Lambda(mapExpression, sourceParam, destParam);
                _mapExpressions.Add((mapping.SourceType, mapping.DestType), mapLambda.Compile());
            }

            _rawMappings = null;
        }

        public static void Clear()
        {
            _projectionExpressions = null;
            _mapExpressions = null;
            _rawMappings = null;
        }
    }
}
