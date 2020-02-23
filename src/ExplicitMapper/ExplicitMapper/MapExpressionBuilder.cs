using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        internal static Expression BuildMapToListExpression(
            ParameterExpression sourceParam,
            ParameterExpression destParam,
            Type sourceType,
            Type destType,
            IReadOnlyCollection<(Expression source, Expression dest)> expressions)
        {
            var assignmentExpressions = new List<Expression>(expressions.Count);

            var sourceCollectionType = typeof(ICollection<>).MakeGenericType(sourceType);
            var destListType = typeof(List<>).MakeGenericType(destType);
            var sourceCollection = Expression.Variable(sourceCollectionType, "sourceCollection");
            var destList = Expression.Variable(destListType, "destList");
            var sourceParamConsersion = Expression.Assign(sourceCollection, Expression.Convert(sourceParam, sourceCollectionType));
            var destParamConversion = Expression.Assign(destList, Expression.Convert(destParam, destListType));

            assignmentExpressions.Add(sourceParamConsersion);
            assignmentExpressions.Add(destParamConversion);

            if (!destType.GetConstructors().Any(c => c.GetParameters().Length == 0))
            {
                throw new ExplicitMapperException($"No default constructor for type {destType} exists");
            }

            var sourceItemVariable = Expression.Variable(sourceType, "sourceItem");
            var destItemVariable = Expression.Variable(destType, "destItem");

            var newDestItemExpression = Expression.New(destType);
            var destItemNewAssignmentExpression = Expression.Assign(destItemVariable, newDestItemExpression);

            var mapExpression = BuildMapExpression(sourceItemVariable, destItemVariable, sourceType, destType, expressions);
            var addMethod = typeof(IList).GetMethod(nameof(IList.Add));
            var itemAssignmentExpression = Expression.Call(destList, addMethod, destItemVariable);
            var loopBlock = Expression.Block(new[] { destItemVariable },
                destItemNewAssignmentExpression, mapExpression, itemAssignmentExpression);

            var forEachLoopExpression = ForEach(sourceCollection, sourceItemVariable, loopBlock);

            assignmentExpressions.Add(forEachLoopExpression);
            assignmentExpressions.Add(destList);

            return Expression.Block(new[] { sourceCollection, destList }, assignmentExpressions);
        }

        private static Expression ForEach(Expression collection, ParameterExpression loopVar, Expression loopContent)
        {
            var elementType = loopVar.Type;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var getEnumeratorCall = Expression.Call(collection, enumerableType.GetMethod("GetEnumerator"));
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);

            // The MoveNext method's actually on IEnumerator, not IEnumerator<T>
            var moveNextCall = Expression.Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext"));

            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(new[] { enumeratorVar },
                enumeratorAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(moveNextCall, Expression.Constant(true)),
                        Expression.Block(new[] { loopVar },
                            Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current")),
                            loopContent
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }
    }
}
