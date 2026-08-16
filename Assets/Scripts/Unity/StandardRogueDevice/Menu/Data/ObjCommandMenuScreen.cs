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
        private readonly List<ISelectOption<MMgr, MArg>> selectOptions;
        private readonly SummaryScreen summaryScreen = new();
        private readonly DetailsScreen detailsScreen = new();
        private readonly RenameDialog renameDialog = new();

        public ISelectOption<MMgr, MArg> Summary { get; }
        public ISelectOption<MMgr, MArg> Details { get; }
        public ISelectOption<MMgr, MArg> Rename { get; }

        private readonly CommandListMenuViewData<ISelectOption<MMgr, MArg>, MMgr> view = new()
        {
        };

        public ObjCommandMenuScreen()
        {
            commands = new List<IObjCommand>();
            selectOptions = new List<ISelectOption<MMgr, MArg>>();

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

            OnOpenScreen += (manager) =>
            {
                var tool = Arg.Arg.Tool;
                RoguegardSettings.ObjCommandTable.GetCommands(Arg.Self, tool, commands);
                selectOptions.Clear();
                foreach (var command in commands)
                {
                    selectOptions.Add(command.SelectOption);
                }

                view.Title = StandardRogueDeviceUtility.GetCaption(tool.Main.InfoSet);

                view.Show(selectOptions, manager)
                ?
                .NameFrom((o, m) => o.GetName(m, Arg))
                .OnClick((o, m) => o.Click(m, "Click", Arg))
                .StyleFrom((o, m) => o.GetStyle(m, Arg))
                .Tail.Option(Details, () => Arg)
                .Tail.Option(Rename, () => Arg)
                .Tail.Back()
                .Build();
            };

            OnCloseScreenView += (manager, back) =>
            {
                view.Hide(manager, back);
            };
        }

        private class SummaryScreen : RogueListuiScreen
        {
            public SummaryScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    object target;
                    if (Arg.Arg.TargetObj != null)
                    {
                        target = Arg.Arg.TargetObj;
                    }
                    else if (Arg.Arg.Other is IRogueTile tile)
                    {
                        target = tile;
                    }
                    else
                    {
                        target = Arg;
                    }

                    manager.Summary.SetObj(target, manager);
                    manager.Summary.Show();
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
                    var obj = Arg.Arg.Tool ?? Arg.Arg.TargetObj;
                    var describable = obj?.Main.InfoSet ?? Arg.Arg.Other as IRogueDescribable;
                    var details = "";
                    if (describable != null) { details = StandardRogueDeviceUtility.GetDescription(describable); }

                    view.Show(details ?? "", manager)
                    ?
                    .Build();
                };
            }
        }

        private class RenameDialog : RogueListuiScreen
        {
            private readonly DialogViewData<MMgr> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public RenameDialog()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show("", manager)
                    ?
                    .VarOnce(out string newName)
                    .Tail.Append(InputFieldWidgetOption.Create<MMgr>(
                        _ =>
                        {
                            var obj = Arg.Arg.Tool ?? Arg.Arg.TargetObj;
                            return newName = NamingEffect.Get(obj)?.Naming;
                        },
                        value =>
                        {
                            return newName = value;
                        })
                    )

                    .Tail.Append(StackWidgetOption.Create(
                        ("1*", SelectOption.Create<MMgr>(":Rename", (manager) =>
                        {
                            var obj = Arg.Arg.Tool ?? Arg.Arg.TargetObj;
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
                        ("1*", BackSelectOption<MMgr>.Instance)))

                    .Build();
                };

                OnCloseScreenView += (manager, back) =>
                {
                    view.Hide(manager, back);
                };
            }
        }
    }
}
