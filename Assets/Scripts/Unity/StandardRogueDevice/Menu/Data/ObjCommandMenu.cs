using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;
using Roguegard;
using Roguegard.Device;
using Roguegard.Extensions;

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

        private readonly CommandListViewTemplate<ISelectOption, MMgr, MArg> view = new()
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
            selectOptions.Add(Details);
            selectOptions.Add(Rename);
            selectOptions.Add(BackSelectOption.Instance);

            view.Title = StandardRogueDeviceUtility.GetCaption(tool.Main.InfoSet);

            view.ShowTemplate(selectOptions, manager, arg)
                ?
                .Build();
        }

        public override void CloseScreenView(MMgr manager, bool back)
        {
            view.HideTemplate(manager, back);
        }

        private class SummaryMenuScreen : RogueMenuScreen
        {
            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                DialogSubViewName = StandardSubViewTable.WidgetsName,
            };

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

                var summary = RoguegardSubViews.GetSummary(manager);
                summary.SetObj(target, manager);
                summary.Show();
            }
        }

        private class DetailsMenuScreen : RogueMenuScreen
        {
            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                DialogSubViewName = StandardSubViewTable.WidgetsName,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var obj = arg.Arg.Tool ?? arg.Arg.TargetObj;
                var description = obj?.Main.InfoSet ?? arg.Arg.Other as IRogueDescription;
                var details = "";
                if (description != null) { details = StandardRogueDeviceUtility.GetDescription(description); }

                view.ShowTemplate(details ?? "", manager, arg)
                    ?
                    .Build();
            }
        }

        private class RenameDialog : RogueMenuScreen
        {
            private string newName;

            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                BackAnchorSubViewName = null,
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate("", manager, arg)
                    ?
                    .Append(InputFieldViewWidget.CreateOption<MMgr, MArg>(
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

                    .Append(new object[]
                    {
                        SelectOption.Create<MMgr, MArg>(":Rename", (manager, arg) =>
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
                        }),
                        BackSelectOption.Instance
                    })

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.HideTemplate(manager, back);
            }
        }
    }
}
