using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    /// <summary>
    /// アイテム一覧を表示するメニュー。
    /// </summary>
    public class ObjsMenu
    {
        public IModelsMenuChoice Close { get; }

        /// <summary>
        /// <see cref="RogueMethodArgument.TargetObj"/> のインベントリを開く
        /// </summary>
        public IModelsMenu Items { get; }

        /// <summary>
        /// <see cref="RogueMethodArgument.TargetObj"/> の足元のアイテム一覧を開く
        /// </summary>
        public IModelsMenu Ground { get; }

        public IModelsMenu PutIntoChest { get; }

        public IModelsMenu TakeOutFromChest { get; }

        public ObjsMenu(
            CaptionWindow captionWindow, ObjCommandMenu commandMenu,
            PutIntoChestCommandMenu putInCommandMenu, TakeOutFromChestCommandMenu takeOutCommandMenu)
        {
            Close = new CloseChoice();
            Items = new ItemsMenu() { caption = captionWindow, commandMenu = commandMenu };
            Ground = new GroundMenu() { caption = captionWindow, commandMenu = commandMenu };
            PutIntoChest = new PutIntoChestMenu() { caption = captionWindow, commandMenu = putInCommandMenu };
            TakeOutFromChest = new TakeOutFromChestMenu() { caption = captionWindow, commandMenu = takeOutCommandMenu };
        }

        private class CloseChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => ":Close";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.Done();
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
            }
        }

        private abstract class ScrollMenu : IModelsMenu, IModelsMenuItemController
        {
            protected abstract string MenuName { get; }
            protected virtual bool Skip0WeightObjs => false;
            protected virtual bool SortIsEnabled => false;

            public CaptionWindow caption;
            public IModelsMenu commandMenu;

            private RogueObj prevTargetObj;
            private float prevPosition;
            private ActionModelsMenuChoice sortChoice;

            private static CategorizedSortTable sortTable;

            /// <summary>
            /// <paramref name="targetObj"/> のインベントリを開く
            /// </summary>
            protected abstract Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj);

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var models = GetObjs(self, arg.TargetObj);

                float position;
                if (arg.TargetObj == prevTargetObj)
                {
                    // 前回開いた対象と同じであれば同じ位置までスクロール
                    position = prevPosition;
                }
                else if (Skip0WeightObjs)
                {
                    // 重さがゼロではないアイテムまで自動スクロール
                    int index;
                    for (index = 0; index < models.Count; index++)
                    {
                        var weight = WeightCalculator.Get(models[index]);
                        if (weight.TotalWeight > 0f) break;
                    }
                    position = index;
                }
                else
                {
                    // 一番上までスクロール
                    position = 0f;
                }

                var scroll = (ScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, models, root, self, user, arg);
                scroll.SetPosition(position);
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);

                caption.ShowCaption(MenuName);

                // プレイヤーパーティのキャラのインベントリまたは倉庫であればソート可能
                if (SortIsEnabled)
                {
                    if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(arg.TargetObj) ||
                        ChestInfo.GetStorage(arg.TargetObj) != null)
                    {
                        sortChoice ??= new ActionModelsMenuChoice(":Sort", Sort);
                        scroll.ShowSortButton(sortChoice);
                    }
                }
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null) return null;

                var obj = (RogueObj)model;
                return obj.GetName();
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model == null)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                    return;
                }

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                // スクロール位置を記憶する
                prevPosition = root.Get(DeviceKw.MenuScroll).GetPosition();
                prevTargetObj = arg.TargetObj;

                // 選択したアイテムの情報と選択肢を表示する
                var obj = (RogueObj)model;
                caption.ShowCaption(obj.Main.InfoSet);
                root.OpenMenuAsDialog(commandMenu, self, null, new(targetObj: arg.TargetObj, tool: obj), arg);
            }

            public void Sort(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, StdKw.Sort);

                // スクロール位置を記憶する
                prevPosition = root.Get(DeviceKw.MenuScroll).GetPosition();
                prevTargetObj = arg.TargetObj;

                // ソートしたあと開きなおす
                if (sortTable == null)
                {
                    sortTable = new CategorizedSortTable(RoguegardSettings.ObjCommandTable.Categories);
                }
                sortTable.Sort(arg.TargetObj);
                root.Reopen(self, user, arg, arg);
                root.Back();
            }
        }

        private class ItemsMenu : ScrollMenu
        {
            protected override string MenuName => ":Inventory";
            protected override bool Skip0WeightObjs => true;
            protected override bool SortIsEnabled => true;

            private readonly List<RogueObj> models = new List<RogueObj>();

            protected override Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                // お金を取り除いたリストを生成
                models.Clear();
                var spaceObjs = targetObj.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    if (spaceObjs[i] == null || spaceObjs[i].Main.InfoSet == RoguegardSettings.MoneyInfoSet) continue;

                    models.Add(spaceObjs[i]);
                }
                return models;
            }
        }

        private class GroundMenu : ScrollMenu
        {
            protected override string MenuName => ":Ground";

            private readonly List<RogueObj> models = new List<RogueObj>();

            protected override Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                var locationObjs = targetObj.Location.Space.Objs;
                var targetPosition = targetObj.Position;
                models.Clear();
                for (int i = 0; i < locationObjs.Count; i++)
                {
                    var obj = locationObjs[i];
                    if (obj == null || obj == targetObj || obj.Position != targetPosition) continue;

                    models.Add(obj);
                }
                return models;
            }
        }

        private class PutIntoChestMenu : ScrollMenu
        {
            protected override string MenuName => ":Put in what?";

            protected override bool SortIsEnabled => true;

            protected override Spanning<RogueObj> GetObjs(RogueObj self, RogueObj targetObj)
            {
                return self.Space.Objs;
            }
        }

        private class TakeOutFromChestMenu : ScrollMenu
        {
            protected override string MenuName => ":Take out what?";

            protected override bool SortIsEnabled => true;

            protected override Spanning<RogueObj> GetObjs(RogueObj self, RogueObj chest)
            {
                var storageObjs = ChestInfo.GetStorage(chest);
                if (storageObjs != null) return storageObjs;
                else return chest.Space.Objs;
            }
        }
    }
}
