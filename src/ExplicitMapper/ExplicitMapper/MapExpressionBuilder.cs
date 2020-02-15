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
            Type sourceType,
            Type destType,
            IReadOnlyCollection<(Expression source, Expression dest)> expressions)
        {
            var assignmentExpressions = new List<Expression>(expressions.Count);

            var sourceParamConverted = Expression.Variable(sourceType, "sourceConverted");
            var destParamConverted = Expression.Variable(destType, "destConverted");

            var sourceParamConsersion = Expression.Assign(sourceParamConverted, Expression.Convert(sourceParam, sourceType));
            var destParamConversion = Expression.Assign(destParamConverted, Expression.Convert(destParam, destType));

            assignmentExpressions.Add(sourceParamConsersion);
            assignmentExpressions.Add(destParamConversion);

            foreach (var (source, dest) in expressions)
            {
                var lambdaSource = (LambdaExpression)source;
                var lambdaDest = (LambdaExpression)dest;
                var destMember = ((MemberExpression)lambdaDest.Body).Member;
                var leftExpression = Expression.MakeMemberAccess(destParamConverted, destMember);
                var rightExpression = ExpressionUtils.ReplaceParameter(lambdaSource.Body, lambdaSource.Parameters[0], sourceParamConverted);
                var assignment = Expression.Assign(leftExpression, rightExpression);
                assignmentExpressions.Add(assignment);
            }

            return Expression.Block(new[] { sourceParamConverted, destParamConverted }, assignmentExpressions);
        }
    }
}
