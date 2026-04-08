using Lysionium;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using System.Collections.Generic;

namespace RoguegardUnity
{
    /// <summary>
    /// アイテム一覧を表示するメニュー。
    /// </summary>
    public class ObjsMenu
    {
        public ISelectOption<MMgr, MArg> Close { get; }

        /// <summary>
        /// <see cref="RogueMethodArgument.TargetObj"/> のインベントリを開く
        /// </summary>
        public RogueListuiScreen Items { get; }

        /// <summary>
        /// <see cref="RogueMethodArgument.TargetObj"/> の足元のアイテム一覧を開く
        /// </summary>
        public RogueListuiScreen Ground { get; }

        public RogueListuiScreen PutIntoContainer { get; }

        public RogueListuiScreen TakeOutOfContainer { get; }

        public ObjsMenu(
            ObjCommandMenuScreen commandMenuScreen,
            PutIntoContainerCommandMenuScreen putInCommandMenuScreen,
            TakeOutOfContainerCommandMenuScreen takeOutCommandMenuScreen)
        {
            Close = SelectOption.Create<MMgr, MArg>(":Close", (m, _) => m.Done(), "Cancel");
            Items = new ItemsScreen() { commandMenuScreen = commandMenuScreen };
            Ground = new GroundScreen() { commandMenuScreen = commandMenuScreen };
            PutIntoContainer = new PutIntoContainerScreen() { commandMenuScreen = putInCommandMenuScreen };
            TakeOutOfContainer = new TakeOutOfContainerScreen() { commandMenuScreen = takeOutCommandMenuScreen };
        }

        private abstract class BaseScreen : RogueListuiScreen
        {
            protected abstract string Title { get; }
            protected virtual bool Skip0WeightObjs => false;
            protected virtual bool SortIsEnabled => false;

            public RogueListuiScreen commandMenuScreen;

            private static CategorizedSortTable sortTable;

            private readonly RogueScrollMenuViewData<RogueObj> view = new()
            {
            };

            protected BaseScreen()
            {
                view.Title = Title;
                if (SortIsEnabled)
                {
                    view.BackAnchorList = new(
                        _ => _
                        .Option(":Sort", Sort, "Sort click:Sp1")
                        .Back());
                }

                OnOpenScreen += (manager) =>
                {
                    var list = GetObjs(Arg.Self, Arg.Arg.TargetObj);
                    var viewStateHolder = GetViewStateHolder(manager);

                    view.Show(list, manager, viewStateHolder)
                    ?
                    .InfoFrom((obj, manager) =>
                    {
                        var icon = obj.Main.InfoSet.Icon;
                        var color = RogueColorUtility.GetColor(obj);
                        var stack = obj.Stack;

                        var weight = WeightCalculator.Get(obj);
                        var weightText = string.Format("重:{0:0.##}", weight.TotalWeight);

                        var equipmentInfo = obj.Main.GetEquipmentInfo(obj);
                        var vehicleInfo = VehicleInfo.Get(obj);
                        var equipeed = equipmentInfo?.EquippedSubslot >= 0 || vehicleInfo?.Rider != null;

                        return (obj, icon, color, stack, null, null, weightText, equipeed);
                    })

                    .OnClick((obj, manager) =>
                    {
                        // 選択したアイテムの情報と選択肢を表示する
                        manager.PushScreen(commandMenuScreen, Arg.Self, null, targetObj: Arg.Arg.TargetObj, tool: obj);
                    })

                    .Build();
                };
            }

            protected virtual object GetViewStateHolder(MMgr manager)
                => Arg.Arg.TargetObj;

            protected abstract List<RogueObj> GetObjs(RogueObj self, RogueObj targetObj);

            protected virtual float GetDefaultViewPosition(MMgr manager)
            {
                if (!Skip0WeightObjs) return 0f;

                // 重さがゼロではないアイテムまで自動スクロール
                var objs = GetObjs(Arg.Self, Arg.Arg.TargetObj);
                for (int i = 0; i < objs.Count; i++)
                {
                    var weight = WeightCalculator.Get(objs[i]);
                    if (weight.TotalWeight > 0f) return i;
                }
                return 0f;
            }

            private void Sort(MMgr manager)
            {
                sortTable ??= new CategorizedSortTable(RoguegardSettings.ObjCommandTable.Categories);

                // ソートしたあと開きなおす
                sortTable.Sort(Arg.Arg.TargetObj);
                manager.Reopen();
            }
        }

        private class ItemsScreen : BaseScreen
        {
            protected override string Title => ":Inventory";
            protected override bool Skip0WeightObjs => true;
            protected override bool SortIsEnabled => true;

            private readonly List<RogueObj> objs = new();

            protected override List<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                // お金を取り除いたリストを生成
                objs.Clear();
                foreach (var spaceObj in targetObj.Space.Objs)
                {
                    if (spaceObj == null || spaceObj.Main.InfoSet.Equals(RoguegardSettings.MoneyInfoSet)) continue;

                    objs.Add(spaceObj);
                }
                return objs;
            }
        }

        private class GroundScreen : BaseScreen
        {
            protected override string Title => ":Ground";

            private readonly List<RogueObj> objs = new();

            protected override List<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                var targetPosition = targetObj.Position;
                objs.Clear();
                foreach (var obj in targetObj.Location.Space.Objs)
                {
                    if (obj == null || obj == targetObj || obj.Position != targetPosition) continue;

                    objs.Add(obj);
                }
                return objs;
            }
        }

        private class PutIntoContainerScreen : BaseScreen
        {
            protected override string Title => ":Put in what?";

            private readonly List<RogueObj> objs = new();

            protected override List<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                objs.Clear();
                foreach (var spaceObj in self.Space.Objs)
                {
                    objs.Add(spaceObj);
                }
                return objs;
            }
        }

        private class TakeOutOfContainerScreen : BaseScreen
        {
            protected override string Title => ":Take out what?";
            protected override bool SortIsEnabled => true;

            private readonly List<RogueObj> objs = new();

            protected override List<RogueObj> GetObjs(RogueObj self, RogueObj container)
            {
                objs.Clear();
                foreach (var obj in container.Space.Objs)
                {
                    objs.Add(obj);
                }
                return objs;
            }
        }
    }
}
