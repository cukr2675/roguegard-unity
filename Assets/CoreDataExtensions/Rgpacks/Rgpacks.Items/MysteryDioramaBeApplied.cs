using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using ListingMF;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public class MysteryDioramaBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private ScriptableStartingItem _newFloor = null;

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
            public ScriptableStartingItem _newFloor;

            private static readonly List<object> elms = new();

            private readonly ScrollViewTemplate<object, RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
                ScrollSubViewName = StandardSubViewTable.WidgetsName,
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                var diorama = arg.Arg.TargetObj;
                var dioramaFloorObjs = diorama.Space.Objs;
                elms.Clear();
                for (int i = 0; i < dioramaFloorObjs.Count; i++)
                {
                    var dioramaFloorObj = dioramaFloorObjs[i];
                    if (dioramaFloorObj == null) continue;

                    elms.Add(dioramaFloorObj);
                }

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .InsertNext(
                        new object[]
                        {
                            "アセットID",
                            InputFieldViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>(
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
                                })
                        })

                    .ElementNameFrom((element, manager, arg) =>
                    {
                        if (element is RogueObj dioramaFloorObj)
                        {
                            return dioramaFloorObj.GetName();
                        }
                        else
                        {
                            return SelectOptionHandler.Instance.GetName(element, manager, arg);
                        }
                    })

                    .VariableOnce(out var nextMenu, new FloorMenu())
                    .OnClickElement((element, manager, arg) =>
                    {
                        if (element is RogueObj dioramaFloorObj)
                        {
                            manager.PushMenuScreen(nextMenu, arg.Self, targetObj: dioramaFloorObj);
                        }
                        else
                        {
                            SelectOptionHandler.Instance.HandleClick(element, manager, arg);
                        }
                    })

                    .Append(SelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(
                        "+ 階層を追加",
                        (manager, arg) =>
                        {
                            var diorama = arg.Arg.TargetObj;
                            _newFloor.Option.CreateObj(_newFloor, diorama, Vector2Int.zero, RogueRandom.Primary);
                            manager.Reopen();
                        }))

                    .Build();
            }
        }

        private class FloorMenu : RogueMenuScreen
        {
            public ScriptableStartingItem _newFloor;

            private readonly DialogViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
                DialogSubViewName = StandardSubViewTable.WidgetsName,
                BackAnchorSubViewName = StandardSubViewTable.BackAnchorName,
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                view.ShowTemplate("", manager, arg)
                    ?
                    .Append(
                        new object[]
                        {
                            "アセットID",
                            InputFieldViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>(
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
                                })
                        })

                    .AppendSelectOption(
                        "入る", (manager, arg) =>
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
