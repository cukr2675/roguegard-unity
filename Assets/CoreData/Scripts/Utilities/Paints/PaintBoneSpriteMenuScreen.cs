using Lysionium;
using OchalikeSprites;
using Roguegard.Device;
using RuntimeDotter;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Roguegard
{
    internal class PaintBoneSpriteMenuScreen : RogueListuiScreen
    {
        private readonly List<object> list = new();
        private readonly VariableWidgetsMenuViewData<MMgr> view = new()
        {
        };

        private static object mirroring;
        private static BoneKeyword[] mirroringBones;

        public PaintBoneSpriteMenuScreen()
        {
            OnOpenScreen += (manager) =>
            {
                if (mirroring == null)
                {
                    mirroring = StackWidgetOption.Create(
                        ("1*", "ミラーリング"),
                        ("1*", InputFieldWidgetOption.Create<MMgr>(
                            _ =>
                            {
                                var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                                var itemIndex = Arg.Arg.Count;
                                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                                return boneSprite.Mirroring ? "T" : "";
                            },
                            value =>
                            {
                                var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                                var itemIndex = Arg.Arg.Count;
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
                var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                var itemIndex = Arg.Arg.Count;

                // 一部ボーンはミラーリング設定を表示する
                list.Clear();
                if (table.Items[itemIndex] is PaintBoneSprite paintBoneSprite && System.Array.IndexOf(mirroringBones, paintBoneSprite.Bone) != -1)
                {
                    list.Add(mirroring);
                }

                view.Show(list, manager)
                ?
                .Head.Option("部位を変更", new BoneSelectionScreen(), () => Arg)

                .HeadStack("中心点距離", InputFieldWidgetOption.Create<MMgr>(
                    _ =>
                    {
                        var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                        var itemIndex = Arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                        return boneSprite.PivotDistance.ToString();
                    },
                    value =>
                    {
                        var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                        var itemIndex = Arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                        if (!int.TryParse(value, out var pivotDistance)) { pivotDistance = 0; }

                        boneSprite.PivotDistance = pivotDistance;
                        return pivotDistance.ToString();
                    },
                    TMP_InputField.ContentType.IntegerNumber))

                .HeadStack("上書き", InputFieldWidgetOption.Create<MMgr>(
                    _ =>
                    {
                        var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                        var itemIndex = Arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                        return boneSprite.IsBare ? "T" : "";
                    },
                    value =>
                    {
                        var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                        var itemIndex = Arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                        boneSprite.IsBare = !string.IsNullOrWhiteSpace(value);
                        return boneSprite.IsBare ? "T" : "";
                    }))

                .Head.Option("正面を編集", new PaintScreen(0), () => Arg)

                .Head.Option("背面を編集", new PaintScreen(2), () => Arg)

                .Tail.Option("<#f00>削除", (manager) =>
                {
                    var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                    var itemIndex = Arg.Arg.Count;
                    table.RemoveAt(itemIndex);
                    manager.PopScreen();
                })

                .Build();
            };
        }

        private class BoneSelectionScreen : RogueListuiScreen
        {
            private readonly ScrollMenuViewData<string, MMgr> view = new()
            {
            };

            public BoneSelectionScreen()
            {
                string[] boneNames = null;

                OnOpenScreen += (manager) =>
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

                    view.Show(boneNames, manager)
                    ?
                    .NameFrom(boneName => boneName)

                    .OnClick((boneName, manager) =>
                    {
                        var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                        var itemIndex = Arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];

                        boneSprite.Bone = new BoneKeyword(boneName);

                        manager.PopScreen();
                    })

                    .Build();
                };
            }
        }

        private class PaintScreen : RogueListuiScreen
        {
            public PaintScreen(int directionIndex)
            {
                if (directionIndex < 0 || 4 <= directionIndex) throw new System.ArgumentOutOfRangeException(nameof(directionIndex));

                DotterBoard[] dotterBoards = new DotterBoard[1];
                Vector2[] pivots = new Vector2[2];
                var back = new SelectOptionList<MMgr>(_ => _.Option("<", Back));

                OnOpenScreen += (manager) =>
                {
                    var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                    var itemIndex = Arg.Arg.Count;
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

                    manager.Paint.SetPaint(dotterBoards, table.Palette, table.MainColor, showsSplitLine, pivots);
                    manager.Paint.Show();

                    ISubviewStateProvider stateProvider = null;
                    manager.BackAnchor.Show(back, manager, ref stateProvider);
                };

                void Back(MMgr manager)
                {
                    var table = (PaintBoneSpriteTable)Arg.Arg.Other;
                    var itemIndex = Arg.Arg.Count;
                    var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                    switch (directionIndex)
                    {
                        case 0:
                        case 1:
                            manager.Paint.Boards[0].CopyTo(boneSprite.NormalFront);
                            boneSprite.BackRear = boneSprite.NormalFront;
                            break;
                        case 2:
                        case 3:
                            manager.Paint.Boards[0].CopyTo(boneSprite.NormalRear);
                            boneSprite.BackFront = boneSprite.NormalRear;
                            break;
                    }
                    table.MainColor = manager.Paint.MainColor;
                    for (int i = 0; i < manager.Paint.Palette.Length; i++)
                    {
                        table.SetPalette(i, manager.Paint.Palette[i]);
                    }

                    manager.PopScreen();
                }
            }
        }
    }
}
