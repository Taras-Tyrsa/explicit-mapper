using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ExplicitMapper.Tests.Errors.BaseMappingNotConfigured
{
    [Collection("Integration tests")]
    [Trait("Errors", "Base mapping not configured")]
    public class BaseMappingNotConfiguredTests : IDisposable
    {
        [Fact(DisplayName = "Base mapping not configured")]
        public void BaseMappingNotConfigured()
        {
            MappingConfiguration.Add<Configuration>();

            Action act = () => MappingConfiguration.Build();
            act.Should().ThrowExactly<ExplicitMapperException>()
                .WithMessage($"Missing mapping configuration for source type {typeof(ChildX1).FullName} and destination type {typeof(ChildY1).FullName}");
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
