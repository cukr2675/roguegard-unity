using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lysionium
{
    /// <summary>
    /// 会話ボックスと選択肢を扱う ViewData
    /// </summary>
    public class SpeechBoxViewData<TMgr> : ListViewData<ISelectOption<TMgr>, TMgr>
        where TMgr : IListuiManager
    {
        public System.Func<TMgr, IMessageBoxSubview> SpeechBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.SpeechBox;
        public System.Func<TMgr, IListHandlerSubview> ChoicesSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.Choices;
        public System.Func<TMgr, IMessageBoxSubview> CaptionBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.CaptionBox;
        public List<StringReplacer> MessageReplacers { get; set; } = new List<StringReplacer>()
        {
            new("{v}[\r\n|\r|\n]?$", "<link=\"VerticalArrow\"></link>"),
            new("{v}[\r\n|\r|\n]?", "\n<link=\"VerticalArrow\"></link><link=\"PageBreak\"></link>"), // 中央揃え・右寄せに対応するため改行する
            new("{>}", "<link=\"HorizontalArrow\"></link>"),
        };

        private object prevViewStateHolder;
        private ISubviewStateProvider speechBoxSubviewStateProvider;
        private ISubviewStateProvider choicesSubviewStateProvider;
        private ISubviewStateProvider captionBoxSubviewStateProvider;
        private event System.Action<TMgr> OnCompleted;

        private string message;

        public Builder Show(string message, TMgr manager, object viewStateHolder = null)
        {
            if (message == null) throw new System.ArgumentNullException(nameof(message));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            // 必要に応じてスクロール位置をリセット
            if (viewStateHolder != prevViewStateHolder)
            {
                speechBoxSubviewStateProvider?.Reset();
                choicesSubviewStateProvider?.Reset();
                captionBoxSubviewStateProvider?.Reset();
            }
            prevViewStateHolder = viewStateHolder;

            // 文字送り矢印などを処理する
            foreach (var replacer in MessageReplacers)
            {
                message = Regex.Replace(message, replacer.From, replacer.GetTo(manager), RegexOptions.IgnoreCase);
            }

            // メッセージボックスのビューを表示
            this.message = message;

            if (TryShowSubviews(manager)) return null;
            else return new Builder(this, manager);
        }

        protected override void ShowSubviews(TMgr manager)
        {
            var speechBoxSubview = SpeechBoxSubviewSelector?.Invoke(manager);
            if (speechBoxSubview != null)
            {
                speechBoxSubview.Show(message, manager, ref speechBoxSubviewStateProvider, (manager) =>
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

                    OnCompleted?.Invoke(tMgr);
                }, OnHide);

                if (List.Count >= 1)
                {
                    ChoicesSubviewSelector?.Invoke(manager)?.SetListHandler(
                        List, SelectOptionViewItemHandler<TMgr>.Instance, manager, ref choicesSubviewStateProvider);
                    speechBoxSubview.DoScheduledAfterCompletion((manager) =>
                    {
                        if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return;

                        ChoicesSubviewSelector?.Invoke(tMgr)?.Show();
                    });
                }
            }

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    Title, manager, ref captionBoxSubviewStateProvider);
            }
        }

        public void Hide(TMgr manager, bool back)
        {
            SpeechBoxSubviewSelector?.Invoke(manager)?.Hide(back);
            ChoicesSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
        }

        public class Builder : BaseListBuilder<SpeechBoxViewData<TMgr>, Builder>, ISelectOptionsBuilder<TMgr, Builder>
        {
            public Builder(SpeechBoxViewData<TMgr> parent, TMgr manager)
                : base(parent, manager)
            {
            }

            public Builder OnCompleted(System.Action<TMgr> onCompleted)
            {
                AssertNotBuilt();

                Parent.OnCompleted += onCompleted;
                return this;
            }

            public Builder Option(ISelectOption<TMgr> option)
            {
                return Tail.Option(option);
            }

            protected override void Unload()
            {
                base.Unload();
                Parent.OnCompleted = null;
            }

            Builder ISelectOptionsBuilder<TMgr, Builder>.Option() => this;
        }

        public class StringReplacer
        {
            public string From { get; }
            private readonly System.Func<TMgr, string> to;

            public StringReplacer(string from, string to)
            {
                if (from == null) throw new System.ArgumentNullException(nameof(from));
                if (to == null) throw new System.ArgumentNullException(nameof(to));

                From = from;
                this.to = delegate { return to; };
            }

            public StringReplacer(string from, System.Func<TMgr, string> to)
            {
                if (from == null) throw new System.ArgumentNullException(nameof(from));
                if (to == null) throw new System.ArgumentNullException(nameof(to));

                From = from;
                this.to = to;
            }

            public string GetTo(TMgr manager)
            {
                return to(manager);
            }
        }
    }
}
