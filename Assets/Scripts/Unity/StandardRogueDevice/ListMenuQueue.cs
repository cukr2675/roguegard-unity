using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class ListMenuQueue
    {
        private readonly Queue<RogueMenuScreen> menus;
        private readonly Queue<RogueObj> selfs;
        private readonly Queue<RogueObj> users;
        private readonly Queue<RogueMethodArgument> args;

        public ListMenuQueue()
        {
            menus = new Queue<RogueMenuScreen>();
            selfs = new Queue<RogueObj>();
            users = new Queue<RogueObj>();
            args = new Queue<RogueMethodArgument>();
        }

        public void Enqueue(RogueMenuScreen menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            menus.Enqueue(menu);
            selfs.Enqueue(self);
            users.Enqueue(user);
            args.Enqueue(arg);
        }

        public void Dequeue(out RogueMenuScreen menu, out RogueObj self, out RogueObj user, out RogueMethodArgument arg)
        {
            menu = menus.Dequeue();
            self = selfs.Dequeue();
            user = users.Dequeue();
            arg = args.Dequeue();
        }
    }
}
