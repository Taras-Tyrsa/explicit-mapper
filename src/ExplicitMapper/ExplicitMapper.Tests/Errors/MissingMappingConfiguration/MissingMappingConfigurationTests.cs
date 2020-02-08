using FluentAssertions;
using System;
using Xunit;

namespace ExplicitMapper.Tests.Errors.MissingMappingConfiguration
{
    [Collection("Integration tests")]
    [Trait("Errors", "Missing mapping configuration")]
    public class MissingMappingConfigurationTests : IDisposable
    {
        [Fact(DisplayName = "Missing mapping configuration")]
        public void MissingMappingConfiguration()
        {
            MappingConfiguration.Build();

            var x = new X()
            {
                X1 = 1,
                X2 = 2,
                X3 = "3"
            };

            Action act = () => Mapper.Map<Y>(x);
            act.Should().ThrowExactly<ExplicitMapperException>()
                .WithMessage($"Missing mapping configuration for source type {typeof(X).FullName} and destination type {typeof(Y).FullName}");
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
