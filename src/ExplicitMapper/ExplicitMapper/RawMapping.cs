using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ExplicitMapper
{
    public abstract class RawMapping
    {
        private readonly List<(Expression source, Expression dest)> _expressions =
            new List<(Expression source, Expression dest)>();

        internal abstract Type SourceType { get; }
        internal abstract Type DestType { get; }
        internal (Type baseSourceType, Type baseDestType)? BaseMapping { get; private set; }
        internal IReadOnlyList<(Expression source, Expression dest)> Expressions => _expressions;

        internal void AddExpressionPair(Expression source, Expression dest)
        {
            _expressions.Add((source, dest));
        }

        protected void SetBaseMapping((Type baseSourceType, Type baseDestType) mapping)
        {
            BaseMapping = mapping;
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

        public RawMapping<TSource, TDest> Inherits<TBaseSource, TBaseDest>()
        {
            if (BaseMapping != null)
            {
                throw new ExplicitMapperException($"Mapping inheritance already configured for source type {typeof(TSource)} and destination type {typeof(TDest)}");
            }

            SetBaseMapping((typeof(TBaseSource), typeof(TBaseDest)));
            return this;
        }
    }
}
