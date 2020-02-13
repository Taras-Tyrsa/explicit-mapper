using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper.Benchmark.SimpleMapping
{
    class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<X, Y>();
        }
    }
}
