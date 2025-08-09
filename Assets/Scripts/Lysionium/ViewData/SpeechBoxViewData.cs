using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lysionium
{
    /// <summary>
    /// 会話ボックスと選択肢を扱う ViewData
    /// </summary>
    public class SpeechBoxViewData<TMgr, TArg> : ListViewData<ISelectOption, TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public string SpeechBoxSubviewName { get; set; } = StandardSubviewTable.SpeechBoxName;
        public string ChoicesSubviewName { get; set; } = StandardSubviewTable.ChoicesName;
        public string CaptionBoxSubviewName { get; set; } = StandardSubviewTable.CaptionBoxName;
        public List<StringReplacer> MessageReplacers { get; set; } = new List<StringReplacer>()
        {
            new("{v}[\r\n|\r|\n]?$", "<link=\"VerticalArrow\"></link>"),
            new("{v}[\r\n|\r|\n]?", "\n<link=\"VerticalArrow\"></link><link=\"PageBreak\"></link>"), // 中央揃え・右寄せに対応するため改行する
            new("{>}", "<link=\"HorizontalArrow\"></link>"),
        };

        private object prevViewStateHolder;
        private ISubviewStateProvider messageBoxSubviewStateProvider;
        private ISubviewStateProvider choicesSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private event ClickItemHandler<TMgr, TArg> OnCompleted;

        private readonly string[] message = new string[1];

        public Builder Show(string message, TMgr manager, TArg arg, object viewStateHolder = null)
        {
            if (message == null) throw new System.ArgumentNullException(nameof(message));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                messageBoxSubviewStateProvider?.Reset();
                choicesSubviewStateProvider?.Reset();
                captionBoxSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            // 文字送り矢印などを処理する
            foreach (var replacer in MessageReplacers)
            {
                message = Regex.Replace(message, replacer.From, replacer.GetTo(manager, arg), RegexOptions.IgnoreCase);
            }

            // メッセージボックスのビューを表示
            this.message[0] = message;

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            if (LuiAssert.Type<MessageBoxSubview>(manager.GetSubview(SpeechBoxSubviewName), out var speechBoxSubview)) return;

            speechBoxSubview.Show(message, ToStringViewItemHandler.Instance, manager, arg, ref messageBoxSubviewStateProvider);
            speechBoxSubview.DoScheduledAfterCompletion((manager, arg) =>
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                    LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

                OnCompleted?.Invoke(tMgr, tArg);
            });

            if (List.Count >= 1)
            {
                manager
                    .GetSubview(ChoicesSubviewName)
                    .SetParameters(List, SelectOptionViewItemHandler.Instance, manager, arg, ref choicesSubviewStateProvider);
                speechBoxSubview.DoScheduledAfterCompletion((manager, arg) =>
                {
                    manager.GetSubview(ChoicesSubviewName).Show();
                });
            }

            if (Title != null)
            {
                manager
                    .GetSubview(CaptionBoxSubviewName)
                    .Show(TitleSingle, ToStringViewItemHandler.Instance, manager, arg, ref captionBoxSubviewStateProvider);
            }
        }

        public void Hide(TMgr manager, bool back)
        {
            manager.GetSubview(SpeechBoxSubviewName).Hide(back);
            manager.GetSubview(ChoicesSubviewName).Hide(back);
            if (Title != null) { manager.GetSubview(CaptionBoxSubviewName).Hide(back); }
        }

        public class Builder : BaseListBuilder<SpeechBoxViewData<TMgr, TArg>, Builder>
        {
            public Builder(SpeechBoxViewData<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder OnCompleted(ClickItemHandler<TMgr, TArg> onCompleted)
            {
                AssertNotBuilt();

                Parent.OnCompleted += onCompleted;
                return this;
            }

            public Builder Option(string name, ClickItemHandler<TMgr, TArg> onClick)
            {
                AssertNotBuilt();

                Tail(SelectOption.Create(name, onClick));
                return this;
            }

            public Builder Back()
            {
                AssertNotBuilt();

                Tail(BackSelectOption.Instance);
                return this;
            }
        }

        public class StringReplacer
        {
            public string From { get; }
            private readonly System.Func<TMgr, TArg, string> to;

            public StringReplacer(string from, string to)
            {
                if (from == null) throw new System.ArgumentNullException(nameof(from));
                if (to == null) throw new System.ArgumentNullException(nameof(to));

                From = from;
                this.to = delegate { return to; };
            }

            public StringReplacer(string from, System.Func<TMgr, TArg, string> to)
            {
                if (from == null) throw new System.ArgumentNullException(nameof(from));
                if (to == null) throw new System.ArgumentNullException(nameof(to));

                From = from;
                this.to = to;
            }

            public string GetTo(TMgr manager, TArg arg)
            {
                return to(manager, arg);
            }
        }
    }
}
