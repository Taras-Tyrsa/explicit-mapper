using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ExplicitMapper
{
    internal static class InstanceFactory
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> _defaultConstructors = new ConcurrentDictionary<Type, Func<object>>();
        private static readonly ConcurrentDictionary<Type, Func<int, object>> _arrayConstructors = new ConcurrentDictionary<Type, Func<int, object>>();
        private static readonly ConcurrentDictionary<Type, Func<int, object>> _listConstructors = new ConcurrentDictionary<Type, Func<int, object>>();

        public static object CreateInstance(Type type)
        {
            Func<object> newCall = GetFactoryMethodForTypeWithDefaultConstructor(type);
            return newCall();
        }

        public static object CreateArray(Type type, int length)
        {
            Func<int, object> newCall = GetArrayFactoryMethodForType(type);
            return newCall(length);
        }

        public static object CreateList(Type type, int length)
        {
            Func<int, object> newCall = GetListFactoryMethodForType(type);
            return newCall(length);
        }

        private static Func<int, object> GetArrayFactoryMethodForType(Type type)
        {
            return _arrayConstructors.GetOrAdd(type, t => 
            {
                var arrayType = t.MakeArrayType();
                var constructor = arrayType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 1);
                var lengthParameter = Expression.Parameter(typeof(int), "length");
                var newExpression = Expression.New(constructor, lengthParameter);
                var newCall = (Func<int, object>)Expression.Lambda(typeof(Func<int, object>), newExpression, lengthParameter).Compile();
                return newCall;
            });
        }

        private static Func<int, object> GetListFactoryMethodForType(Type type)
        {
            return _listConstructors.GetOrAdd(type, t =>
            {
                var listType = typeof(List<>).MakeGenericType(t);
                var constructor = listType.GetConstructors()
                    .FirstOrDefault(c => c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType == typeof(int));
                var lengthParameter = Expression.Parameter(typeof(int), "length");
                var newExpression = Expression.New(constructor, lengthParameter);
                var newCall = (Func<int, object>)Expression.Lambda(typeof(Func<int, object>), newExpression, lengthParameter).Compile();
                return newCall;
            });
        }

        private static Func<object> GetFactoryMethodForTypeWithDefaultConstructor(Type type)
        {
            return _defaultConstructors.GetOrAdd(type, t =>
            {
                if (!type.GetConstructors().Any(c => c.GetParameters().Length == 0))
                {
                    throw new ExplicitMapperException($"No default constructor for type {type} exists");
                }

                var newCall = (Func<object>)Expression.Lambda(typeof(Func<object>), Expression.New(type)).Compile();
                return newCall;
            });
        }
    }
}
