using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ExplicitMapper.Tests.Integration.SameMappingConfiguredTwice.ObjectInitializerConfiguration
{
    [Collection("Integration tests")]
    [Trait("Integration", "Same mapping configured twice")]
    public class SameMappingConfiguredTwiceTests : IDisposable
    {
        [Fact(DisplayName = "Use object initializer configuration -> ExplicitMapperException thrown")]
        public void ExceptionShouldBeThrown()
        {
            MappingConfiguration.Add<XtoYObjectInitializerConfiguration1>();
            MappingConfiguration.Add<XtoYObjectInitializerConfiguration2>();

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
