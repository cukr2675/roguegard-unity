using Lysionium;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMemberMenu : RogueListuiScreen
    {
        private readonly MainMenuViewData<MMgr> view = new()
        {
            PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
        };

        public PartyMemberMenu(ObjsMenu objsMenu, ObjCommandMenuScreen objCommandMenu, SkillsMenu skillsMenu)
        {
            OnOpenScreen += (manager) =>
            {
                view.Show(manager)
                ?
                .Tail.Option(objCommandMenu.Summary, () => Arg)

                .Option(":Items", (manager) =>
                {
                    manager.PushScreen(objsMenu.Items, Arg.Self, targetObj: Arg.Self);
                })

                .Option(":Skills", (manager) =>
                {
                    manager.PushScreen(skillsMenu.Use, Arg.Self);
                })

                .Back()

                .Build();
            };

            OnCloseScreenView += (manager, back) =>
            {
                view.Hide(manager, back);
            };
        }
    }
}
