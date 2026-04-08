using Lysionium;
using Lysionium.Views;
using OchalikeSprites;
using Roguegard;
using Roguegard.Device;
using RuntimeDotter;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoguegardUnity
{
    public class DopesheetLane : ViewItem, IPointerClickHandler
    {
        [SerializeField] private Image _keyIconPrefab = null;
        private readonly List<Image> keyIcons = new();

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

        protected virtual void Awake()
        {
            TryGetComponent(out animator);
        }

        public void SetParent(float timeScale, MotionGrapherInfo editInfo)
        {
            this.timeScale = timeScale;
            this.editInfo = editInfo;
        }

        protected override void BindCore(object item, IViewItemHandler handler)
        {
            editList = item;

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

        protected override void UnbindCore(object item, IViewItemHandler handler)
        {
            editList = null;

            foreach (var keyIcon in keyIcons)
            {
                Destroy(keyIcon.gameObject);
            }
            keyIcons.Clear();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, null, out var localPoint);
            var clickTime = Mathf.Round((localPoint.x - RectTransform.rect.xMin) / timeScale / _resolution) * _resolution;
            if (editList is FloatKeyFrameList floatList)
            {
                floatDialog.SetTarget(floatList, clickTime);
                MManager.PushScreen(floatDialog);
            }
            else if (editList is PaintKeyFrameList paintList)
            {
                paintDialog.SetTarget(editInfo, paintList, clickTime);
                MManager.PushScreen(paintDialog);
            }
            else
            {
                throw new System.InvalidOperationException();
            }
        }

        private class FloatDialog : RogueListuiScreen
        {
            private FloatKeyFrameList editList;
            private float targetTime;
            private float? value;

            private readonly DialogViewData<MMgr> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public FloatDialog()
            {
                OnOpenScreen += (manager) =>
                {
                    if (editList.TryGetValue(targetTime, out var floatValue))
                    {
                        value = floatValue;
                    }
                    else
                    {
                        value = null;
                    }

                    view.Show(string.Empty, manager)
                    ?
                    .Tail.Append(InputFieldWidgetOption.Create<MMgr>(
                        _ => value.ToString(),
                        value => SetKeyFrame(value)))

                    .Tail.Append(StackWidgetOption.Create(
                        ("1*", SelectOption.Create<MMgr>(":Submit", (manager) =>
                        {
                            if (value != null) { editList.Set(targetTime, value.Value); }
                            else { editList.Remove(targetTime); }
                            manager.PopScreen();
                        })),
                        ("1*", BackSelectOption<MMgr>.Instance)))

                    .Build();
                };

                OnCloseScreenView += (manager, back) =>
                {
                    view.Hide(manager, back);
                };
            }

            public void SetTarget(FloatKeyFrameList list, float time)
            {
                editList = list;
                targetTime = time;
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
        }

        private class PaintDialog : RogueListuiScreen
        {
            private MotionGrapherInfo editInfo;
            private PaintKeyFrameList editList;
            private float targetTime;

            private readonly PaintScreen nextScreen = new(0);

            private readonly DialogViewData<MMgr> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public void SetTarget(MotionGrapherInfo info, PaintKeyFrameList list, float time)
            {
                editInfo = info;
                editList = list;
                targetTime = time;
            }

            public PaintDialog()
            {
                OnOpenScreen += (manager) =>
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
                    nextScreen.SetTarget(editInfo, (PaintBoneSprite)value);

                    view.Show(string.Empty, manager)
                    ?
                    .Tail.Option(":Edit", (manager) =>
                    {
                        manager.PushScreen(nextScreen);
                    })

                    //.Tail.Option(":Delete", (manager) =>
                    //{
                    //    manager.Back();
                    //})

                    .Tail.Back()

                    .Build();
                };

                OnCloseScreenView += (manager, back) =>
                {
                    view.Hide(manager, back);
                };
            }
        }

        private class PaintScreen : RogueListuiScreen
        {
            private readonly int directionIndex;

            private static readonly DotterBoard[] dotterBoards = new DotterBoard[1];
            private static readonly Vector2[] pivots = new Vector2[2];
            private readonly SelectOptionList<MMgr> back;

            private MotionGrapherInfo editInfo;
            private PaintBoneSprite boneSprite;

            public PaintScreen(int directionIndex)
            {
                if (directionIndex < 0 || 4 <= directionIndex) throw new System.ArgumentOutOfRangeException(nameof(directionIndex));

                this.directionIndex = directionIndex;

                back = new(_ => _.Option("<", Back));

                OnOpenScreen += (manager) =>
                {
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

                    manager.Paint.SetPaint(dotterBoards, editInfo.Palette, editInfo.MainColor, showsSplitLine, pivots);
                    manager.Paint.Show();

                    ISubviewStateProvider stateProvider = null;
                    manager.BackAnchor.Show(back, manager, ref stateProvider);
                };
            }

            public void SetTarget(MotionGrapherInfo info, PaintBoneSprite boneSprite)
            {
                editInfo = info;
                this.boneSprite = boneSprite;
            }

            private void Back(MMgr manager)
            {
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
                editInfo.MainColor = manager.Paint.MainColor;
                for (int i = 0; i < manager.Paint.Palette.Length; i++)
                {
                    editInfo.SetPalette(i, manager.Paint.Palette[i]);
                }

                manager.PopScreen();
            }
        }
    }
}
