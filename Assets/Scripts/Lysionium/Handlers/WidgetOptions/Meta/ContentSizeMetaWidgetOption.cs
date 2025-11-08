namespace Lysionium
{
    public static class ContentSizeMetaWidgetOption
    {
        public static IContentSizeMetaWidgetOption Create(float width)
        {
            return new WidgetOptionImplement()
            {
                Width = width,
            };
        }

        private class WidgetOptionImplement : IContentSizeMetaWidgetOption
        {
            public float Width { get; set; }
        }
    }
}
