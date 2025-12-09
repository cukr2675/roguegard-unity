using Lysionium;
using OchalikeSprites;
using Roguegard.Device;
using RuntimeDotter;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Roguegard
{
    internal class PaintBoneSpriteMenu : RogueMenuScreen
    {
        private readonly List<object> list = new();
        private readonly VariableWidgetsMenuViewData<MMgr, MArg> view = new()
        {
        };

        private static object mirroring;
        private static BoneKeyword[] mirroringBones;

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            if (mirroring == null)
            {
                mirroring = StackWidgetOption.Create(
                    ("1*", "ミラーリング"),
                    ("1*", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            var table = (PaintBoneSpriteTable)arg.Arg.Other;
                            var itemIndex = arg.Arg.Count;
                            var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                            return boneSprite.Mirroring ? "T" : "";
                        },
                        (manager, arg, value) =>
                        {
                            var table = (PaintBoneSpriteTable)arg.Arg.Other;
                            var itemIndex = arg.Arg.Count;
                            var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                            boneSprite.Mirroring = !string.IsNullOrWhiteSpace(value);
                            return boneSprite.Mirroring ? "T" : "";
                        })));

                mirroringBones = new[]
                {
                    BoneKeyword.LeftArm,
                    BoneKeyword.LeftLeg,
                    BoneKeyword.LeftEye,
                    BoneKeyword.LeftEar,
                    BoneKeyword.RightArm,
                    BoneKeyword.RightLeg,
                    BoneKeyword.RightEye,
                    BoneKeyword.RightEar,
                };
            }
            var table = (PaintBoneSpriteTable)arg.Arg.Other;
            var itemIndex = arg.Arg.Count;

            // 一部ボーンはミラーリング設定を表示する
            list.Clear();
            if (table.Items[itemIndex] is PaintBoneSprite paintBoneSprite && System.Array.IndexOf(mirroringBones, paintBoneSprite.Bone) != -1)
            {
                list.Add(mirroring);
            }

            view.Show(list, manager, arg)
                ?
                .Head.Option("部位を変更", new BoneMenu())

                .HeadStack("中心点距離", InputFieldWidgetOption.Create<MMgr, MArg>(
                    (manager, arg) =>
                    {
                        var table = (PaintBoneSpriteTable)arg.Arg.Other;
                        var itemIndex = arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                        return boneSprite.PivotDistance.ToString();
                    },
                    (manager, arg, value) =>
                    {
                        var table = (PaintBoneSpriteTable)arg.Arg.Other;
                        var itemIndex = arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                        if (!int.TryParse(value, out var pivotDistance)) { pivotDistance = 0; }

                        boneSprite.PivotDistance = pivotDistance;
                        return pivotDistance.ToString();
                    },
                    TMP_InputField.ContentType.IntegerNumber))

                .HeadStack("上書き", InputFieldWidgetOption.Create<MMgr, MArg>(
                    (manager, arg) =>
                    {
                        var table = (PaintBoneSpriteTable)arg.Arg.Other;
                        var itemIndex = arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                        return boneSprite.IsBare ? "T" : "";
                    },
                    (manager, arg, value) =>
                    {
                        var table = (PaintBoneSpriteTable)arg.Arg.Other;
                        var itemIndex = arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                        boneSprite.IsBare = !string.IsNullOrWhiteSpace(value);
                        return boneSprite.IsBare ? "T" : "";
                    }))

                .Head.Option("正面を編集", new PaintMenu(0))

                .Head.Option("背面を編集", new PaintMenu(2))

                .Tail.Option("<#f00>削除", (manager, arg) =>
                {
                    var table = (PaintBoneSpriteTable)arg.Arg.Other;
                    var itemIndex = arg.Arg.Count;
                    table.RemoveAt(itemIndex);
                    manager.PopMenuScreen();
                })

                .Build();
        }

        private class BoneMenu : RogueMenuScreen
        {
            private readonly ScrollMenuViewData<string, MMgr, MArg> view = new()
            {
            };

            private static string[] boneNames;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                boneNames ??= new[]
                {
                    BoneKeyword.Body.Name,
                    BoneKeyword.LeftArm.Name,
                    BoneKeyword.RightArm.Name,
                    BoneKeyword.LeftLeg.Name,
                    BoneKeyword.RightLeg.Name,
                    BoneKeyword.Hair.Name,
                    BoneKeyword.Head.Name,
                };

                view.Show(boneNames, manager, arg)
                    ?
                    .NameFrom(boneName => boneName)

                    .OnClick((boneName, manager, arg) =>
                    {
                        var table = (PaintBoneSpriteTable)arg.Arg.Other;
                        var itemIndex = arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];

                        boneSprite.Bone = new BoneKeyword(boneName);

                        manager.PopMenuScreen();
                    })

                    .Build();
            }
        }

        private class PaintMenu : RogueMenuScreen
        {
            private readonly int directionIndex;

            private static readonly DotterBoard[] dotterBoards = new DotterBoard[1];
            private static readonly Vector2[] pivots = new Vector2[2];
            private readonly object[] back;

            public PaintMenu(int directionIndex)
            {
                if (directionIndex < 0 || 4 <= directionIndex) throw new System.ArgumentOutOfRangeException(nameof(directionIndex));

                this.directionIndex = directionIndex;

                back = new object[]
                {
                    SelectOption.Create<MMgr, MArg>("<", Back),
                };
            }

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                var table = (PaintBoneSpriteTable)arg.Arg.Other;
                var itemIndex = arg.Arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                switch (directionIndex)
                {
                    case 0:
                        dotterBoards[0] = boneSprite.NormalFront;
                        break;
                    case 1:
                        dotterBoards[0] = boneSprite.NormalRear;
                        break;
                    case 2:
                        dotterBoards[0] = boneSprite.BackFront;
                        break;
                    case 3:
                        dotterBoards[0] = boneSprite.BackRear;
                        break;
                }
                var showsSplitLine = boneSprite.ShowsSplitLine(dotterBoards[0], out pivots[0], out pivots[1]);

                var paint = RoguegardSubviews.GetPaint(manager);
                paint.SetPaint(dotterBoards, table.Palette, table.MainColor, showsSplitLine, pivots);
                paint.Show();

                ISubviewStateProvider stateProvider = null;
                manager.BackAnchor.Show(back, SelectOptionViewItemHandler<MMgr, MArg>.Instance, manager, arg, ref stateProvider);
            }

            private void Back(MMgr manager, MArg arg)
            {
                var table = (PaintBoneSpriteTable)arg.Arg.Other;
                var itemIndex = arg.Arg.Count;
                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                var paint = RoguegardSubviews.GetPaint(manager);
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
                for (int i = 0; i < paint.Palette.Length; i++)
                {
                    table.SetPalette(i, paint.Palette[i]);
                }

                manager.PopMenuScreen();
            }
        }
    }
}
