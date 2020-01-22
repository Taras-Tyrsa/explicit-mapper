using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExplicitMapper
{
    public class RawMapping
    {
        private readonly List<(Expression source, Expression dest)> _expressions =
            new List<(Expression source, Expression dest)>();

        internal virtual Type SourceType { get; }
        internal virtual Type DestType { get; }
        internal IReadOnlyList<(Expression source, Expression dest)> Expressions => _expressions;

        internal RawMapping()
        {
        }

        internal RawMapping(Type sourceType, Type destType)
        {
            SourceType = sourceType;
            DestType = destType;
        }

        internal void AddExpressionPair(Expression source, Expression dest)
        {
            _expressions.Add((source, dest));
        }
    }

    public class RawMapping<TSource, TDest> : RawMapping
    {
        internal override Type SourceType => typeof(TSource);
        internal override Type DestType => typeof(TDest);

        public RawMapping<TSource, TDest> For<TDestMember>(
            Expression<Func<TDest, TDestMember>> dest, Expression<Func<TSource, TDestMember>> source)
        {
            if (dest.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new ArgumentException(nameof(dest));
            }

            AddExpressionPair(source, dest);

            return this;
        }
    }
}
