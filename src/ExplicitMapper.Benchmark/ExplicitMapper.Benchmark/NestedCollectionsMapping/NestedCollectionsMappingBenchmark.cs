using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using IExplicitMapper = ExplicitMapper.IMapper;
using IAutoMapper = AutoMapper.IMapper;
using AutoMapper;
using System.Collections.Generic;

namespace ExplicitMapper.Benchmark.NestedCollectionsMapping
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [RPlotExporter]
    public class NestedCollectionsMappingBenchmark
    {
        private X[] _source;
        private Y[] _dest;

        private IExplicitMapper _explicitMapper;
        private IAutoMapper _autoMapper;

        [Params(10000, 100000)]
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
                int size = i % 100 + 1;

                _source[i] = new X()
                {
                    ArrayP = new NestedX[size],
                    ListP = new List<NestedX>(size)
                };

                for (int j = 0; j < size; j++)
                {
                    _source[i].ArrayP[j] = new NestedX()
                    {
                        P1 = random.Next(),
                        P2 = random.Next()
                    };
                    _source[i].ListP.Add(new NestedX()
                    {
                        P1 = random.Next(),
                        P2 = random.Next()
                    });
                }
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

        [Benchmark(Baseline = true)]
        public void ManualCodeMapping()
        {
            for (int i = 0; i < N; i++)
            {
                _dest[i] = ManualCodeMapper.Map(_source[i]);
            }
        }
    }
}
