using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public class PostboxBeApplied : BaseApplyRogueMethod
    {
        private static MenuScreen menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                menu ??= new MenuScreen();
                RogueDevice.Primary.AddMenu(menu, user, null, new(tool: self));
                return false;
            }
            else
            {
                return false;
            }
        }

        private class MenuScreen : RogueMenuScreen
        {
            private readonly List<RoguePost> posts = new();

            private readonly ScrollViewTemplate<RoguePost, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var info = PostboxInfo.Get(arg.Arg.Tool);
                posts.Clear();
                for (int i = 0; i < info.Posts.Count; i++)
                {
                    posts.Add(info.Posts[i]);
                }

                view.ShowTemplate(posts, manager, arg)
                    ?
                    .ElementNameFrom((post, manager, arg) => post.Name)

                    .VariableOnce(out var nextScreen, new DetailsScreen())
                    .OnClickElement((post, manager, arg) => manager.PushMenuScreen(nextScreen, other: post))

                    .Build();
            }
        }

        private class DetailsScreen : RogueMenuScreen
        {
            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                DialogSubViewName = StandardSubViewTable.WidgetsName,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var post = (RoguePost)arg.Arg.Other;

                view.ShowTemplate(post.Name, manager, arg)
                    ?
                    .Build();
            }
        }
    }
}
