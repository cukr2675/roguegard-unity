using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lysionium
{
    /// <inheritdoc/>
    public class SpeechBoxViewData<TMgr> : SpeechBoxViewData<TMgr, IListuiArg>
        where TMgr : IListuiManager
    { }

    /// <summary>
    /// 会話ボックスと選択肢を扱う ViewData
    /// </summary>
    public class SpeechBoxViewData<TMgr, TArg> : ListViewData<ISelectOption<TMgr, TArg>, TMgr, TArg>
        where TMgr : IListuiManager
        where TArg : IListuiArg
    {
        public System.Func<TMgr, IMessageBoxSubview> SpeechBoxSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.SpeechBox;
        public System.Func<TMgr, IListHandlerSubview> ChoicesSubviewSelector { get; set; }
            = manager => (manager as IDefaultSubviewTable)?.Choices;
        public System.Func<TMgr, IListHandlerSubview> CaptionBoxSubviewSelector { get; set; }
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
        private event ListuiEventHandler<TMgr, TArg> OnCompleted;

        private readonly string[] message = new string[1];

        public Builder Show(string message, TMgr manager, TArg arg, object viewStateHolder = null)
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
                message = Regex.Replace(message, replacer.From, replacer.GetTo(manager, arg), RegexOptions.IgnoreCase);
            }

            // メッセージボックスのビューを表示
            this.message[0] = message;

            if (TryShowSubviews(manager, arg)) return null;
            else return new Builder(this, manager, arg);
        }

        protected override void ShowSubviews(TMgr manager, TArg arg)
        {
            var speechBoxSubview = SpeechBoxSubviewSelector?.Invoke(manager);
            if (speechBoxSubview != null)
            {
                speechBoxSubview.Show(message, ToStringViewItemHandler.Instance, manager, arg, ref speechBoxSubviewStateProvider);
                speechBoxSubview.DoScheduledAfterCompletion((manager, arg) =>
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                        LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

                    OnCompleted?.Invoke(tMgr, tArg);
                });

                if (List.Count >= 1)
                {
                    ChoicesSubviewSelector?.Invoke(manager)?.SetListHandler(
                        List, SelectOptionViewItemHandler<TMgr, TArg>.Instance, manager, arg, ref choicesSubviewStateProvider);
                    speechBoxSubview.DoScheduledAfterCompletion((manager, arg) =>
                    {
                        if (LuiAssert.Type<TMgr>(manager, out var tMgr)) return;

                        ChoicesSubviewSelector?.Invoke(tMgr)?.Show();
                    });
                }
            }

            if (Title != null)
            {
                CaptionBoxSubviewSelector?.Invoke(manager)?.Show(
                    TitleSingle, ToStringViewItemHandler.Instance, manager, arg, ref captionBoxSubviewStateProvider);
            }
        }

        public void Hide(TMgr manager, bool back)
        {
            SpeechBoxSubviewSelector?.Invoke(manager)?.Hide(back);
            ChoicesSubviewSelector?.Invoke(manager)?.Hide(back);
            if (Title != null) { CaptionBoxSubviewSelector?.Invoke(manager)?.Hide(back); }
        }

        public class Builder : BaseListBuilder<SpeechBoxViewData<TMgr, TArg>, Builder>, ISelectOptionListBuilder<TMgr, TArg, Builder>
        {
            public Builder(SpeechBoxViewData<TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public Builder OnCompleted(ListuiEventHandler<TMgr, TArg> onCompleted)
            {
                AssertNotBuilt();

                Parent.OnCompleted += onCompleted;
                return this;
            }

            public Builder Option(ISelectOption<TMgr, TArg> option)
            {
                return Tail.Option(option);
            }

            protected override void Unload()
            {
                base.Unload();
                Parent.OnCompleted = null;
            }

            Builder ISelectOptionListBuilder<TMgr, TArg, Builder>.Option() => this;
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
