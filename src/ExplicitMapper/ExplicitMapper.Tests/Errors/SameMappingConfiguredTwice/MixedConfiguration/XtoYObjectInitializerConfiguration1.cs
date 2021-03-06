﻿using ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper.Tests.Errors.SameMappingConfiguredTwice.MixedConfiguration
{
    public class XtoYObjectInitializerConfiguration1 : MappingConfiguration
    {
        public XtoYObjectInitializerConfiguration1()
        {
            CreateMap<X, Y>(
                x => new Y()
                {
                    Y1 = x.X1,
                    Y2 = x.X2
                });
        }
    }
}
