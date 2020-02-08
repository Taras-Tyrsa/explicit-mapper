using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper.Tests.Mapping.NestedClassMapping
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
}
