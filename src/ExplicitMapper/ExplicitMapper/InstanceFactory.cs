using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExplicitMapper
{
    internal static class InstanceFactory
    {
        private static readonly Dictionary<Type, Func<object>> _defaultConstructors = new Dictionary<Type, Func<object>>();

        public static object CreateInstance(Type type)
        {
            Func<object> newCall = null;

            if (!_defaultConstructors.TryGetValue(type, out newCall))
            {
                newCall = AddTypeWithDefaultConstructor(type);
            }

            return newCall();
        }

        private static Func<object> AddTypeWithDefaultConstructor(Type type)
        {
            if (!type.GetConstructors().Any(c => c.GetParameters().Length == 0))
            {
                throw new ExplicitMapperException($"No default constructor for type {type} exists");
            }

            var newCall = (Func<object>)Expression.Lambda(typeof(Func<object>), Expression.New(type)).Compile();
            _defaultConstructors.Add(type, newCall);
            return newCall;
        }
    }
}
