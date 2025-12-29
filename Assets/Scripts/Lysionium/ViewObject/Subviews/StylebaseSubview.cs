using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/Subviews/LUI Stylebase Subview")]
    public class StylebaseSubview : Subview
    {
        [Tooltip("インジケータを構成する要素リスト")]
        [SerializeField] private List<StylebaseElement> _viewItems;

#if UNITY_EDITOR
        [Header("Editor Only")]

        [SerializeField] private ViewItem _generationViewItemPrefab;

        [SerializeField] private TextAsset _generationInputInfoPartialScriptFile;

        [SerializeField] private TextAsset _generationIndicatorMenuScreenScriptFile;
#endif

        private IViewItemHandler handler;
        private StateProvider currentStateProvider;

        protected override void CommonInitCore()
        {
            foreach (var item in _viewItems)
            {
                item.ViewItem.Initialize(this);
            }
        }

        public override void SetListHandler(
            IReadOnlyList<object> list, IViewItemHandler handler, IListMenuManager manager, IListMenuArg arg,
            ref ISubviewStateProvider stateProvider)
        {
            stateProvider ??= new StateProvider();
            if (stateProvider is not StateProvider local) throw new ArgumentException(
                $"{stateProvider} は {nameof(StateProvider)} ではありません。");

            // 現在の StateProvider を外す前に状態を保存する
            if (currentStateProvider != null)
            {
            }

            this.handler = handler;
            SetArg(manager, arg);
            UpdateViewItems(list);
            SetStatusCode(0);

            // 新しい StateProvider に切り替える
            currentStateProvider = local;
        }

        private void UpdateViewItems(IReadOnlyList<object> list)
        {
            // 初期化
            foreach (var item in _viewItems)
            {
                item.ViewItem.Unbind();
            }

            // バインド
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var style = handler.GetStyle(item, Manager, Arg);
                foreach (var viewItem in _viewItems)
                {
                    if (!viewItem.Match(style)) continue;

                    // スタイルがマッチする要素にバインド
                    viewItem.ViewItem.Bind(item, handler);
                }
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Generate View Items")]
        private void GenerateViewItems()
        {
            var keybindStyleSheet = GetComponentInParent<KeybindStyleSheet>();
            if (keybindStyleSheet == null)
            {
                Debug.LogError($"{nameof(KeybindStyleSheet)} が見つかりません。");
                return;
            }

            foreach (var binding in keybindStyleSheet.Bindings)
            {
                // キーバインドから ViewItem を生成する
                _viewItems.Add(new StylebaseElement
                {
                    Style = binding.Style,
                    ViewItem = (ViewItem)UnityEditor.PrefabUtility.InstantiatePrefab(_generationViewItemPrefab, transform)
                });
            }
        }

        //[ContextMenu("Generate InputInfo Script")]
        //private void GenerateInputInfoScript()
        //{
        //    var partialScriptFilePath = "Assets/InputInfo.Partial.cs";

        //    var className = "InputInfo";
        //    if (_generationInputInfoPartialScriptFile != null)
        //    {
        //        partialScriptFilePath = UnityEditor.AssetDatabase.GetAssetPath(_generationInputInfoPartialScriptFile);
        //        className = _generationInputInfoPartialScriptFile.name;
        //    }

        //    throw new NotImplementedException();
        //}

        //[ContextMenu("Generate IndicatorMenuScreen Script")]
        //private void GenerateIndicatorMenuScreenScript()
        //{
        //    var partialScriptFilePath = "Assets/InputInfo.Partial.cs";
        //    if (_generationInputInfoPartialScriptFile != null)
        //    {
        //        partialScriptFilePath = UnityEditor.AssetDatabase.GetAssetPath(_generationInputInfoPartialScriptFile);
        //    }

        //    throw new NotImplementedException();
        //}
#endif

        [Serializable]
        private class StylebaseElement
        {
            [SerializeField] private string _style;
            public string Style
            {
                get => _style;
                set => _style = value;
            }

            [SerializeField] private ViewItem _viewItem;
            public ViewItem ViewItem
            {
                get => _viewItem;
                set => _viewItem = value;
            }

            public bool Match(string style)
            {
                var startIndex = 0;
                for (int i = 0; i < style.Length; i++)
                {
                    if (style[i] == ' ')
                    {
                        var styleName = style.AsSpan(startIndex, i);
                        if (styleName.SequenceEqual(" ") && styleName.SequenceEqual(_style)) return true;

                        startIndex = i + 1;
                    }
                }
                {
                    var styleName = style.AsSpan(startIndex);
                    if (styleName.SequenceEqual(_style)) return true;
                }
                return false;
            }
        }

        private class StateProvider : ISubviewStateProvider
        {
            public void Reset()
            {
            }
        }
    }
}
