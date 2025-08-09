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
            private readonly ScrollMenuViewData<RoguePost, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var info = PostboxInfo.Get(arg.Arg.Tool);

                view.Show(info.Posts, manager, arg)
                    ?
                    .NameFrom((post, manager, arg) => post.Name)

                    .VarOnce(out var nextScreen, new DetailsScreen())
                    .OnClick((post, manager, arg) => manager.PushMenuScreen(nextScreen, other: post))

                    .Build();
            }
        }

        private class DetailsScreen : RogueMenuScreen
        {
            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                DialogSubviewName = StandardSubviewTable.WidgetsName,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var post = (RoguePost)arg.Arg.Other;

                view.Show(post.Name, manager, arg)
                    ?
                    .Build();
            }
        }
    }
}
