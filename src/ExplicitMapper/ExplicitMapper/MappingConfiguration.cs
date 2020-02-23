using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExplicitMapper
{
    public class MappingConfiguration
    {
        private static List<RawMapping> _rawMappings = new List<RawMapping>();
        private static Dictionary<(Type source, Type dest), Action<object, object>> _projectionExpressions;
        private static Dictionary<(Type source, Type dest), Action<object, object>> _mapExpressions;

        internal static IReadOnlyDictionary<(Type source, Type dest), Action<object, object>> ProjectionExpressions => _projectionExpressions;
        internal static IReadOnlyDictionary<(Type source, Type dest), Action<object, object>> MapExpressions => _mapExpressions;

        protected TDest Map<TDest>(object source)
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

        protected RawMapping<TSource, TDest> CreateMap<TSource, TDest>(Expression<Func<TSource, TDest>> objectInitializer)
            where TDest : new()
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
            return mapping;
        }

        public static void Add<T>()
            where T: MappingConfiguration, new()
        {
            new T();
        }

        public static void Build()
        {
            if (_rawMappings == null)
            {
                _projectionExpressions = new Dictionary<(Type source, Type dest), Action<object, object>>(0);
                _mapExpressions = new Dictionary<(Type source, Type dest), Action<object, object>>(0);
                return;
            }

            _projectionExpressions = new Dictionary<(Type source, Type dest), Action<object, object>>(_rawMappings.Count);
            _mapExpressions = new Dictionary<(Type source, Type dest), Action<object, object>>(_rawMappings.Count);

            foreach (var mapping in _rawMappings)
            {
                if (_mapExpressions.ContainsKey((mapping.SourceType, mapping.DestType)))
                {
                    throw new ExplicitMapperException(
                        $"Duplicate mapping configuration for source type '{mapping.SourceType.FullName}' and destination type '{mapping.DestType.FullName}'");
                }

                var sourceParam = Expression.Parameter(typeof(object), "source");
                var destParam = Expression.Parameter(typeof(object), "dest");

                /*var projectionExpression = ProjectionExpressionBuilder.BuildProjectionExpression(mapping.DestType, sourceParam, mapping.Expressions);
                var projectionLambda = Expression.Lambda(projectionExpression, sourceParam);
                _projectionExpressions.Add((mapping.SourceType, mapping.DestType), projectionLambda.Compile());*/

                var mappingExpressions = GetMappingExpressions(mapping);
                var mapExpression = MapExpressionBuilder.BuildMapExpression(sourceParam, destParam, mapping.SourceType, mapping.DestType, mappingExpressions);
                var mapLambda = Expression.Lambda(typeof(Action<object, object>), mapExpression, sourceParam, destParam);
                _mapExpressions.Add((mapping.SourceType, mapping.DestType), (Action<object, object>)mapLambda.Compile());

                if (!mapping.DestType.IsAbstract)
                {
                    var sourceCollectionType = typeof(ICollection<>).MakeGenericType(mapping.SourceType);
                    var destListType = typeof(List<>).MakeGenericType(mapping.DestType);
                    var mapListExpression = MapExpressionBuilder.BuildMapToListExpression(sourceParam, destParam, mapping.SourceType, mapping.DestType, mappingExpressions);
                    var mapListLambda = Expression.Lambda(typeof(Action<object, object>), mapListExpression, sourceParam, destParam);
                    _mapExpressions.Add((sourceCollectionType, destListType), (Action<object, object>)mapListLambda.Compile());
                }
            }

            _rawMappings = null;


            IReadOnlyList<(Expression source, Expression dest)> GetMappingExpressions(RawMapping mapping)
            {
                var expressions = new List<(Expression source, Expression dest)>();

                if (mapping.BaseMapping != null)
                {
                    var baseSourceType = mapping.BaseMapping.Value.baseSourceType;
                    var baseDestType = mapping.BaseMapping.Value.baseDestType;

                    var baseMapping = _rawMappings.FirstOrDefault(m => m.SourceType == baseSourceType && m.DestType == baseDestType);
                    if (baseMapping == null)
                    {
                        throw new ExplicitMapperException($"Missing mapping configuration for source type {mapping.BaseMapping.Value.baseSourceType.FullName} and destination type {mapping.BaseMapping.Value.baseDestType.FullName}");
                    }

                    expressions.AddRange(GetMappingExpressions(baseMapping));
                }

                expressions.AddRange(mapping.Expressions);
                return expressions;
            }
        }

        public static void Clear()
        {
            _projectionExpressions = null;
            _mapExpressions = null;
            _rawMappings = null;
        }
    }
}
