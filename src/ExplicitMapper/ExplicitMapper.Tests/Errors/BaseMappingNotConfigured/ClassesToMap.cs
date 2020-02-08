namespace ExplicitMapper.Tests.Errors.BaseMappingNotConfigured
{
    public abstract class BaseX
    {
        public int X1 { get; set; }
        public int X2 { get; set; }
    }

    public class ChildX1 : BaseX
    {
        public int X3 { get; set; }
        public int X4 { get; set; }
    }

    public class ChildX2 : ChildX1
    {
        public int X5 { get; set; }
        public int X6 { get; set; }
    }

    public abstract class BaseY
    {
        public int Y { get; set; }
        public int Y1 { get; set; }
        public int Y2 { get; set; }
    }

    public class ChildY1 : BaseY
    {
        public int Y3 { get; set; }
        public int Y4 { get; set; }
    }

    public class ChildY2 : ChildY1
    {
        public int Y5 { get; set; }
        public int Y6 { get; set; }
    }
}
