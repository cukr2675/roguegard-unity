using Lysionium;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMemberMenu : RogueListuiScreen, IListuiScreen<MMgr, MArg>
    {
        private readonly ObjsMenu objsMenu;
        private readonly ObjCommandMenuScreen objCommandMenu;
        private readonly SkillsMenu skillsMenu;

        private readonly MainMenuViewData<MMgr, MArg> view = new()
        {
            PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
        };

        public override bool IsIncremental => true;

        public PartyMemberMenu(ObjsMenu objsMenu, ObjCommandMenuScreen objCommandMenu, SkillsMenu skillsMenu)
        {
            this.objsMenu = objsMenu;
            this.objCommandMenu = objCommandMenu;
            this.skillsMenu = skillsMenu;
        }

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            view.Show(manager, arg)
                ?
                .Tail.Option(objCommandMenu.Summary)

                .Option(":Items", (manager, arg) =>
                {
                    manager.PushScreen(objsMenu.Items, arg.Self, targetObj: arg.Self);
                })

                .Option(":Skills", (manager, arg) =>
                {
                    manager.PushScreen(skillsMenu.Use, arg.Self);
                })

                .Back()

                .Build();
        }

        public override void CloseScreenView(MMgr manager, bool back)
        {
            view.Hide(manager, back);
        }
    }
}
