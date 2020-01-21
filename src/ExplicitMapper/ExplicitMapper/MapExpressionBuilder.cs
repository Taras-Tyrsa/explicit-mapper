using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExplicitMapper
{
    internal static class MapExpressionBuilder
    {
        internal static Expression BuildMapExpression(
            ParameterExpression sourceParam,
            ParameterExpression destParam,
            IReadOnlyCollection<(Expression source, Expression dest)> expressions)
        {
            var assignmentExpressions = new List<Expression>(expressions.Count);

            foreach (var (source, dest) in expressions)
            {
                var lambdaSource = (LambdaExpression)source;
                var lambdaDest = (LambdaExpression)dest;
                var destMember = ((MemberExpression)lambdaDest.Body).Member;
                var leftExpression = Expression.MakeMemberAccess(destParam, destMember);
                var rightExpression = ExpressionUtils.ReplaceParameter(lambdaSource.Body, lambdaSource.Parameters[0], sourceParam);
                var assignment = Expression.Assign(leftExpression, rightExpression);
                assignmentExpressions.Add(assignment);
            }

            assignmentExpressions.Add(Expression.Empty());

            return Expression.Block(assignmentExpressions);
        }
    }
}
