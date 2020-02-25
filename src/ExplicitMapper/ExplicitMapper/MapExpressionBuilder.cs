using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

            var sourceParamCasted = sourceParam;
            var destParamCasted = destParam;

            var blockParameters = new List<ParameterExpression>(2);

            if (sourceParam.Type != sourceType)
            {
                sourceParamCasted = Expression.Variable(sourceType, "sourceConverted");
                var sourceParamConversionExpression = Expression.Convert(sourceParam, sourceType);
                var sourceParamAssignment = Expression.Assign(sourceParamCasted, sourceParamConversionExpression);
                assignmentExpressions.Add(sourceParamAssignment);
                blockParameters.Add(sourceParamCasted);
            }

            if (destParam.Type != destType)
            {
                destParamCasted = Expression.Variable(destType, "destConverted");
                var destParamConversionExpression = Expression.Convert(destParam, destType);
                var destParamAssignment = Expression.Assign(destParamCasted, destParamConversionExpression);
                assignmentExpressions.Add(destParamAssignment);
                blockParameters.Add(destParamCasted);
            }

            foreach (var (source, dest) in expressions)
            {
                var lambdaSource = (LambdaExpression)source;
                var lambdaDest = (LambdaExpression)dest;
                var destMember = ((MemberExpression)lambdaDest.Body).Member;
                var leftExpression = Expression.MakeMemberAccess(destParamCasted, destMember);
                var rightExpression = ExpressionUtils.ReplaceParameter(lambdaSource.Body, lambdaSource.Parameters[0], sourceParamCasted);
                var assignment = Expression.Assign(leftExpression, rightExpression);
                assignmentExpressions.Add(assignment);
            }

            return Expression.Block(blockParameters, assignmentExpressions);
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
            var indexVariable = Expression.Variable(typeof(int), "i");
            var addMethod = destListType.GetMethod("Add");
            var itemAssignmentExpression = Expression.Call(destList, addMethod, destItemVariable);
            var indexIncrementExpression = Expression.PostIncrementAssign(indexVariable);
            var loopBlock = Expression.Block(new[] { destItemVariable },
                destItemNewAssignmentExpression, mapExpression, itemAssignmentExpression, indexIncrementExpression);

            var mapFromArrayExpression = MapFromArray(sourceType, sourceCollection, sourceItemVariable, indexVariable, loopBlock);
            var mapFromListExpression = MapFromList(sourceType, sourceCollection, sourceItemVariable, indexVariable, loopBlock);
            var mapFromEnumerableExpression = MapFromEnumerable(sourceType, sourceCollection, sourceItemVariable, loopBlock);

            var mapMethodSelectionExpression = Expression.IfThenElse(
                Expression.TypeIs(sourceCollection, sourceType.MakeArrayType()),
                mapFromArrayExpression,
                Expression.IfThenElse(
                    Expression.TypeIs(sourceCollection, typeof(List<>).MakeGenericType(sourceType)),
                    mapFromListExpression,
                    mapFromEnumerableExpression
                )
            );

            assignmentExpressions.Add(mapMethodSelectionExpression);
            assignmentExpressions.Add(destList);

            return Expression.Block(new[] { indexVariable, sourceCollection, destList }, assignmentExpressions);
        }

        internal static Expression BuildMapToArrayExpression(
            ParameterExpression sourceParam,
            ParameterExpression destParam,
            Type sourceType,
            Type destType,
            IReadOnlyCollection<(Expression source, Expression dest)> expressions)
        {
            var assignmentExpressions = new List<Expression>(expressions.Count);

            var sourceCollectionType = typeof(ICollection<>).MakeGenericType(sourceType);
            var destArrayType = destType.MakeArrayType();
            var sourceCollection = Expression.Variable(sourceCollectionType, "sourceCollection");
            var destArray = Expression.Variable(destArrayType, "destArray");
            var sourceParamConsersion = Expression.Assign(sourceCollection, Expression.Convert(sourceParam, sourceCollectionType));
            var destParamConversion = Expression.Assign(destArray, Expression.Convert(destParam, destArrayType));

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
            var indexVariable = Expression.Variable(typeof(int), "i");
            var destArrayAccessExpression = Expression.ArrayAccess(destArray, indexVariable);
            var itemAssignmentExpression = Expression.Assign(destArrayAccessExpression, destItemVariable);
            var indexIncrementExpression = Expression.PostIncrementAssign(indexVariable);
            var loopBlock = Expression.Block(
                new[] { destItemVariable },
                destItemNewAssignmentExpression, mapExpression, itemAssignmentExpression, indexIncrementExpression);

            var mapFromArrayExpression = MapFromArray(sourceType, sourceCollection, sourceItemVariable, indexVariable, loopBlock);
            var mapFromListExpression = MapFromList(sourceType, sourceCollection, sourceItemVariable, indexVariable, loopBlock);
            var mapFromEnumerableExpression = MapFromEnumerable(sourceType, sourceCollection, sourceItemVariable, loopBlock);

            var mapMethodSelectionExpression = Expression.IfThenElse(
                Expression.TypeIs(sourceCollection, sourceType.MakeArrayType()),
                mapFromArrayExpression,
                Expression.IfThenElse(
                    Expression.TypeIs(sourceCollection, typeof(List<>).MakeGenericType(sourceType)),
                    mapFromListExpression,
                    mapFromEnumerableExpression
                )
            );

            assignmentExpressions.Add(mapMethodSelectionExpression);
            assignmentExpressions.Add(destArray);

            return Expression.Block(new[] { indexVariable, sourceCollection, destArray }, assignmentExpressions);
        }

        private static Expression MapFromArray(Type sourceType, Expression collection, ParameterExpression loopVariable, ParameterExpression indexVariable, Expression loopContent)
        {
            var breakLabel = Expression.Label("LoopBreak");
            var arrayType = sourceType.MakeArrayType();
            var sourceArrayVariable = Expression.Variable(arrayType, "sourceArray");
            var arraySizeVariable = Expression.Variable(typeof(int), "arraySize");
            var sourceArrayAccessExpression = Expression.ArrayAccess(sourceArrayVariable, indexVariable);
            var getLengthExpression = Expression.ArrayLength(sourceArrayVariable);

            var loop = Expression.Block(new[] { sourceArrayVariable, arraySizeVariable },
                Expression.Assign(sourceArrayVariable, Expression.Convert(collection, arrayType)),
                Expression.Assign(arraySizeVariable, getLengthExpression),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(indexVariable, arraySizeVariable),
                        Expression.Block(new[] { loopVariable },
                            Expression.Assign(loopVariable, sourceArrayAccessExpression),
                            loopContent
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }

        private static Expression MapFromList(Type sourceType, Expression collection, ParameterExpression loopVariable, ParameterExpression indexVariable, Expression loopContent)
        {
            var breakLabel = Expression.Label("LoopBreak");
            var listType = typeof(List<>).MakeGenericType(sourceType);
            var sourceListVariable = Expression.Variable(listType, "sourceList");
            var listSizeVariable = Expression.Variable(typeof(int), "listSize");
            var getLengthMethod = Expression.Call(sourceListVariable, listType.GetProperty(nameof(IList.Count)).GetGetMethod());
            var indexExpression = Expression.MakeIndex(sourceListVariable, listType.GetProperty("Item"), new[] { indexVariable });

            var loop = Expression.Block(new[] { sourceListVariable, listSizeVariable },
                Expression.Assign(sourceListVariable, Expression.Convert(collection, listType)),
                Expression.Assign(listSizeVariable, getLengthMethod),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(indexVariable, listSizeVariable),
                        Expression.Block(new[] { loopVariable },
                            Expression.Assign(loopVariable, indexExpression),
                            loopContent
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }

        private static Expression MapFromEnumerable(Type sourceType, Expression collection, ParameterExpression loopVariable, Expression loopContent)
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(sourceType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(sourceType);

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
                        Expression.Block(new[] { loopVariable },
                            Expression.Assign(loopVariable, Expression.Property(enumeratorVar, "Current")),
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
