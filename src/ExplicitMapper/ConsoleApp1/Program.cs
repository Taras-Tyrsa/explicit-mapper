using ExplicitMapper;
using System;

namespace ConsoleApp1
{
    public class X
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
    }

    public class Y
    {
        public int Y1 { get; set; }
        public string Y2 { get; set; }
    }

    public class XMapping : MappingConfiguration
    {
        public string Convert(int x)
        {
            return x.ToString();
        }

        public XMapping()
        {
            CreateMap<X, Y>()
                .For(y => y.Y1, x => x.X1)
                .For(y => y.Y2, x => x.X2.ToString());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            new XMapping();

            Mapper.Build();

            X x = new X()
            {
                X1 = 1,
                X2 = 2
            };

            Y y = Mapper.Map<Y>(x);
        }
    }
}
