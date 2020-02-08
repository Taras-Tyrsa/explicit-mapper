using FluentAssertions;
using System;
using Xunit;

namespace ExplicitMapper.Tests.Errors.MapperNotInitialized
{
    [Collection("Integration tests")]
    [Trait("Errors", "Mapper not initialized")]
    public class MapperNotInitializedTests : IDisposable
    {
        [Fact(DisplayName = "Mapper not initialized")]
        public void BaseMappingNotConfigured()
        {
            var x = new X()
            {
                X1 = 1,
                X2 = 2,
                X3 = "3"
            };

            Action act = () => Mapper.Map<Y>(x);
            act.Should().ThrowExactly<ExplicitMapperException>()
                .WithMessage("Mapper not initialized");
        }

        public void Dispose()
        {
            MappingConfiguration.Clear();
        }
    }
}
