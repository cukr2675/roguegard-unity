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

        private MysteryDioramaScreen mysteryDioramaScreen;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            mysteryDioramaScreen ??= new MysteryDioramaScreen(_newFloor);
            var mysteryDioramaInfo = MysteryDioramaInfo.Get(self);
            if (mysteryDioramaInfo == null)
            {
                MysteryDioramaInfo.SetTo(self);
            }

            RogueDevice.Primary.AddScreen(mysteryDioramaScreen, user, null, new(targetObj: self));
            return false;
        }

        private class MysteryDioramaScreen : RogueListuiScreen
        {
            private readonly ScrollMenuViewData<RogueObj, MMgr> view = new()
            {
                ScrollSubviewSelector = m => m.Widgets,
            };

            public MysteryDioramaScreen(AssetStartingItem newFloor)
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(Arg.Arg.TargetObj.Space.Objs, manager)
                    ?
                    .Filter(obj => obj != null)

                    .Head.Append(StackWidgetOption.Create(
                        ("1*", "アセットID"),
                        ("1*", InputFieldWidgetOption.Create<MMgr>(
                            _ =>
                            {
                                var diorama = Arg.Arg.TargetObj;
                                return NamingEffect.Get(diorama)?.Naming;
                            },
                            value =>
                            {
                                var diorama = Arg.Arg.TargetObj;
                                default(IActiveRogueMethodCaller).Affect(diorama, 1f, NamingEffect.Callback);
                                return NamingEffect.Get(diorama).Naming = value;
                            }))))

                    .NameFrom(dioramaFloorObj => dioramaFloorObj.GetName())

                    .VarOnce(out var nextScreen, new FloorScreen())
                    .OnClick((dioramaFloorObj, manager) => manager.PushScreen(nextScreen, Arg.Self, targetObj: dioramaFloorObj))

                    .Tail.Option("+ 階層を追加", (manager) =>
                    {
                        var diorama = Arg.Arg.TargetObj;
                        newFloor.Option.CreateObj(newFloor, diorama, Vector2Int.zero, RogueRandom.Primary);
                        manager.Reopen();
                    })

                    .Build();
                };
            }
        }

        private class FloorScreen : RogueListuiScreen
        {
            private readonly DialogViewData<MMgr> view = new()
            {
                DialogSubviewSelector = m => m.Widgets,
            };

            public FloorScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show("", manager)
                    ?
                    .Tail.Append(StackWidgetOption.Create(
                        ("1*", "アセットID"),
                        ("1*", InputFieldWidgetOption.Create<MMgr>(
                            _ =>
                            {
                                var diorama = Arg.Arg.TargetObj;
                                return NamingEffect.Get(diorama)?.Naming;
                            },
                            value =>
                            {
                                var diorama = Arg.Arg.TargetObj;
                                default(IActiveRogueMethodCaller).Affect(diorama, 1f, NamingEffect.Callback);
                                return NamingEffect.Get(diorama).Naming = value;
                            }))))

                    .Tail.Option("入る", (manager) =>
                    {
                        var dioramaFloor = Arg.Arg.TargetObj;
                        SpaceUtility.TryLocate(Arg.Self, dioramaFloor, Vector2Int.one);
                        manager.Done();
                    })

                    .Build();
                };
            }
        }
    }
}
