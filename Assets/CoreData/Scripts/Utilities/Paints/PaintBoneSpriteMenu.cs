using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using SkeletalSprite;
using RuntimeDotter;
using Roguegard.Device;

namespace Roguegard
{
    internal class PaintBoneSpriteMenu : IModelsMenu
    {
        private static List<object> models;
        private static BoneKeyword[] mirroringBones;

        private static readonly IModelsMenu boneMenu = new BoneMenu();

        private static readonly IModelsMenu[] paintMenus = new[]
        {
            new PaintMenu(0),
            new PaintMenu(1),
            new PaintMenu(2),
            new PaintMenu(3),
        };

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (models == null)
            {
                models = new List<object>
                {
                    new ActionModelsMenuChoice("部位を変更", EditBone),
                    new PivotDistanceOption(),
                    new IsFirstOption(),
                    new ActionModelsMenuChoice("正面を編集", EditNormalSprite),
                    new ActionModelsMenuChoice("背面を編集", EditBackSprite)
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
            models.RemoveRange(5, models.Count - 5);
            if (table.Items[itemIndex] is PaintBoneSprite paintBoneSprite && System.Array.IndexOf(mirroringBones, paintBoneSprite.Bone) != -1)
            {
                models.Add(new MirroringOption());
            }
            models.Add(new RemoveChoice());

            var scroll = root.Get(DeviceKw.MenuOptions);
            scroll.OpenView(ChoicesModelsMenuItemController.Instance, models, root, null, null, new(other: table, count: itemIndex));
            ExitModelsMenuChoice.OpenLeftAnchorExit(root);
        }

        private static void EditBone(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

            var table = (PaintBoneSpriteTable)arg.Other;
            var itemIndex = arg.Count;
            root.OpenMenu(boneMenu, null, null, new(other: table, count: itemIndex), arg);
        }

        private static void EditNormalSprite(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

            var table = (PaintBoneSpriteTable)arg.Other;
            var itemIndex = arg.Count;
            root.OpenMenu(paintMenus[0], null, null, new(other: table, count: itemIndex), arg);
        }

        private static void EditBackSprite(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

            var table = (PaintBoneSpriteTable)arg.Other;
            var itemIndex = arg.Count;
            root.OpenMenu(paintMenus[2], null, null, new(other: table, count: itemIndex), arg);
        }

        private class BoneMenu : IModelsMenu, IModelsMenuItemController
        {
            private static object[] models;

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (models == null)
                {
                    models = new[]
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
                var scroll = root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, models, root, null, null, new(other: boneSprite));
                ExitModelsMenuChoice.OpenLeftAnchorExit(root);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return (string)model;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var boneSprite = (PaintBoneSprite)arg.Other;
                boneSprite.Bone = new BoneKeyword((string)model);

                root.Back();
            }
        }

        private class PivotDistanceOption : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.IntegerNumber;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "中心点距離";
            }

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                return boneSprite.PivotDistance.ToString();
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
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

        private class IsFirstOption : IModelsMenuOptionCheckBox
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.IntegerNumber;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "上書き";
            }

            public bool GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                return boneSprite.IsFirst;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, bool value)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                boneSprite.IsFirst = value;
            }
        }

        private class MirroringOption : IModelsMenuOptionCheckBox
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.IntegerNumber;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "ミラーリング";
            }

            public bool GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                return boneSprite.Mirroring;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, bool value)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                boneSprite.Mirroring = value;
            }
        }

        private class RemoveChoice : IModelsMenuChoice
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.IntegerNumber;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "<#f00>削除";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                table.RemoveAt(itemIndex);
                root.Back();
            }
        }

        private class PaintMenu : IModelsMenu
        {
            private readonly int directionIndex;
            private readonly object[] exit;

            private static readonly DotterBoard[] models = new DotterBoard[1];
            private static readonly Vector2[] pivots = new Vector2[2];

            public PaintMenu(int directionIndex)
            {
                if (directionIndex < 0 || 4 <= directionIndex) throw new System.ArgumentOutOfRangeException(nameof(directionIndex));

                this.directionIndex = directionIndex;
                exit = new[] { new ExitModelsMenuChoice(Exit) };
            }

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                switch (directionIndex)
                {
                    case 0:
                        models[0] = boneSprite.NormalFront;
                        break;
                    case 1:
                        models[0] = boneSprite.NormalRear;
                        break;
                    case 2:
                        models[0] = boneSprite.BackFront;
                        break;
                    case 3:
                        models[0] = boneSprite.BackRear;
                        break;
                }
                var showsSplitLine = boneSprite.ShowsSplitLine(models[0], out pivots[0], out pivots[1]);
                var paint = (IPaintModelsMenuView)root.Get(DeviceKw.MenuPaint);
                paint.OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, null, new(other: table, count: itemIndex));
                paint.ShowSplitLine(showsSplitLine, pivots);
                var leftAnchor = root.Get(DeviceKw.MenuLeftAnchor);
                leftAnchor.OpenView(ChoicesModelsMenuItemController.Instance, exit, root, self, null, new(other: table, count: itemIndex));
            }

            private void Exit(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var table = (PaintBoneSpriteTable)arg.Other;
                var itemIndex = arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                var paint = (IPaintModelsMenuView)root.Get(DeviceKw.MenuPaint);
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
