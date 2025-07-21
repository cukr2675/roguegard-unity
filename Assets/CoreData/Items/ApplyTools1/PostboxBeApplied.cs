using Lysionium;
using Roguegard.Device;
using System.Collections.Generic;

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
                foreach (var post in info.Posts)
                {
                    posts.Add(post);
                }

                view.ShowTemplate(posts, manager, arg)
                    ?
                    .NameFrom((post, manager, arg) => post.Name)

                    .VarOnce(out var nextScreen, new DetailsScreen())
                    .OnClick((post, manager, arg) => manager.PushMenuScreen(nextScreen, other: post))

                    .Build();
            }
        }

        private class DetailsScreen : RogueMenuScreen
        {
            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                DialogSubviewName = StandardSubviewTable.WidgetsName,
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
