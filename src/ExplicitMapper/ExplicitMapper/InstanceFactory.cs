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
        private static readonly Dictionary<Type, Func<int, object>> _arrayConstructors = new Dictionary<Type, Func<int, object>>();
        private static readonly Dictionary<Type, Func<int, object>> _listConstructors = new Dictionary<Type, Func<int, object>>();

        public static object CreateInstance(Type type)
        {
            Func<object> newCall = null;

            if (!_defaultConstructors.TryGetValue(type, out newCall))
            {
                newCall = AddTypeWithDefaultConstructor(type);
            }

            return newCall();
        }

        public static object CreateArray(Type type, int length)
        {
            Func<int, object> newCall = null;

            if (!_arrayConstructors.TryGetValue(type, out newCall))
            {
                newCall = AddArrayForType(type);
            }

            return newCall(length);
        }

        public static object CreateList(Type type, int length)
        {
            Func<int, object> newCall = null;

            if (!_listConstructors.TryGetValue(type, out newCall))
            {
                newCall = AddListForType(type);
            }

            return newCall(length);
        }

        private static Func<int, object> AddArrayForType(Type type)
        {
            var arrayType = type.MakeArrayType();
            var constructor = arrayType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 1);
            var lengthParameter = Expression.Parameter(typeof(int), "length");
            var newExpression = Expression.New(constructor, lengthParameter);
            var newCall = (Func<int, object>)Expression.Lambda(typeof(Func<int, object>), newExpression, lengthParameter).Compile();
            _arrayConstructors.Add(type, newCall);
            return newCall;
        }

        private static Func<int, object> AddListForType(Type type)
        {
            var listType = typeof(List<>).MakeGenericType(type);
            var constructor = listType.GetConstructors()
                .FirstOrDefault(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(int));
            var lengthParameter = Expression.Parameter(typeof(int), "length");
            var newExpression = Expression.New(constructor, lengthParameter);
            var newCall = (Func<int, object>)Expression.Lambda(typeof(Func<int, object>), newExpression, lengthParameter).Compile();
            _listConstructors.Add(type, newCall);
            return newCall;
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
