namespace Lysionium
{
    public interface IDefaultSubviewTable
    {
        public IListHandlerSubview PlayingHud { get; }
        public IListHandlerSubview Scroll { get; }
        public IListHandlerSubview Widgets { get; }
        public IMessageBoxSubview LongMessage { get; }
        public IListHandlerSubview BackAnchor { get; }
        public IListHandlerSubview ForwardAnchor { get; }
        public IListHandlerSubview PrimaryCommand { get; }
        public IListHandlerSubview CaptionBox { get; }
        public IListHandlerSubview SecondaryCommand { get; }
        public IListHandlerSubview Dialog { get; }
        public IColorPickerSubview ColorPicker { get; }
        public IMessageBoxSubview MessageBox { get; }
        public IListHandlerSubview FadeMask { get; }
        public IListHandlerSubview Overlay { get; }
        public IMessageBoxSubview SpeechBox { get; }
        public IListHandlerSubview Choices { get; }
        public IListHandlerSubview DropdownList { get; }
        public IListHandlerSubview DropdownGrid { get; }
    }
}
