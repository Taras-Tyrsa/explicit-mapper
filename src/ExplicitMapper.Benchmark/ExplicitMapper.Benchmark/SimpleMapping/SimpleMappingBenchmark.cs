using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using IExplicitMapper = ExplicitMapper.IMapper;
using IAutoMapper = AutoMapper.IMapper;
using AutoMapper;

namespace ExplicitMapper.Benchmark.SimpleMapping
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [RPlotExporter]
    public class SimpleMappingBenchmark
    {
        private X[] _source;
        private Y[] _dest;

        private IExplicitMapper _explicitMapper;
        private IAutoMapper _autoMapper;

        [Params(100000, 1000000)]
        public int N;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _source = new X[N];
            _dest = new Y[N];

            MappingConfiguration.Add<ExplicitMapperConfiguration>();
            MappingConfiguration.Build();
            _explicitMapper = new MapperInstance();

            var automapperConfig = new MapperConfiguration(c => c.AddProfile(new AutoMapperProfile()));
            automapperConfig.AssertConfigurationIsValid();
            _autoMapper = automapperConfig.CreateMapper();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            MappingConfiguration.Clear();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            var random = new Random();

            for (int i = 0; i < N; i++)
            {
                _source[i] = new X()
                {
                    P1 = Guid.NewGuid().ToString(),
                    P2 = Guid.NewGuid().ToString(),
                    P3 = random.Next(),
                    P4 = random.Next(),
                    P5 = Guid.NewGuid(),
                    P6 = Guid.NewGuid(),
                    P7 = random.Next(),
                    P8 = random.Next()
                };
            }
        }

        [Benchmark]
        public void ExplicitMapper()
        {
            for (int i = 0; i < N; i++)
            {
                _dest[i] = _explicitMapper.Map<Y>(_source[i]);
            }
        }

        [Benchmark]
        public void AutoMapper()
        {
            for (int i = 0; i < N; i++)
            {
                _dest[i] = _autoMapper.Map<Y>(_source[i]);
            }
        }
    }
}
