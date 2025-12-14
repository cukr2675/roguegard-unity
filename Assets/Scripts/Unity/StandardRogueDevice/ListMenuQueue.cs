using Lysionium;
using Roguegard;
using Roguegard.Device;
using System.Collections.Generic;

namespace RoguegardUnity
{
    internal class ListMenuQueue
    {
        private readonly Queue<IMenuScreen<MMgrBase, MArg>> menus;
        private readonly Queue<RogueObj> selfs;
        private readonly Queue<RogueObj> users;
        private readonly Queue<RogueMethodArgument> args;

        public ListMenuQueue()
        {
            menus = new Queue<IMenuScreen<MMgrBase, MArg>>();
            selfs = new Queue<RogueObj>();
            users = new Queue<RogueObj>();
            args = new Queue<RogueMethodArgument>();
        }

        public void Enqueue(IMenuScreen<MMgrBase, MArg> menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            menus.Enqueue(menu);
            selfs.Enqueue(self);
            users.Enqueue(user);
            args.Enqueue(arg);
        }

        public void Dequeue(out IMenuScreen<MMgrBase, MArg> menu, out RogueObj self, out RogueObj user, out RogueMethodArgument arg)
        {
            menu = menus.Dequeue();
            self = selfs.Dequeue();
            user = users.Dequeue();
            arg = args.Dequeue();
        }
    }
}
