using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public class MArg : IListMenuArg
    {
        private readonly Builder source;
        public RogueObj Self => source.Self;
        public RogueObj User => source.User;
        public RogueMethodArgument Arg => source.Arg;

        private MArg(Builder source)
        {
            this.source = source;
        }

        public void CopyTo(ref IListMenuArg dest)
        {
            if (!(dest is Copy destArg)) { dest = destArg = new Copy(); }

            destArg.Self = Self;
            destArg.User = User;
            destArg.Arg = Arg;
        }

        private class Copy : IListMenuArg
        {
            public RogueObj Self { get; set; }
            public RogueObj User { get; set; }
            public RogueMethodArgument Arg { get; set; }

            public void CopyTo(ref IListMenuArg dest)
            {
                if (!(dest is Copy destArg)) { dest = destArg = new Copy(); }

                destArg.Self = Self;
                destArg.User = User;
                destArg.Arg = Arg;
            }
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
