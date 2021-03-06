﻿using ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice.StandardConfiguration
{
    [Collection("Integration tests")]
    [Trait("Errors", "Same mapping configured twice")]
    public class SameMappingConfiguredTwiceTests : IDisposable
    {
        [Fact(DisplayName = "Use standard configuration -> ExplicitMapperException thrown")]
        public void ExceptionShouldBeThrown()
        {
            MappingConfiguration.Add<XtoYStandardConfiguration1>();
            MappingConfiguration.Add<XtoYStandardConfiguration2>();

            Action act = () => MappingConfiguration.Build();
            act.Should().ThrowExactly<ExplicitMapperException>()
                .WithMessage($"Duplicate mapping configuration for source type '{typeof(X).FullName}' and destination type '{typeof(Y).FullName}'");
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
