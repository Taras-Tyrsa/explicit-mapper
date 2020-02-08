using ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice.MixedConfiguration
{
    public class XtoYStandardConfiguration2 : MappingConfiguration
    {
        public XtoYStandardConfiguration2()
        {
            CreateMap<X, Y>()
                .For(y => y.Y1, x => x.X1);
        }
    }
}
