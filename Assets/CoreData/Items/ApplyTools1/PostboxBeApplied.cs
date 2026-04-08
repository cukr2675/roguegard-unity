using Lysionium;
using Roguegard.Device;
using System.Collections.Generic;

namespace Roguegard
{
    public class PostboxBeApplied : BaseApplyRogueMethod
    {
        private static PostboxScreen postboxScreen;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (user == RogueDevice.Primary.Player)
            {
                postboxScreen ??= new PostboxScreen();
                RogueDevice.Primary.AddScreen(postboxScreen, user, null, new(tool: self));
                return false;
            }
            else
            {
                return false;
            }
        }

        private class PostboxScreen : RogueListuiScreen
        {
            private readonly ScrollMenuViewData<RoguePost, MMgr> view = new()
            {
            };

            public PostboxScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    var info = PostboxInfo.Get(Arg.Arg.Tool);

                    view.Show(info.Posts, manager)
                    ?
                    .NameFrom((post, manager) => post.Name)

                    .VarOnce(out var nextScreen, new DetailsScreen())
                    .OnClick((post, manager) => manager.PushScreen(nextScreen, other: post))

                    .Build();
                };
            }
        }

        private class DetailsScreen : RogueListuiScreen
        {
            private readonly DialogViewData<MMgr> view = new()
            {
                DialogSubviewSelector = m => m.Widgets,
            };

            public DetailsScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    var post = (RoguePost)Arg.Arg.Other;

                    view.Show(post.Name, manager)
                    ?
                    .Build();
                };
            }
        }
    }
}
