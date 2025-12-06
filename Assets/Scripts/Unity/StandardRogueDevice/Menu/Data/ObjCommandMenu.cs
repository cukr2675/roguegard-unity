using Lysionium;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;
using System.Collections.Generic;

namespace RoguegardUnity
{
    /// <summary>
    /// <see cref="RogueObj"/> に対するコマンド（食べる・投げるなど）を選択するメニュー。
    /// </summary>
    public class ObjCommandMenu : RogueMenuScreen
    {
        private readonly List<IObjCommand> commands;
        private readonly List<ISelectOption> selectOptions;
        private readonly SummaryMenuScreen summaryMenuScreen = new();
        private readonly DetailsMenuScreen detailsMenuScreen = new();
        private readonly RenameDialog renameDialog = new();

        public ISelectOption Summary { get; }
        public ISelectOption Details { get; }
        public ISelectOption Rename { get; }

        private readonly CommandListMenuViewData<ISelectOption, MMgr, MArg> view = new()
        {
        };

        public override bool IsIncremental => true;

        public ObjCommandMenu()
        {
            commands = new List<IObjCommand>();
            selectOptions = new List<ISelectOption>();

            Summary = SelectOption.Create<MMgr, MArg>("つよさ", (manager, arg) =>
            {
                manager.PushMenuScreen(summaryMenuScreen, arg.Self, targetObj: arg.Arg.TargetObj, other: arg.Arg.Other);
            });
            Details = SelectOption.Create<MMgr, MArg>("説明", (manager, arg) =>
            {
                manager.PushMenuScreen(detailsMenuScreen, arg);
            });
            Rename = SelectOption.Create<MMgr, MArg>("名前変更", (manager, arg) =>
            {
                manager.PushMenuScreen(renameDialog, arg);
            });
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            var tool = arg.Arg.Tool;
            RoguegardSettings.ObjCommandTable.GetCommands(arg.Self, tool, commands);
            selectOptions.Clear();
            foreach (var command in commands)
            {
                selectOptions.Add(command.SelectOption);
            }

            view.Title = StandardRogueDeviceUtility.GetCaption(tool.Main.InfoSet);

            view.Show(selectOptions, manager, arg)
                ?
                .Tail(Details)
                .Tail(Rename)
                .Tail(BackSelectOption.Instance)
                .Build();
        }

        public override void CloseScreenView(MMgr manager, bool back)
        {
            view.Hide(manager, back);
        }

        private class SummaryMenuScreen : RogueMenuScreen
        {
            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                object target;
                if (arg.Arg.TargetObj != null)
                {
                    target = arg.Arg.TargetObj;
                }
                else if (arg.Arg.Other is IRogueTile tile)
                {
                    target = tile;
                }
                else
                {
                    target = arg;
                }

                var summary = RoguegardSubviews.GetSummary(manager);
                summary.SetObj(target, manager);
                summary.Show();
            }
        }

        private class DetailsMenuScreen : RogueMenuScreen
        {
            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                DialogSubviewSelector = m => m.Widgets,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var obj = arg.Arg.Tool ?? arg.Arg.TargetObj;
                var describable = obj?.Main.InfoSet ?? arg.Arg.Other as IRogueDescribable;
                var details = "";
                if (describable != null) { details = StandardRogueDeviceUtility.GetDescription(describable); }

                view.Show(details ?? "", manager, arg)
                    ?
                    .Build();
            }
        }

        private class RenameDialog : RogueMenuScreen
        {
            private string newName;

            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show("", manager, arg)
                    ?
                    .Tail(InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            var obj = arg.Arg.Tool ?? arg.Arg.TargetObj;
                            return newName = NamingEffect.Get(obj)?.Naming;
                        },
                        (manager, arg, value) =>
                        {
                            return newName = value;
                        })
                    )

                    .Tail(StackWidgetOption.Create(
                        ("1*", SelectOption.Create<MMgr, MArg>(":Rename", (manager, arg) =>
                        {
                            var obj = arg.Arg.Tool ?? arg.Arg.TargetObj;
                            default(IActiveRogueMethodCaller).Affect(obj, 1f, NamingEffect.Callback);
                            if (string.IsNullOrWhiteSpace(newName))
                            {
                                BaseStatusEffect.Close<NamingEffect>(obj);
                            }
                            else
                            {
                                NamingEffect.Get(obj).Naming = newName;
                            }
                            manager.PopMenuScreen(2);
                            manager.Reopen();
                        })),
                        ("1*", BackSelectOption.Instance)))

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.Hide(manager, back);
            }
        }
    }
}
