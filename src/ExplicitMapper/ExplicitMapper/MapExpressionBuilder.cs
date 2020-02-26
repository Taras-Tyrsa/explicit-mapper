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

            if (!destType.IsAbstract && destType.GetConstructor(new Type[0]) != null)
            {
                var createDestExpression = Expression.IfThen(
                    Expression.Equal(destParam, Expression.Constant(null, destType)),
                    Expression.Assign(destParam, Expression.New(destType))
                );

                assignmentExpressions.Add(createDestExpression);
            }

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

            assignmentExpressions.Add(destParam);

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

            var lengthVariable = Expression.Variable(typeof(int), "length");
            var getLengthExpression = BuildLengthCalculationExpression(sourceParam, sourceType, lengthVariable);

            var createDestExpression = Expression.Assign(destList, Expression.New(destListType.GetConstructor(new[] { typeof(int) }), lengthVariable));

            assignmentExpressions.Add(getLengthExpression);
            assignmentExpressions.Add(createDestExpression);

            var sourceParamConsersion = Expression.Assign(sourceCollection, Expression.Convert(sourceParam, sourceCollectionType));

            assignmentExpressions.Add(sourceParamConsersion);

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

            var mapFromArrayExpression = MapFromArray(sourceType, sourceCollection, sourceItemVariable, indexVariable, loopBlock, lengthVariable);
            var mapFromListExpression = MapFromList(sourceType, sourceCollection, sourceItemVariable, indexVariable, loopBlock, lengthVariable);
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

            return Expression.Block(new[] { lengthVariable, indexVariable, sourceCollection, destList }, assignmentExpressions);
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

            var lengthVariable = Expression.Variable(typeof(int), "length");
            var getLengthExpression = BuildLengthCalculationExpression(sourceParam, sourceType, lengthVariable);

            var createDestExpression = Expression.Assign(destArray, Expression.New(destArrayType.GetConstructor(new[] { typeof(int) }), lengthVariable));

            assignmentExpressions.Add(getLengthExpression);
            assignmentExpressions.Add(createDestExpression);

            var sourceParamConsersion = Expression.Assign(sourceCollection, Expression.Convert(sourceParam, sourceCollectionType));

            assignmentExpressions.Add(sourceParamConsersion);

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

            var mapFromArrayExpression = MapFromArray(sourceType, sourceCollection, sourceItemVariable, indexVariable, loopBlock, lengthVariable);
            var mapFromListExpression = MapFromList(sourceType, sourceCollection, sourceItemVariable, indexVariable, loopBlock, lengthVariable);
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

            return Expression.Block(new[] { lengthVariable, indexVariable, sourceCollection, destArray }, assignmentExpressions);
        }

        private static Expression BuildLengthCalculationExpression(ParameterExpression sourceParam, Type sourceType, ParameterExpression lengthVariable)
        {
            var arrayType = sourceType.MakeArrayType();
            var listType = typeof(List<>).MakeGenericType(sourceType);
            var collectionType = typeof(ICollection<>).MakeGenericType(sourceType);
            return Expression.IfThenElse(
                Expression.TypeIs(sourceParam, arrayType),
                Expression.Assign(lengthVariable, Expression.ArrayLength(Expression.Convert(sourceParam, arrayType))),
                Expression.IfThenElse(
                    Expression.TypeIs(sourceParam, listType),
                    Expression.Assign(
                        lengthVariable,
                        Expression.Call(
                            Expression.Convert(sourceParam, listType),
                            listType.GetProperty(nameof(IList.Count)).GetGetMethod())
                        ),
                    Expression.Assign(lengthVariable,
                        Expression.Call(
                            Expression.Convert(sourceParam, collectionType),
                            collectionType.GetProperty(nameof(ICollection.Count)).GetGetMethod())
                        )
                )
            );
        }

        private static Expression MapFromArray(
            Type sourceType, Expression collection, ParameterExpression loopVariable,
            ParameterExpression indexVariable, Expression loopContent, ParameterExpression arraySizeVariable)
        {
            var breakLabel = Expression.Label("LoopBreak");
            var arrayType = sourceType.MakeArrayType();
            var sourceArrayVariable = Expression.Variable(arrayType, "sourceArray");
            var sourceArrayAccessExpression = Expression.ArrayAccess(sourceArrayVariable, indexVariable);

            var loop = Expression.Block(new[] { sourceArrayVariable },
                Expression.Assign(sourceArrayVariable, Expression.Convert(collection, arrayType)),
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

        private static Expression MapFromList(
            Type sourceType, Expression collection, ParameterExpression loopVariable,
            ParameterExpression indexVariable, Expression loopContent, ParameterExpression listSizeVariable)
        {
            var breakLabel = Expression.Label("LoopBreak");
            var listType = typeof(List<>).MakeGenericType(sourceType);
            var sourceListVariable = Expression.Variable(listType, "sourceList");
            var indexExpression = Expression.MakeIndex(sourceListVariable, listType.GetProperty("Item"), new[] { indexVariable });

            var loop = Expression.Block(new[] { sourceListVariable },
                Expression.Assign(sourceListVariable, Expression.Convert(collection, listType)),
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
