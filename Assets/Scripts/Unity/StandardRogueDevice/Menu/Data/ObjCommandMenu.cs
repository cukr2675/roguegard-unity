using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard;
using Roguegard.Device;

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

        public ISelectOption Summary { get; }
        public ISelectOption Details { get; }

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
            selectOptions.Add(BackSelectOption.Instance);

            view.Title = StandardRogueDeviceUtility.GetCaption(tool.Main.InfoSet);

            view.ShowTemplate(selectOptions, manager, arg)
                ?
                .ElementNameFrom((selectOption, manager, arg) =>
                {
                    return selectOption.GetName(manager, arg);
                })

                .OnClickElement((selectOption, manager, arg) =>
                {
                    selectOption.HandleClick(manager, arg);
                })

                .Build();
        }

        public override void CloseScreen(MMgr manager, bool back)
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
    }
}
