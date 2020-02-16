using System;
using System.Collections.Generic;
using System.Text;

namespace ExplicitMapper.Benchmark.NestedCollectionsMapping
{
    class NestedX
    {
        public int P1 { get; set; }
        public int P2 { get; set; }
    }

    class NestedY
    {
        public int P1 { get; set; }
        public int P2 { get; set; }
    }

    class X
    {
        public List<NestedX> ListP { get; set; }
        public NestedX[] ArrayP { get; set; }
    }

    class Y
    {
        public List<NestedY> ListP { get; set; }
        public NestedY[] ArrayP { get; set; }
    }
}
