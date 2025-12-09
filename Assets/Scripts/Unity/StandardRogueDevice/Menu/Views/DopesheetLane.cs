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

            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public override bool IsIncremental => true;

            public void SetTarget(FloatKeyFrameList list, float time)
            {
                editList = list;
                targetTime = time;
            }

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                if (editList.TryGetValue(targetTime, out var floatValue))
                {
                    value = floatValue;
                }
                else
                {
                    value = null;
                }

                view.Show(string.Empty, manager, arg)
                    ?
                    .Tail.Append(InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) => value.ToString(),
                        (manager, arg, value) => SetKeyFrame(value)))

                    .Tail.Append(StackWidgetOption.Create(
                        ("1*", SelectOption.Create<MMgr, MArg>(":Submit", (manager, arg) =>
                        {
                            if (value != null) { editList.Set(targetTime, value.Value); }
                            else { editList.Remove(targetTime); }
                            manager.PopMenuScreen();
                        })),
                        ("1*", BackSelectOption<MMgr, MArg>.Instance)))

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
                view.Hide(manager, back);
            }
        }

        private class PaintDialog : RogueMenuScreen
        {
            private MotionGrapherInfo editInfo;
            private PaintKeyFrameList editList;
            private float targetTime;

            private readonly PaintMenu nextMenu = new(0);

            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                BackAnchorSubviewSelector = null,
            };

            public override bool IsIncremental => true;

            public void SetTarget(MotionGrapherInfo info, PaintKeyFrameList list, float time)
            {
                editInfo = info;
                editList = list;
                targetTime = time;
            }

            public override void OpenScreen(MMgr manager, MArg arg)
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

                view.Show(string.Empty, manager, arg)
                    ?
                    .Tail.Option(":Edit", (manager, arg) =>
                    {
                        manager.PushMenuScreen(nextMenu, arg);
                    })

                    //.Tail.Option(":Delete", (manager, arg) =>
                    //{
                    //    manager.Back();
                    //})

                    .Tail.Back()

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.Hide(manager, back);
            }
        }

        private class PaintMenu : RogueMenuScreen
        {
            private readonly int directionIndex;

            private static readonly DotterBoard[] dotterBoards = new DotterBoard[1];
            private static readonly Vector2[] pivots = new Vector2[2];
            private readonly SelectOptionList<MMgr, MArg> back;

            private MotionGrapherInfo editInfo;
            private PaintBoneSprite boneSprite;

            public PaintMenu(int directionIndex)
            {
                if (directionIndex < 0 || 4 <= directionIndex) throw new System.ArgumentOutOfRangeException(nameof(directionIndex));

                this.directionIndex = directionIndex;

                back = new(_ => _.Option("<", Back));
            }

            public void SetTarget(MotionGrapherInfo info, PaintBoneSprite boneSprite)
            {
                editInfo = info;
                this.boneSprite = boneSprite;
            }

            public override void OpenScreen(MMgr manager, MArg arg)
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

                var paint = RoguegardSubviews.GetPaint(manager);
                paint.SetPaint(dotterBoards, editInfo.Palette, editInfo.MainColor, showsSplitLine, pivots);
                paint.Show();

                ISubviewStateProvider stateProvider = null;
                manager.BackAnchor.Show(back, manager, arg, ref stateProvider);
            }

            private void Back(MMgr manager, MArg arg)
            {
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
                editInfo.MainColor = paint.MainColor;
                for (int i = 0; i < paint.Palette.Length; i++)
                {
                    editInfo.SetPalette(i, paint.Palette[i]);
                }

                manager.PopMenuScreen();
            }
        }
    }
}
