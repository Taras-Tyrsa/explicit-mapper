using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper.Tests.Integration.SameMappingConfiguredTwice.StandardConfiguration
{
    public class XtoYStandardConfiguration1 : MappingConfiguration
    {
        public XtoYStandardConfiguration1()
        {
            CreateMap<X, Y>()
                .For(y => y.Y1, x => x.X1)
                .For(y => y.Y2, x => x.X2);
        }
    }
}
