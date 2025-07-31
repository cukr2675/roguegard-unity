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
        private static List<object> elms;
        private static object mirroring;
        private static object remove;
        private static BoneKeyword[] mirroringBones;

        private static readonly RogueMenuScreen[] paintMenus = new[]
        {
            new PaintMenu(0),
            new PaintMenu(1),
            new PaintMenu(2),
            new PaintMenu(3),
        };

        private readonly VariableWidgetsViewData<MMgr, MArg> view = new()
        {
        };

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            if (elms == null)
            {
                elms = new List<object>
                {
                    SelectOption.Create<MMgr, MArg>("部位を変更", new BoneMenu()),

                    new object[]
                    {
                        "中心点距離",
                        InputFieldViewWidget.CreateOption<MMgr, MArg>(
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
                            TMP_InputField.ContentType.IntegerNumber),
                    },

                    new object[]
                    {
                        "上書き",
                        InputFieldViewWidget.CreateOption<MMgr, MArg>(
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
                            }),
                    },

                    SelectOption.Create<MMgr, MArg>("正面を編集", paintMenus[0]),

                    SelectOption.Create<MMgr, MArg>("背面を編集", paintMenus[2]),
                };

                mirroring = new object[]
                {
                    "ミラーリング",
                    InputFieldViewWidget.CreateOption<MMgr, MArg>(
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
                        }),
                };

                remove = SelectOption.Create<MMgr, MArg>(
                    "<#f00>削除",
                    (manager, arg) =>
                    {
                        var table = (PaintBoneSpriteTable)arg.Arg.Other;
                        var itemIndex = arg.Arg.Count;
                        table.RemoveAt(itemIndex);
                        manager.PopMenuScreen();
                    });

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
            elms.RemoveRange(5, elms.Count - 5);
            if (table.Items[itemIndex] is PaintBoneSprite paintBoneSprite && System.Array.IndexOf(mirroringBones, paintBoneSprite.Bone) != -1)
            {
                elms.Add(mirroring);
            }
            elms.Add(remove);

            view.Show(elms, manager, arg)
                ?
                .Build();
        }

        private class BoneMenu : RogueMenuScreen
        {
            private static string[] elms;

            private readonly ScrollViewData<string, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                elms ??= new[]
                {
                    BoneKeyword.Body.Name,
                    BoneKeyword.LeftArm.Name,
                    BoneKeyword.RightArm.Name,
                    BoneKeyword.LeftLeg.Name,
                    BoneKeyword.RightLeg.Name,
                    BoneKeyword.Hair.Name,
                    BoneKeyword.Head.Name,
                };

                view.Show(elms, manager, arg)
                    ?
                    .NameFrom((boneName, manager, arg) => boneName)

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

            private static readonly DotterBoard[] elms = new DotterBoard[1];
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

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var table = (PaintBoneSpriteTable)arg.Arg.Other;
                var itemIndex = arg.Arg.Count;
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

                var paint = RoguegardSubviews.GetPaint(manager);
                paint.SetPaint(elms, table.Palette, table.MainColor, showsSplitLine, pivots);
                paint.Show();

                ISubviewStateProvider stateProvider = null;
                manager
                    .GetSubview(StandardSubviewTable.BackAnchorName)
                    .Show(back, SelectOptionViewItemHandler.Instance, manager, arg, ref stateProvider);
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
