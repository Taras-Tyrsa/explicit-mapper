using System;

namespace ExplicitMapper
{
    public static class Mapper
    {
        public static TDest Map<TDest>(object source)
            where TDest : new()
        {
            if (!MappingConfiguration.ProjectionExpressions.TryGetValue((source.GetType(), typeof(TDest)), out var func))
            {
                throw new Exception();
            }

            var dest = func.DynamicInvoke(source);

            return (TDest)dest;
        }
    }
}
