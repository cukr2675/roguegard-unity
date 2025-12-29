using Lysionium;
using Roguegard;
using Roguegard.Device;
using System.Collections.Generic;

namespace RoguegardUnity
{
    internal class ListuiScreenQueue
    {
        private readonly Queue<IListuiScreen<MMgrBase, MArg>> screens;
        private readonly Queue<RogueObj> selfs;
        private readonly Queue<RogueObj> users;
        private readonly Queue<RogueMethodArgument> args;

        public ListuiScreenQueue()
        {
            screens = new Queue<IListuiScreen<MMgrBase, MArg>>();
            selfs = new Queue<RogueObj>();
            users = new Queue<RogueObj>();
            args = new Queue<RogueMethodArgument>();
        }

        public void Enqueue(IListuiScreen<MMgrBase, MArg> screen, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            screens.Enqueue(screen);
            selfs.Enqueue(self);
            users.Enqueue(user);
            args.Enqueue(arg);
        }

        public void Dequeue(out IListuiScreen<MMgrBase, MArg> screen, out RogueObj self, out RogueObj user, out RogueMethodArgument arg)
        {
            screen = screens.Dequeue();
            self = selfs.Dequeue();
            user = users.Dequeue();
            arg = args.Dequeue();
        }
    }
}
