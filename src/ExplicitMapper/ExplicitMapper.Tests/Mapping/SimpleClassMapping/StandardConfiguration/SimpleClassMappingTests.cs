﻿using FluentAssertions;
using System;
using Xunit;

namespace ExplicitMapper.Tests.Mapping.SimpleClassMapping.StandardConfiguration
{
    [Collection("Integration tests")]
    [Trait("Mapping", "Simple class mapping")]
    public class SimpleClassMappingTests : IDisposable
    {
        [Fact(DisplayName = "Use standard configuration")]
        public void UseStandardConfiguration_FieldsMapped()
        {
            MappingConfiguration.Add<XtoYStandardConfiguration>();
            MappingConfiguration.Build();

            var x = new X()
            {
                X1 = 1,
                X2 = 2,
                X3 = "3"
            };

            var y = Mapper.Map<Y>(x);

            y.Y1.Should().Be(x.X1);
            y.Y2.Should().Be(x.X2.ToString());
            y.Y3.Should().Be(5);
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
