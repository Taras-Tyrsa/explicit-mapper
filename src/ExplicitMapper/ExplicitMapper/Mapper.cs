using System;

namespace ExplicitMapper
{
    public static class Mapper
    {
        public static TDest Map<TDest>(object source)
            where TDest : new()
        {
            if (MappingConfiguration.MapExpressions == null)
            {
                throw new Exception();
            }

            if (!MappingConfiguration.MapExpressions.TryGetValue((source.GetType(), typeof(TDest)), out var func))
            {
                throw new Exception();
            }

            var dest = new TDest();
            func.DynamicInvoke(source, dest);
            return dest;
        }
    }
}
