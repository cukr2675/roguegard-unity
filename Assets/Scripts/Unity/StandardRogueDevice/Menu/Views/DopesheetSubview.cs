using Lysionium;
using Lysionium.Views;
using OchalikeSprites;
using Roguegard;
using Roguegard.Device;
using Roguegard.Rgpacks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoguegardUnity
{
    public class DopesheetSubview : Subview, ICharacterCreationElementsSubview
    {
        [SerializeField] private ScrollRect _scrollRect = null;
        [SerializeField] private RectTransform _floatingContent = null;
        [SerializeField] private ButtonViewItem _menuButton = null;
        [SerializeField] private ButtonViewItem _playButton = null;
        [SerializeField] private Slider _seekBar = null;
        [SerializeField] private ButtonViewItem _cameraButton = null;

        [SerializeField] private ButtonViewItem _itemHeaderPrefab = null;
        [SerializeField] private DopesheetLane _itemLanePrefab = null;
        [SerializeField] private float _itemHeight = 100f;
        [SerializeField] private float _width = 10000f;
        [SerializeField] private float _timeScale = 1000f;

        private readonly NewBoneScreen newBoneScreen = new();
        private const int buttonsCount = 3;

        private readonly List<ViewItem> viewItems = new();
        private StateProvider currentStateProvider;
        private readonly MenuScreen menuScreen = new();

        private MArg arg;

        /// <summary>
        /// スクロールバーの遊び
        /// </summary>
        private Vector2 marginSize;

        private float VerticalAbsolutePosition
        {
            get
            {
                // 後から要素が増えたときのため、スクロール位置を変換したものを返す
                return (1f - _scrollRect.verticalNormalizedPosition) * marginSize.y;
            }
            set
            {
                const float epsilon = 1e-4f;
                if (marginSize.y < epsilon) { _scrollRect.verticalNormalizedPosition = 0f; } // ゼロ除算対策
                else { _scrollRect.verticalNormalizedPosition = 1f - (value / marginSize.y); }
            }
        }

        private float HorizontalAbsolutePosition
        {
            get
            {
                // 後から要素が増えたときのため、スクロール位置を変換したものを返す
                return _scrollRect.horizontalNormalizedPosition * marginSize.x;
            }
            set
            {
                const float epsilon = 1e-4f;
                if (marginSize.x < epsilon) { _scrollRect.horizontalNormalizedPosition = 0f; } // ゼロ除算対策
                else { _scrollRect.horizontalNormalizedPosition = value / marginSize.x; }
            }
        }

        ISelectOption<MMgr, MArg> ICharacterCreationElementsSubview.LoadPresetOption => throw new System.NotSupportedException();

        public void Initialize()
        {
            _menuButton.Initialize(this);
            _playButton.Initialize(this);
            _cameraButton.Initialize(this);
            viewItems.Add(_menuButton);
            viewItems.Add(_playButton);
            viewItems.Add(_cameraButton);
        }

        public void SetListHandler(
            IReadOnlyList<object> list, IViewItemHandler handler, MMgr manager, MArg arg, ref ISubviewStateProvider stateProvider)
        {
            stateProvider ??= new StateProvider();
            if (stateProvider is not StateProvider local) throw new System.ArgumentException(
                $"{stateProvider} は {nameof(StateProvider)} ではありません。");

            // 現在の StateProvider を外す前に状態を保存する
            if (currentStateProvider != null)
            {
                currentStateProvider.VerticalAbsolutePosition = VerticalAbsolutePosition;
                currentStateProvider.HorizontalAbsolutePosition = HorizontalAbsolutePosition;
                currentStateProvider.SelectedIndex = viewItems.IndexOf(LastSelectedItem);
            }

            var editInfo = (MotionGrapherInfo)arg.Arg.Other;
            if (editInfo.Tracks.Length == 0)
            {
                editInfo.AddTrack(new SpriteMotionGrapherTrack());
            }

            // 表示更新
            Manager = manager;
            this.arg = arg;
            UpdateElements(editInfo);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
            VerticalAbsolutePosition = local.VerticalAbsolutePosition;
            HorizontalAbsolutePosition = local.HorizontalAbsolutePosition;
            local.ApplySelectedIndex(this, viewItems);
        }

        private void UpdateElements(MotionGrapherInfo editInfo)
        {
            // 横スクロール幅変更
            _scrollRect.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, _width);

            for (int i = buttonsCount; i < viewItems.Count; i++)
            {
                Destroy(viewItems[i].gameObject);
            }
            viewItems.RemoveRange(buttonsCount, viewItems.Count - buttonsCount);

            var sumHeight = 0f;
            for (int i = 0; i < editInfo.Tracks.Length; i++)
            {
                var track = editInfo.Tracks[i];
                if (track is SpriteMotionGrapherTrack spriteMotionTrack)
                {
                    for (int j = 0; j < spriteMotionTrack.Bones.Length; j++)
                    {
                        var bone = spriteMotionTrack.Bones[j];
                        var removeIndex = j;
                        UpdateKeyFrameListElement(
                            $"{bone.BoneName}: Position.X", bone.Position.XKeys, editInfo, delegate { spriteMotionTrack.RemoveBoneAt(removeIndex); }, ref sumHeight);
                        UpdateKeyFrameListElement(
                            $"{bone.BoneName}: Position.Y", bone.Position.YKeys, editInfo, delegate { spriteMotionTrack.RemoveBoneAt(removeIndex); }, ref sumHeight);
                        UpdateKeyFrameListElement(
                            $"{bone.BoneName}: Rotation.Z", bone.Rotation.ZKeys, editInfo, delegate { spriteMotionTrack.RemoveBoneAt(removeIndex); }, ref sumHeight);
                        UpdateKeyFrameListElement(
                            $"{bone.BoneName}: Sprite", bone.Sprite, editInfo, delegate { spriteMotionTrack.RemoveBoneAt(removeIndex); }, ref sumHeight);
                        UpdateKeyFrameListElement(
                            $"{bone.BoneName}: Reorder", bone.Reorder, editInfo, delegate { spriteMotionTrack.RemoveBoneAt(removeIndex); }, ref sumHeight);
                    }
                }
                else if (track is SubTimelineMotionGrapherTrack subTimelineTrack &&
                    subTimelineTrack.TryGetValue(0f, out var subTimelineClip) &&
                    subTimelineClip is RgpackReferenceTimelineClip rgpackReferenceClip)
                {
                    var removeIndex = i;
                    UpdateKeyFrameListElement(
                        $"{rgpackReferenceClip.Id}: RefTime", new FloatKeyFrameList(), editInfo, delegate { editInfo.RemoveTrackAt(removeIndex); }, ref sumHeight);
                }
            }
            {
                // 制御対象ボーン追加ボタン
                var y = sumHeight;
                var headerWidth = _floatingContent.rect.width;
                var header = Instantiate(_itemHeaderPrefab, _scrollRect.content);
                header.Initialize(this);
                header.Bind(new SelectOption<MMgr>("+ ボーンを追加", (manager) =>
                {
                    manager.PushScreen(newBoneScreen, other: editInfo);
                }));
                header.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y, _itemHeight);
                header.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, headerWidth);
                viewItems.Add(header);

                sumHeight += _itemHeight;
            }
            {
                _menuButton.Bind(new SelectOption<MMgr>("...", (manager) => manager.PushScreen(menuScreen, arg)));
            }

            var scrollRect = _scrollRect.viewport.rect;
            marginSize.y = sumHeight - scrollRect.height;
            marginSize.x = _width - scrollRect.width;

            _scrollRect.content.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, sumHeight);
        }

        private void UpdateKeyFrameListElement(
            string name, object keyFrameList, MotionGrapherInfo editInfo, ClickOptionHandler<MMgr> handleRemove, ref float sumHeight)
        {
            var y = sumHeight;
            var headerWidth = _floatingContent.rect.width;

            var header = Instantiate(_itemHeaderPrefab, _scrollRect.content);
            header.Initialize(this);
            header.Bind(new SelectOption<MMgr>(name, (manager) =>
            {
                manager.PushScreen(
                    new ChoicesScreen($"{name} を削除しますか？")
                    .Option(":Yes", (manager) =>
                    {
                        handleRemove(manager);
                        manager.PopScreen();
                    })
                    .Back(), arg);
            }));
            header.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y, _itemHeight);
            header.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, headerWidth);
            viewItems.Add(header);

            var lane = Instantiate(_itemLanePrefab, _scrollRect.content);
            lane.Initialize(this);
            lane.SetParent(_timeScale, editInfo);
            lane.Bind(keyFrameList, SelectOptionViewItemHandler<IListuiManager>.Instance);
            lane.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, y, _itemHeight);
            lane.RectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, headerWidth, _width - headerWidth);
            viewItems.Add(lane);

            sumHeight += _itemHeight;
        }

        private class StateProvider : ISubviewStateProvider
        {
            public float VerticalAbsolutePosition { get; set; }
            public float HorizontalAbsolutePosition { get; set; }
            public int SelectedIndex { get; set; }

            public void Reset()
            {
                VerticalAbsolutePosition = 0f;
                HorizontalAbsolutePosition = 0f;
                SelectedIndex = -1;
            }

            public void ApplySelectedIndex(Subview subview, List<ViewItem> viewItems)
            {
                if (SelectedIndex <= 0 || viewItems.Count <= SelectedIndex || subview.EventSystem == null)
                {
                    // 選択オブジェクトが見つからなければ最初の項目を選択
                    if (viewItems.Count >= 2)
                    {
                        subview.EventSystem.SetSelectedGameObject(viewItems[1].gameObject);
                    }
                    return;
                }

                subview.EventSystem.SetSelectedGameObject(viewItems[SelectedIndex].gameObject);
            }
        }

        private class NewBoneScreen : RogueListuiScreen
        {
            private readonly string[] boneNames = new string[]
            {
                BoneKeyword.Body.Name,
                BoneKeyword.LeftArm.Name,
                BoneKeyword.RightArm.Name,
                BoneKeyword.LeftLeg.Name,
                BoneKeyword.RightLeg.Name,
                BoneKeyword.Head.Name,
                BoneKeyword.LeftEye.Name,
                BoneKeyword.RightEye.Name,
                BoneKeyword.Mouth.Name,
                BoneKeyword.HeadEffect.Name,
                "外部参照"
            };

            private readonly ScrollMenuViewData<string, MMgr> view = new()
            {
            };

            public NewBoneScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(boneNames, manager)
                    ?
                    .NameFrom(boneName => boneName)

                    .VarOnce(out var referenceScreen, new ReferenceNameScreen())
                    .OnClick((boneName, manager) =>
                    {
                        var editInfo = (MotionGrapherInfo)Arg.Arg.Other;
                        if (boneName == "外部参照")
                        {
                            manager.PushScreen(referenceScreen, other: editInfo);
                        }
                        else
                        {
                            ((SpriteMotionGrapherTrack)editInfo.Tracks[^1]).AddBone(boneName);
                            manager.PopScreen();
                        }
                    })

                    .Build();
                };
            }
        }

        private class ReferenceNameScreen : RogueListuiScreen
        {
            private readonly DialogViewData<MMgr> view = new()
            {
                BackAnchorSubviewSelector = null
            };

            public ReferenceNameScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(string.Empty, manager)
                    ?
                    .VarOnce(out string id)
                    .Tail.Append(InputFieldWidgetOption.Create<MMgr>(
                        _ => id,
                        value => id = value))

                    .Tail.Append(StackWidgetOption.Create(
                        ("1*", SelectOption.Create<MMgr>("追加", (manager) =>
                        {
                            var editInfo = (MotionGrapherInfo)Arg.Arg.Other;
                            var newTrack = new SubTimelineMotionGrapherTrack();
                            newTrack.AddClip(new RgpackReferenceTimelineClip() { Id = id });
                            editInfo.InsertTrack(0, newTrack);
                            manager.PopScreen(2);
                        })),
                        ("1*", BackSelectOption<MMgr>.Instance)))

                    .Build();
                };
            }
        }

        private class MenuScreen : RogueListuiScreen
        {
            private readonly VariableWidgetsMenuViewData<MMgr> view = new()
            {
            };

            public MenuScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(System.Array.Empty<object>(), manager)
                    ?
                    .TailStack("ループ回数", InputFieldWidgetOption.Create<MMgr>(
                        _ => ((MotionGrapherInfo)Arg.Arg.Other).LoopCount.ToString(),
                        (strValue) =>
                        {
                            if (!int.TryParse(strValue, out var value)) return strValue;

                            ((MotionGrapherInfo)Arg.Arg.Other).LoopCount = value;
                            return strValue;
                        }))

                    .TailStack("再生速度", InputFieldWidgetOption.Create<MMgr>(
                        _ => ((MotionGrapherInfo)Arg.Arg.Other).PlaybackSpeed.ToString(),
                        (strValue) =>
                        {
                            if (!float.TryParse(strValue, out var value))return strValue;

                            ((MotionGrapherInfo)Arg.Arg.Other).PlaybackSpeed = value;
                            return strValue;
                        }))

                    .Tail.Option("編集終了", (manager) =>
                    {
                        var editInfo = (MotionGrapherInfo)Arg.Arg.Other;
                        RogueDevice.AddWork(
                            DeviceKw.EnqueueWork,
                            RogueCharacterWork.CreateSpriteMotion(Arg.Self, new MotionGrapherSpriteMotion(editInfo), true));

                        manager.PopScreen(2);
                    })

                    .Build();
                };
            }
        }
    }
}
