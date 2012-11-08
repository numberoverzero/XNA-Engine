namespace Engine.UI
{
    public struct Alignment
    {
        public HorizontalAlignment Horizontal;
        public VerticalAlignment Vertical;

        public Alignment(VerticalAlignment valign, HorizontalAlignment halign)
        {
            Vertical = valign;
            Horizontal = halign;
        }

        public static Alignment Centered
        {
            get { return new Alignment(VerticalAlignment.Center, HorizontalAlignment.Center); }
        }
    }
}