using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExplicitMapper
{
    internal static class ProjectionExpressionBuilder
    {
        internal static Expression BuildProjectionExpression(
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
