using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class ModelsMenuQueue
    {
        private readonly Queue<IModelsMenu> menus;
        private readonly Queue<RogueObj> selfs;
        private readonly Queue<RogueObj> users;
        private readonly Queue<RogueMethodArgument> args;

        public ModelsMenuQueue()
        {
            menus = new Queue<IModelsMenu>();
            selfs = new Queue<RogueObj>();
            users = new Queue<RogueObj>();
            args = new Queue<RogueMethodArgument>();
        }

        public void Enqueue(IModelsMenu menu, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            menus.Enqueue(menu);
            selfs.Enqueue(self);
            users.Enqueue(user);
            args.Enqueue(arg);
        }

        public void Dequeue(out IModelsMenu menu, out RogueObj self, out RogueObj user, out RogueMethodArgument arg)
        {
            menu = menus.Dequeue();
            self = selfs.Dequeue();
            user = users.Dequeue();
            arg = args.Dequeue();
        }
    }
}
