using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper.Benchmark.NestedCollectionsMapping
{
    class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<NestedX, NestedY>();
            CreateMap<X, Y>();
        }
    }
}
