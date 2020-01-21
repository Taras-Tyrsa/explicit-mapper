using System.Collections.Generic;

namespace ExplicitMapper
{
    public class MappingConfiguration
    {
        private static readonly List<RawMapping> _rawMappings = new List<RawMapping>();

        internal static IReadOnlyCollection<RawMapping> RawMappings => _rawMappings;

        protected TDest Map<TDest>(object source)
        {
            return default(TDest);
        }

        protected RawMapping<TSource, TDest> CreateMap<TSource, TDest>()
        {
            var mapping = new RawMapping<TSource, TDest>();
            _rawMappings.Add(mapping);
            return mapping;
        }
    }
}
