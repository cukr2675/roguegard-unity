using Lysionium;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class PartyMemberMenu : RogueMenuScreen, IMenuScreen<MMgr, MArg>
    {
        private readonly ObjsMenu objsMenu;
        private readonly ObjCommandMenu objCommandMenu;
        private readonly SkillsMenu skillsMenu;

        private readonly MainMenuViewData<MMgr, MArg> view = new()
        {
            PrimaryCommandSubviewSelector = m => m.SecondaryCommand,
        };

        public override bool IsIncremental => true;

        public PartyMemberMenu(ObjsMenu objsMenu, ObjCommandMenu objCommandMenu, SkillsMenu skillsMenu)
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
                    manager.PushMenuScreen(objsMenu.Items, arg.Self, targetObj: arg.Self);
                })

                .Option(":Skills", (manager, arg) =>
                {
                    manager.PushMenuScreen(skillsMenu.Use, arg.Self);
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
