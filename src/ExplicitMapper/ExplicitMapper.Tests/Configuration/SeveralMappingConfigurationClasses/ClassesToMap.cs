using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper.Tests.Configuration.SeveralMappingConfigurationClasses
{
    public class Product
    {
        public string Manufacturer { get; set; }
        public string Name { get; set; }
        public Size Size { get; set; }
    }

    public class Size
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }
    }

    public class ProductViewModel
    {
        public string Description { get; set; }
        public SizeViewModel Size { get; set; }
    }

    public class SizeViewModel
    {
        public string Width { get; set; }
        public string Height { get; set; }
        public string Depth { get; set; }
    }

    public class X
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
    }

    public class Y
    {
        public int Y1 { get; set; }
        public int Y2 { get; set; }
    }
}
