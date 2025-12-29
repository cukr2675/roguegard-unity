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
    public class ObjCommandMenuScreen : RogueListuiScreen
    {
        private readonly List<IObjCommand> commands;
        private readonly SelectOptionList<MMgr, MArg> selectOptions;
        private readonly SummaryScreen summaryScreen = new();
        private readonly DetailsScreen detailsScreen = new();
        private readonly RenameDialog renameDialog = new();

        public ISelectOption<MMgr, MArg> Summary { get; }
        public ISelectOption<MMgr, MArg> Details { get; }
        public ISelectOption<MMgr, MArg> Rename { get; }

        private readonly CommandListMenuViewData<ISelectOption<MMgr, MArg>, MMgr, MArg> view = new()
        {
        };

        public override bool IsIncremental => true;

        public ObjCommandMenuScreen()
        {
            commands = new List<IObjCommand>();
            selectOptions = new SelectOptionList<MMgr, MArg>();

            Summary = SelectOption.Create<MMgr, MArg>("つよさ", (manager, arg) =>
            {
                manager.PushScreen(summaryScreen, arg.Self, targetObj: arg.Arg.TargetObj, other: arg.Arg.Other);
            });
            Details = SelectOption.Create<MMgr, MArg>("説明", (manager, arg) =>
            {
                manager.PushScreen(detailsScreen, arg);
            });
            Rename = SelectOption.Create<MMgr, MArg>("名前変更", (manager, arg) =>
            {
                manager.PushScreen(renameDialog, arg);
            });
        }

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            var tool = arg.Arg.Tool;
            RoguegardSettings.ObjCommandTable.GetCommands(arg.Self, tool, commands);
            selectOptions.Clear();
            foreach (var command in commands)
            {
                selectOptions.Option(command.SelectOption);
            }

            view.Title = StandardRogueDeviceUtility.GetCaption(tool.Main.InfoSet);

            view.Show(selectOptions, manager, arg)
                ?
                .Tail.Option(Details)
                .Tail.Option(Rename)
                .Tail.Back()
                .Build();
        }

        public override void CloseScreenView(MMgr manager, bool back)
        {
            view.Hide(manager, back);
        }

        private class SummaryScreen : RogueListuiScreen
        {
            public override void OpenScreen(MMgr manager, MArg arg)
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

                manager.Summary.SetObj(target, manager);
                manager.Summary.Show();
            }
        }

        private class DetailsScreen : RogueListuiScreen
        {
            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                DialogSubviewSelector = m => m.Widgets,
            };

            public override void OpenScreen(MMgr manager, MArg arg)
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

        private class RenameDialog : RogueListuiScreen
        {
            private string newName;

            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                view.Show("", manager, arg)
                    ?
                    .Tail.Append(InputFieldWidgetOption.Create<MMgr, MArg>(
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

                    .Tail.Append(StackWidgetOption.Create(
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
                            manager.PopScreen(2);
                            manager.Reopen();
                        })),
                        ("1*", BackSelectOption<MMgr, MArg>.Instance)))

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.Hide(manager, back);
            }
        }
    }
}
