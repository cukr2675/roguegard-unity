using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Lysionium;
using OchalikeSprites;
using RuntimeDotter;
using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class DopesheetLane : ViewElement, IPointerClickHandler
    {
        [SerializeField] private Image _keyIconPrefab = null;
        private List<Image> keyIcons = new();

        [SerializeField] private float _resolution = 0.1f;

        [Header("Animation")]
        [SerializeField] private string _defaultStyle = "Submit";
        [Space, SerializeField] private Button.ButtonClickedEvent _onClick = null;
        private Animator animator;

        private readonly FloatDialog floatDialog = new();
        private readonly PaintDialog paintDialog = new();
        private MMgr MManager => (MMgr)Manager;

        private MotionGrapherInfo editInfo;
        private float timeScale;
        private object editList;

        private void Awake()
        {
            TryGetComponent(out animator);
        }

        public void SetParent(float timeScale, MotionGrapherInfo editInfo)
        {
            this.timeScale = timeScale;
            this.editInfo = editInfo;
        }

        protected override void SetElementCore(object element, IElementHandler handler)
        {
            editList = element;

            foreach (var keyIcon in keyIcons)
            {
                Destroy(keyIcon.gameObject);
            }
            keyIcons.Clear();

            if (editList is FloatKeyFrameList floatList)
            {
                for (int i = 0; i < floatList.Count; i++)
                {
                    var keyFrame = floatList[i];
                    var keyIcon = Instantiate(_keyIconPrefab, transform);
                    keyIcon.rectTransform.anchorMin = new Vector2(0f, .5f);
                    keyIcon.rectTransform.anchorMax = new Vector2(0f, .5f);
                    keyIcon.rectTransform.anchoredPosition = new Vector2(keyFrame.Time * timeScale, 0f);
                }
            }
            else if (editList is PaintKeyFrameList paintList)
            {
                for (int i = 0; i < paintList.Count; i++)
                {
                    var keyFrame = paintList[i];
                    var keyIcon = Instantiate(_keyIconPrefab, transform);
                    keyIcon.rectTransform.anchorMin = new Vector2(0f, .5f);
                    keyIcon.rectTransform.anchorMax = new Vector2(0f, .5f);
                    keyIcon.rectTransform.anchoredPosition = new Vector2(keyFrame.Time * timeScale, 0f);
                }
            }
            else
            {
                throw new System.InvalidOperationException();
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, null, out var localPoint);
            var clickTime = Mathf.Round((localPoint.x - RectTransform.rect.xMin) / timeScale / _resolution) * _resolution;
            if (editList is FloatKeyFrameList floatList)
            {
                floatDialog.SetTarget(floatList, clickTime);
                MManager.PushMenuScreen(floatDialog);
            }
            else if (editList is PaintKeyFrameList paintList)
            {
                paintDialog.SetTarget(editInfo, paintList, clickTime);
                MManager.PushMenuScreen(paintDialog);
            }
            else
            {
                throw new System.InvalidOperationException();
            }
        }

        private class FloatDialog : RogueMenuScreen
        {
            private FloatKeyFrameList editList;
            private float targetTime;
            private float? value;

            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                BackAnchorSubViewName = null,
            };

            public override bool IsIncremental => true;

            public void SetTarget(FloatKeyFrameList list, float time)
            {
                editList = list;
                targetTime = time;
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                if (editList.TryGetValue(targetTime, out var floatValue))
                {
                    value = floatValue;
                }
                else
                {
                    value = null;
                }

                view.ShowTemplate(string.Empty, manager, arg)
                    ?
                    .Tail(
                        new object[]
                        {
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => value.ToString(),
                                (manager, arg, value) => SetKeyFrame(value)),
                        })

                    .Tail(
                        new object[]
                        {
                            SelectOption.Create<MMgr, MArg>(":Submit", (manager, arg) =>
                            {
                                if (value != null) { editList.Set(targetTime, value.Value); }
                                else { editList.Remove(targetTime); }
                                manager.PopMenuScreen();
                            }),
                            BackSelectOption.Instance
                        })

                    .Build();
            }

            private string SetKeyFrame(string strValue)
            {
                if (float.TryParse(strValue, out var floatValue))
                {
                    value = floatValue;
                    return strValue;
                }
                else
                {
                    value = null;
                    return string.Empty;
                }
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.HideTemplate(manager, back);
            }
        }

        private class PaintDialog : RogueMenuScreen
        {
            private MotionGrapherInfo editInfo;
            private PaintKeyFrameList editList;
            private float targetTime;

            private readonly PaintMenu nextMenu = new(0);

            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                BackAnchorSubViewName = null,
            };

            public override bool IsIncremental => true;

            public void SetTarget(MotionGrapherInfo info, PaintKeyFrameList list, float time)
            {
                editInfo = info;
                editList = list;
                targetTime = time;
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                if (!editList.TryGetValue(targetTime, out var value))
                {
                    var boneSprite = new PaintBoneSprite();
                    boneSprite.NormalFront = boneSprite.BackRear = new DotterBoard(new Vector2Int(32, 32), 16);
                    boneSprite.NormalRear = boneSprite.BackFront = new DotterBoard(new Vector2Int(32, 32), 16);
                    boneSprite.Bone = BoneKeyword.Body;
                    boneSprite.Mirroring = true;
                    value = boneSprite;
                    editList.Set(targetTime, value);
                }
                nextMenu.SetTarget(editInfo, (PaintBoneSprite)value);

                view.ShowTemplate(string.Empty, manager, arg)
                    ?
                    .Tail(SelectOption.Create<MMgr, MArg>(":Edit", (manager, arg) =>
                    {
                        manager.PushMenuScreen(nextMenu, arg);
                    }))

                    //.Append(SelectOption.Create<MMgr, MArg>(":Delete", (manager, arg) =>
                    //{
                    //    manager.Back();
                    //}))

                    .Tail(BackSelectOption.Instance)

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.HideTemplate(manager, back);
            }
        }

        private class PaintMenu : RogueMenuScreen
        {
            private readonly int directionIndex;

            private static readonly DotterBoard[] elms = new DotterBoard[1];
            private static readonly Vector2[] pivots = new Vector2[2];
            private readonly object[] back;

            private MotionGrapherInfo editInfo;
            private PaintBoneSprite boneSprite;

            public PaintMenu(int directionIndex)
            {
                if (directionIndex < 0 || 4 <= directionIndex) throw new System.ArgumentOutOfRangeException(nameof(directionIndex));

                this.directionIndex = directionIndex;

                back = new object[]
                {
                    SelectOption.Create<MMgr, MArg>("<", Back),
                };
            }

            public void SetTarget(MotionGrapherInfo info, PaintBoneSprite boneSprite)
            {
                editInfo = info;
                this.boneSprite = boneSprite;
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
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

                var paint = RoguegardSubViews.GetPaint(manager);
                paint.SetPaint(elms, editInfo.Palette, editInfo.MainColor, showsSplitLine, pivots);
                paint.Show();

                IElementsSubViewStateProvider stateProvider = null;
                manager
                    .GetSubView(StandardSubViewTable.BackAnchorName)
                    .Show(back, SelectOptionHandler.Instance, manager, arg, ref stateProvider);
            }

            private void Back(MMgr manager, MArg arg)
            {
                var paint = RoguegardSubViews.GetPaint(manager);
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
                editInfo.MainColor = paint.MainColor;
                for (int i = 0; i < paint.Palette.Count; i++)
                {
                    editInfo.SetPalette(i, paint.Palette[i]);
                }

                manager.PopMenuScreen();
            }
        }
    }
}
