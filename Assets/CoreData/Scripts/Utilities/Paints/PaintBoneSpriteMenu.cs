using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using ListingMF;
using SDSSprite;
using RuntimeDotter;
using Roguegard.Device;

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

        private readonly VariableWidgetsViewTemplate<MMgr, MArg> view = new()
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
                                return boneSprite.IsFirst ? "T" : "";
                            },
                            (manager, arg, value) =>
                            {
                                var table = (PaintBoneSpriteTable)arg.Arg.Other;
                                var itemIndex = arg.Arg.Count;
                                var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
                                boneSprite.IsFirst = !string.IsNullOrWhiteSpace(value);
                                return boneSprite.IsFirst ? "T" : "";
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
                        manager.Back();
                    });

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
            var table = (PaintBoneSpriteTable)arg.Arg.Other;
            var itemIndex = arg.Arg.Count;

            // 一部ボーンはミラーリング設定を表示する
            elms.RemoveRange(5, elms.Count - 5);
            if (table.Items[itemIndex] is PaintBoneSprite paintBoneSprite && System.Array.IndexOf(mirroringBones, paintBoneSprite.Bone) != -1)
            {
                elms.Add(mirroring);
            }
            elms.Add(remove);

            view.ShowTemplate(elms, manager, arg)
                ?
                .Build();
        }

        private class BoneMenu : RogueMenuScreen
        {
            private static string[] elms;

            private readonly ScrollViewTemplate<string, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
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

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .ElementNameFrom((boneName, manager, arg) => boneName)

                    .OnClickElement((boneName, manager, arg) =>
                    {
                        var table = (PaintBoneSpriteTable)arg.Arg.Other;
                        var itemIndex = arg.Arg.Count;
                        var boneSprite = (PaintBoneSprite)table.Items[itemIndex];

                        boneSprite.Bone = new BoneKeyword(boneName);

                        manager.Back();
                    })

                    .Build();
            }
        }

        private class PaintMenu : RogueMenuScreen
        {
            private readonly int directionIndex;
            private readonly object[] back;

            private static readonly DotterBoard[] elms = new DotterBoard[1];
            private static readonly Vector2[] pivots = new Vector2[2];

            public PaintMenu(int directionIndex)
            {
                if (directionIndex < 0 || 4 <= directionIndex) throw new System.ArgumentOutOfRangeException(nameof(directionIndex));

                this.directionIndex = directionIndex;
                //back = new[] { new ExitListMenuSelectOption(Back) };
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                throw new System.NotImplementedException();
            }

            //public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            //{
            //    var table = (PaintBoneSpriteTable)arg.Other;
            //    var itemIndex = arg.Count;
            //    var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
            //    switch (directionIndex)
            //    {
            //        case 0:
            //            elms[0] = boneSprite.NormalFront;
            //            break;
            //        case 1:
            //            elms[0] = boneSprite.NormalRear;
            //            break;
            //        case 2:
            //            elms[0] = boneSprite.BackFront;
            //            break;
            //        case 3:
            //            elms[0] = boneSprite.BackRear;
            //            break;
            //    }
            //    var showsSplitLine = boneSprite.ShowsSplitLine(elms[0], out pivots[0], out pivots[1]);
            //    var paint = (IPaintElementsView)manager.GetView(DeviceKw.MenuPaint);
            //    paint.OpenView(SelectOptionPresenter.Instance, elms, manager, self, null, new(other: table, count: itemIndex));
            //    paint.ShowSplitLine(showsSplitLine, pivots);
            //    var leftAnchor = manager.GetView(DeviceKw.MenuLeftAnchor);
            //    leftAnchor.OpenView(SelectOptionPresenter.Instance, back, manager, self, null, new(other: table, count: itemIndex));
            //}

            //private void Back(RogueMenuManager manager, ReadOnlyMenuArg arg)
            //{
            //    var table = (PaintBoneSpriteTable)arg.Arg.Other;
            //    var itemIndex = arg.Arg.Count;
            //    var boneSprite = (PaintBoneSprite)table.Items[itemIndex];
            //    var paint = (IPaintElementsView)manager.GetView(DeviceKw.MenuPaint);
            //    switch (directionIndex)
            //    {
            //        case 0:
            //        case 1:
            //            paint.Boards[0].CopyTo(boneSprite.NormalFront);
            //            boneSprite.BackRear = boneSprite.NormalFront;
            //            break;
            //        case 2:
            //        case 3:
            //            paint.Boards[0].CopyTo(boneSprite.NormalRear);
            //            boneSprite.BackFront = boneSprite.NormalRear;
            //            break;
            //    }
            //    table.MainColor = paint.MainColor;
            //    for (int i = 0; i < paint.Palette.Count; i++)
            //    {
            //        table.SetPalette(i, paint.Palette[i]);
            //    }
            //}
        }
    }
}
