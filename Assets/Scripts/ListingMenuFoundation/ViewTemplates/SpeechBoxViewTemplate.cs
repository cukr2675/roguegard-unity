using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;

namespace ListingMF
{
    /// <summary>
    /// 会話ボックスと選択肢を扱う ViewTemplate
    /// </summary>
    public class SpeechBoxViewTemplate<TMgr, TArg> : ListViewTemplate<ISelectOption, TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string SpeechBoxSubViewName { get; set; } = StandardSubViewTable.SpeechBoxName;
        public string ChoicesSubViewName { get; set; } = StandardSubViewTable.ChoicesName;
        public string CaptionBoxSubViewName { get; set; } = StandardSubViewTable.CaptionBoxName;
        public List<StringReplacer> MessageReplacers { get; set; } = new List<StringReplacer>()
        {
            new("{v}$", "<link=\"VerticalArrow\"></link>"),
            new("{v}", "<link=\"VerticalArrow\"></link><link=\"PageBreak\"></link>"),
            new("{>}", "<link=\"HorizontalArrow\"></link>"),
        };

        private object prevViewStateHolder;
        private IElementsSubViewStateProvider messageBoxSubViewStateProvider;
        private IElementsSubViewStateProvider choicesSubViewStateProvider;
        private IElementsSubViewStateProvider captionBoxSubViewStateProvider;
        private event HandleClickElement<TMgr, TArg> OnCompleted;

        private readonly string[] message = new string[1];

        public Builder ShowTemplate(string message, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (message == null) throw new System.ArgumentNullException(nameof(message));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                messageBoxSubViewStateProvider?.Reset();
                choicesSubViewStateProvider?.Reset();
                captionBoxSubViewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            // 文字送り矢印などを処理する
            foreach (var replacer in MessageReplacers)
            {
                message = Regex.Replace(message, replacer.From, replacer.To, RegexOptions.IgnoreCase);
            }

            // メッセージボックスのビューを表示
            this.message[0] = message;

            if (TryShowSubViews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubViews(TMgr manager, TArg arg)
        {
            if (LMFAssert.Type<MessageBoxSubView>(manager.GetSubView(SpeechBoxSubViewName), out var speechBoxSubView)) return;

            speechBoxSubView.Show(message, ElementToStringHandler.Instance, manager, arg, ref messageBoxSubViewStateProvider);
            speechBoxSubView.DoScheduledAfterCompletion((manager, arg) =>
            {
                if (LMFAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                    LMFAssert.Type<TArg>(arg, out var tArg, manager)) return;

                OnCompleted?.Invoke(tMgr, tArg);
            });

            if (List.Count >= 1)
            {
                manager
                    .GetSubView(ChoicesSubViewName)
                    .SetParameters(List, SelectOptionHandler.Instance, manager, arg, ref choicesSubViewStateProvider);
                speechBoxSubView.DoScheduledAfterCompletion((manager, arg) =>
                {
                    manager.GetSubView(ChoicesSubViewName).Show();
                });
            }

            if (Title != null)
            {
                manager
                    .GetSubView(CaptionBoxSubViewName)
                    .Show(TitleSingle, ElementToStringHandler.Instance, manager, arg, ref captionBoxSubViewStateProvider);
            }
        }

        public void HideTemplate(TMgr manager, bool back)
        {
            manager.GetSubView(SpeechBoxSubViewName).Hide(back);
            manager.GetSubView(ChoicesSubViewName).Hide(back);
            if (Title != null) { manager.GetSubView(CaptionBoxSubViewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<Builder>
        {
            private readonly SpeechBoxViewTemplate<TMgr, TArg> parent;

            public Builder(SpeechBoxViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public Builder OnCompleted(HandleClickElement<TMgr, TArg> onCompleted)
            {
                AssertNotBuilded();

                parent.OnCompleted += onCompleted;
                return this;
            }

            public Builder Option(string name, HandleClickElement<TMgr, TArg> onClick)
            {
                AssertNotBuilded();

                Append(SelectOption.Create(name, onClick));
                return this;
            }

            public Builder Back()
            {
                AssertNotBuilded();

                Append(BackSelectOption.Instance);
                return this;
            }
        }

        public class StringReplacer
        {
            public string From { get; }
            public string To { get; }

            public StringReplacer(string from, string to)
            {
                From = from;
                To = to;
            }

            public string Replace(string value) => value.Replace(From, To, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
