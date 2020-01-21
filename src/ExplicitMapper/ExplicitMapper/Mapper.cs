using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExplicitMapper
{
    public static class Mapper
    {
        private static Dictionary<(Type source, Type dest), Delegate> _compiledExpressions;

        public static TDest Map<TDest>(object source) 
            where TDest: new()
        {            
            if (!_compiledExpressions.TryGetValue((source.GetType(), typeof(TDest)), out var func))
            {
                throw new Exception();
            }

            var dest = func.DynamicInvoke(source);

            return (TDest)dest;
        }

        public static void Build()
        {
            _compiledExpressions = new Dictionary<(Type source, Type dest), Delegate>(MappingConfiguration.RawMappings.Count);

            foreach (var mapping in MappingConfiguration.RawMappings)
            {
                var sourceParam = Expression.Parameter(mapping.SourceType, "x");

                var mappingExpression = BuildCreateAndMapExpression(mapping.DestinationType, sourceParam, mapping.Expressions);

                var lambda = Expression.Lambda(mappingExpression, sourceParam);

                _compiledExpressions.Add((mapping.SourceType, mapping.DestinationType), lambda.Compile());
            }
        }

        private static Expression BuildCreateAndMapExpression(
            Type destType, ParameterExpression sourceParam,
            IReadOnlyCollection<(Expression source, Expression dest)> expressions)
        {
            var newExpression = Expression.New(destType);
            var bindings = new List<MemberAssignment>(expressions.Count);

            foreach (var (source, dest) in expressions)
            {
                var lambdaSource = (LambdaExpression)source;
                var lambdaDest = (LambdaExpression)dest;
                var destMember = ((MemberExpression)lambdaDest.Body).Member;
                var invocationExpression = ExpressionUtils.ReplaceParameter(lambdaSource.Body, lambdaSource.Parameters[0], sourceParam);

                bindings.Add(Expression.Bind(destMember, invocationExpression));
            }

            return Expression.MemberInit(newExpression, bindings);
        }
    }
}
