using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public class MysteryDioramaBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private AssetStartingItem _newFloor = null;

        private Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new() { _newFloor = _newFloor };
            var mysteryDioramaInfo = MysteryDioramaInfo.Get(self);
            if (mysteryDioramaInfo == null)
            {
                MysteryDioramaInfo.SetTo(self);
            }

            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        private class Menu : RogueMenuScreen
        {
            public AssetStartingItem _newFloor;

            private readonly ScrollMenuViewData<RogueObj, MMgr, MArg> view = new()
            {
                ScrollSubviewSelector = m => m.Widgets,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var diorama = arg.Arg.TargetObj;

                view.Show(diorama.Space.Objs, manager, arg)
                    ?
                    .Filter(obj => obj != null)

                    .Head(StackWidgetOption.Create(
                        ("1*", "アセットID"),
                        ("1*", InputFieldWidgetOption.Create<MMgr, MArg>(
                            (manager, arg) =>
                            {
                                var diorama = arg.Arg.TargetObj;
                                return NamingEffect.Get(diorama)?.Naming;
                            },
                            (manager, arg, value) =>
                            {
                                var diorama = arg.Arg.TargetObj;
                                default(IActiveRogueMethodCaller).Affect(diorama, 1f, NamingEffect.Callback);
                                return NamingEffect.Get(diorama).Naming = value;
                            }))))

                    .NameFrom(dioramaFloorObj => dioramaFloorObj.GetName())

                    .VarOnce(out var nextMenu, new FloorMenu())
                    .OnClick((dioramaFloorObj, manager, arg) => manager.PushMenuScreen(nextMenu, arg.Self, targetObj: dioramaFloorObj))

                    .TailOption("+ 階層を追加", (manager, arg) =>
                    {
                        var diorama = arg.Arg.TargetObj;
                        _newFloor.Option.CreateObj(_newFloor, diorama, Vector2Int.zero, RogueRandom.Primary);
                        manager.Reopen();
                    })

                    .Build();
            }
        }

        private class FloorMenu : RogueMenuScreen
        {
            public AssetStartingItem _newFloor;

            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                DialogSubviewSelector = m => m.Widgets,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show("", manager, arg)
                    ?
                    .Tail(StackWidgetOption.Create(
                        ("1*", "アセットID"),
                        ("1*", InputFieldWidgetOption.Create<MMgr, MArg>(
                            (manager, arg) =>
                            {
                                var diorama = arg.Arg.TargetObj;
                                return NamingEffect.Get(diorama)?.Naming;
                            },
                            (manager, arg, value) =>
                            {
                                var diorama = arg.Arg.TargetObj;
                                default(IActiveRogueMethodCaller).Affect(diorama, 1f, NamingEffect.Callback);
                                return NamingEffect.Get(diorama).Naming = value;
                            }))))

                    .Option("入る", (manager, arg) =>
                    {
                        var dioramaFloor = arg.Arg.TargetObj;
                        SpaceUtility.TryLocate(arg.Self, dioramaFloor, Vector2Int.one);
                        manager.Done();
                    })

                    .Build();
            }
        }
    }
}
