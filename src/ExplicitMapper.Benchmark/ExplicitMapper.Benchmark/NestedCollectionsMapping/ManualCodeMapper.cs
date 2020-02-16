using System.Collections.Generic;

namespace ExplicitMapper.Benchmark.NestedCollectionsMapping
{
    class ManualCodeMapper
    {
        public static NestedY Map(NestedX x)
        {
            return new NestedY()
            {
                P1 = x.P1,
                P2 = x.P2
            };
        }

        public static Y Map(X x)
        {
            NestedY[] array = null;
            List<NestedY> list = null;

            if (x.ArrayP != null)
            {
                array = new NestedY[x.ArrayP.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = Map(x.ArrayP[i]);
                }
            }

            if (x.ListP != null)
            {
                list = new List<NestedY>(x.ListP.Count);

                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = Map(x.ListP[i]);
                }
            }

            return new Y()
            {
                ArrayP = array,
                ListP = list
            };
        }
    }
}
