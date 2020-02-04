using System.Collections.Generic;

namespace ExplicitMapper.Tests.Integration.NestedCollectionsMapping
{
    public class X_IEnumerable
    {
        public IEnumerable<NestedX> X { get; set; }
    }

    public class X_TypedArray
    {
        public NestedX[] X { get; set; }
    }

    public class NestedX
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
    }

    public class Y_IEnumerable
    {
        public IEnumerable<NestedY> Y { get; set; }
    }

    public class Y_TypedArray
    {
        public NestedY[] Y { get; set; }
    }

    public class Y_IList
    {
        public IList<NestedY> Y { get; set; }
    }

    public class Y_List
    {
        public List<NestedY> Y { get; set; }
    }

    public class Y_ICollection
    {
        public ICollection<NestedY> Y { get; set; }
    }

    public class NestedY
    {
        public int Y1 { get; set; }
        public int Y2 { get; set; }
    }
}
