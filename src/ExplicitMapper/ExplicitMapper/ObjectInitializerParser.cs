using System;
using System.Linq.Expressions;

namespace ExplicitMapper
{
    internal static class ObjectInitializerParser
    {
        public static RawMapping<TSource, TDest> Parse<TSource, TDest>(Expression<Func<TSource, TDest>> expression)
        {
            var rawMapping = new RawMapping<TSource, TDest>();

            foreach (MemberAssignment binding in ((MemberInitExpression)expression.Body).Bindings)
            {
                var destParam = Expression.Parameter(typeof(TDest), "dest");
                var sourceParam = Expression.Parameter(typeof(TSource), "source");
                var dest = Expression.Lambda(Expression.MakeMemberAccess(destParam, binding.Member), destParam);
                var sourceExpression = ExpressionUtils.ReplaceParameter(binding.Expression, expression.Parameters[0], sourceParam);
                var source = Expression.Lambda(sourceExpression, sourceParam);
                rawMapping.AddExpressionPair(source, dest);
            }

            return rawMapping;
        }
    }
}
