namespace Lysionium
{
    public static class StyleMetaWidgetOption
    {
        public static IStyleMetaWidgetOption Create(string style)
        {
            return new WidgetOptionImplement()
            {
                Style = style,
            };
        }

        private class WidgetOptionImplement : IStyleMetaWidgetOption
        {
            public string Style { get; set; }
        }
    }
}
