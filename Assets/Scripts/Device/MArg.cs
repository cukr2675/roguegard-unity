namespace Roguegard.Device
{
    public class MArg
    {
        private readonly Builder source;
        public RogueObj Self => source.Self;
        public RogueObj User => source.User;
        public RogueMethodArgument Arg => source.Arg;

        private MArg(Builder source)
        {
            this.source = source;
        }

        private class Copy
        {
            public RogueObj Self { get; set; }
            public RogueObj User { get; set; }
            public RogueMethodArgument Arg { get; set; }
        }

        public class Builder
        {
            public RogueObj Self { get; set; }
            public RogueObj User { get; set; }
            public RogueMethodArgument Arg { get; set; }
            public MArg ReadOnly { get; }

            public Builder(RogueObj self = null, RogueObj user = null, RogueMethodArgument arg = default)
            {
                Self = self;
                User = user;
                Arg = arg;
                ReadOnly = new MArg(this);
            }
        }
    }
}
