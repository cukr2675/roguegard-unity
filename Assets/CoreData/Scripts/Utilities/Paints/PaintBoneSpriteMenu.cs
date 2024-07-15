using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using SDSSprite;
using RuntimeDotter;
using Roguegard.Device;

namespace Roguegard
{
    internal class PaintBoneSpriteMenu : IListMenu
    {
        private static List<object> elms;
        private static BoneKeyword[] mirroringBones;

        private static readonly IListMenu boneMenu = new BoneMenu();

        private static readonly IListMenu[] paintMenus = new[]
        {
            new PaintMenu(0),
            new PaintMenu(1),
            new PaintMenu(2),
            new PaintMenu(3),
        };

        public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (elms == null)
            {
                elms = new List<object>
                {
                    new ActionListMenuSelectOption("部位を変更", EditBone),
                    new PivotDistanceOption(),
                    new IsFirstOption(),
                    new ActionListMenuSelectOption("正面を編集", EditNormalSprite),
                    new ActionListMenuSelectOption("背面を編集", EditBackSprite)
                };
                mirroringBones = new[]
                {
                    BoneKeyword.LeftArm,
                    BoneKeyword.LeftLeg,
                    BoneKeyword.LeftEye,
                    BoneKeyword.LeftEar,
                    BoneKeyword.LeftWing,
                    BoneKeyword.RightArm,
                    BoneKeyword.RightLeg,
                    BoneKeyword.RightEye,
                    BoneKeyword.RightEar,
                    BoneKeyword.RightWing,
                };
            }
            var table = (PaintBoneSpriteTable)arg.Other;
            var itemIndex = arg.Count;

            // 一部ボーンはミラーリング設定を表示する
            elms.RemoveRange(5, elms.Count - 5);
            if (table.Items[itemIndex] is PaintBoneSprite paintBoneSprite && System.Array.IndexOf(mirroringBones, paintBoneSprite.Bone) != -1)
            {
                elms.Add(new MirroringOption());
            }
            elms.Add(new RemoveSelectOption());

            var scroll = manager.GetView(DeviceKw.MenuOptions);
            scroll.OpenView(SelectOptionPresenter.Instance, elms, manager, null, null, new(other: table, count: itemIndex));
            ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
        }

        private static void EditBone(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

            var table = (PaintBoneSpriteTable)arg.Other;
            var itemIndex = arg.Count;
            manager.OpenMenu(boneMenu, null, null, new(other: table, count: itemIndex));
        }

        private static void EditNormalSprite(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

            var table = (PaintBoneSpriteTable)arg.Other;
            var itemIndex = arg.Count;
            manager.OpenMenu(paintMenus[0], null, null, new(other: table, count: itemIndex));
        }

        private static void EditBackSprite(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

            var table = (PaintBoneSpriteTable)arg.Other;
            var itemIndex = arg.Count;
            manager.OpenMenu(paintMenus[2], null, null, new(other: table, count: itemIndex));
        }

        private class BoneMenu : IListMenu, IElementPresenter
        {
            private static object[] elms;

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (elms == null)
                {
                    elms = new[]
                    {
                        BoneKeyword.Body.Name,
                        BoneKeyword.LeftArm.Name,
                        BoneKeyword.RightArm.Name,
                        BoneKeyword.LeftLeg.Name,
                        BoneKeyword.RightLeg.Name,
                        BoneKeyword.Hair.Name,
                        BoneKeyword.Head.Name,
                    };
                }

                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = table.Items[itemIndex];
                var scroll = manager.GetView(DeviceKw.MenuScroll);
                scroll.OpenView(this, elms, manager, null, null, new(other: boneSprite));
                ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
            }

            public string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return (string)element;
            }

            public void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var boneSprite = (PaintBoneSprite)arg.Other;
                boneSprite.Bone = new BoneKeyword((string)element);

                manager.Back();
            }
        }

        private class PivotDistanceOption : IOptionsMenuText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.IntegerNumber;

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "中心点距離";
            }

            public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                return boneSprite.PivotDistance.ToString();
            }

            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                if (int.TryParse(value, out var pivotDistance))
                {
                    boneSprite.PivotDistance = pivotDistance;
                }
            }
        }

        private class IsFirstOption : IOptionsMenuCheckBox
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.IntegerNumber;

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "上書き";
            }

            public bool GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                return boneSprite.IsFirst;
            }

            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, bool value)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                boneSprite.IsFirst = value;
            }
        }

        private class MirroringOption : IOptionsMenuCheckBox
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.IntegerNumber;

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "ミラーリング";
            }

            public bool GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                return boneSprite.Mirroring;
            }

            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, bool value)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                boneSprite.Mirroring = value;
            }
        }

        private class RemoveSelectOption : BaseListMenuSelectOption
        {
            public override string Name => "<#f00>削除";

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                table.RemoveAt(itemIndex);
                manager.Back();
            }
        }

        private class PaintMenu : IListMenu
        {
            private readonly int directionIndex;
            private readonly object[] exit;

            private static readonly DotterBoard[] elms = new DotterBoard[1];
            private static readonly Vector2[] pivots = new Vector2[2];

            public PaintMenu(int directionIndex)
            {
                if (directionIndex < 0 || 4 <= directionIndex) throw new System.ArgumentOutOfRangeException(nameof(directionIndex));

                this.directionIndex = directionIndex;
                exit = new[] { new ExitListMenuSelectOption(Exit) };
            }

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                switch (directionIndex)
                {
                    case 0:
                        elms[0] = boneSprite.NormalFront;
                        break;
                    case 1:
                        elms[0] = boneSprite.NormalRear;
                        break;
                    case 2:
                        elms[0] = boneSprite.BackFront;
                        break;
                    case 3:
                        elms[0] = boneSprite.BackRear;
                        break;
                }
                var showsSplitLine = boneSprite.ShowsSplitLine(elms[0], out pivots[0], out pivots[1]);
                var paint = (IPaintElementsView)manager.GetView(DeviceKw.MenuPaint);
                paint.OpenView(SelectOptionPresenter.Instance, elms, manager, self, null, new(other: table, count: itemIndex));
                paint.ShowSplitLine(showsSplitLine, pivots);
                var leftAnchor = manager.GetView(DeviceKw.MenuLeftAnchor);
                leftAnchor.OpenView(SelectOptionPresenter.Instance, exit, manager, self, null, new(other: table, count: itemIndex));
            }

            private void Exit(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                var paint = (IPaintElementsView)manager.GetView(DeviceKw.MenuPaint);
                switch (directionIndex)
                {
                    case 0:
                    case 1:
                        paint.Boards[0].CopyTo(boneSprite.NormalFront);
                        boneSprite.BackRear = boneSprite.NormalFront;
                        break;
                    case 2:
                    case 3:
                        paint.Boards[0].CopyTo(boneSprite.NormalRear);
                        boneSprite.BackFront = boneSprite.NormalRear;
                        break;
                }
                table.MainColor = paint.MainColor;
                for (int i = 0; i < paint.Palette.Count; i++)
                {
                    table.SetPalette(i, paint.Palette[i]);
                }
            }
        }
    }
}
