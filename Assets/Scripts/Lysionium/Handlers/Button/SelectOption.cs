using System.Collections.Generic;

namespace Lysionium
{
    /// <inheritdoc/>
    public class SelectOption : SelectOption<IListuiManager>
    {
        public SelectOption()
        {
        }

        public SelectOption(string name, System.Action<IListuiManager> onClick, string style = null)
            : base(name, onClick, style)
        {
        }

        public static SelectOption<TMgr> Create<TMgr>(
            string name, System.Action<TMgr> onClick, string style = "")
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.EventGestureConfirmed = (m, _) => onClick(m);
            return instance;
        }

        public static SelectOption<TMgr> Create<TMgr>(
            System.Func<TMgr, string> getName, System.Action<TMgr> onClick, string style = "")
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.EventGestureConfirmed = (m, _) => onClick(m);
            return instance;
        }

        public static SelectOption<TMgr> Create<TMgr>(
            string name, System.Action<TMgr> onClick, System.Func<TMgr, string> style)
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.EventGestureConfirmed = (m, _) => onClick(m);
            return instance;
        }

        public static SelectOption<TMgr> Create<TMgr>(
            System.Func<TMgr, string> getName, System.Action<TMgr> onClick, System.Func<TMgr, string> style)
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.EventGestureConfirmed = (m, _) => onClick(m);
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, System.Action<TMgr, TArg> onClick, string style = "")
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.EventGestureConfirmed = (m, _, a) => onClick(m, a);
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName, System.Action<TMgr, TArg> onClick, string style = "")
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.EventGestureConfirmed = (m, _, a) => onClick(m, a);
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, System.Action<TMgr, TArg> onClick, System.Func<TMgr, TArg, string> style)
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.EventGestureConfirmed = (m, _, a) => onClick(m, a);
            return instance;
        }

        public static SelectOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName,
            System.Action<TMgr, TArg> onClick,
            System.Func<TMgr, TArg, string> style)
            where TMgr : IListuiManager
        {
            var instance = new SelectOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.EventGestureConfirmed = (m, _, a) => onClick(m, a);
            return instance;
        }
    }

    public class SelectOption<TMgr> : ISelectOption<TMgr>
    {
        private string name;
        private System.Func<TMgr, string> getName;

        private string style;
        private System.Func<TMgr, string> getStyle;

        private readonly IReadOnlyList<string> candidateEventGestureNames = clickOnlyEventGestureNames;

        public System.Action<TMgr, string> EventGestureConfirmed { get; set; }

        private static readonly IReadOnlyList<string> clickOnlyEventGestureNames
            = new List<string> { "Click" }.AsReadOnly();

        public SelectOption()
        {
        }

        public SelectOption(string name, System.Action<TMgr> onClick, string style = null)
        {
            this.name = name;
            EventGestureConfirmed = (m, _) => onClick(m);
            this.style = style;
        }

        public SelectOption(
            string name,
            IReadOnlyList<string> candidateEventGestureNames,
            System.Action<TMgr, string> onEventGestureConfirmed,
            string style = null)
        {
            this.name = name;
            this.candidateEventGestureNames = candidateEventGestureNames;
            EventGestureConfirmed = onEventGestureConfirmed;
            this.style = style;
        }

        public void SetName(string name)
        {
            this.name = name ?? throw new System.ArgumentNullException(nameof(name));
            getName = null;
        }

        public void SetName(System.Func<TMgr, string> selector)
        {
            getName = selector ?? throw new System.ArgumentNullException(nameof(selector));
            name = null;
        }

        public void SetStyle(string style)
        {
            this.style = style;
            getStyle = null;
        }

        public void SetStyle(System.Func<TMgr, string> selector)
        {
            getStyle = selector;
            style = null;
        }

        string ISelectOption<TMgr>.GetName(TMgr manager) => getName?.Invoke(manager) ?? name;
        string ISelectOption<TMgr>.GetStyle(TMgr manager) => getStyle?.Invoke(manager) ?? style;
        IReadOnlyList<string> ISelectOption<TMgr>.GetCandidateEventGestureNames(TMgr manager)
            => candidateEventGestureNames;
        void ISelectOption<TMgr>.EventGestureConfirmed(TMgr manager, string eventGestureName)
            => EventGestureConfirmed?.Invoke(manager, eventGestureName);
    }

    public class SelectOption<TMgr, TArg> : ISelectOption<TMgr, TArg>
    {
        private string name;
        private System.Func<TMgr, TArg, string> getName;

        private string style;
        private System.Func<TMgr, TArg, string> getStyle;

        private readonly IReadOnlyList<string> candidateEventGestureNames = clickOnlyEventGestureNames;

        public System.Action<TMgr, string, TArg> EventGestureConfirmed { get; set; }

        private static readonly IReadOnlyList<string> clickOnlyEventGestureNames
            = new List<string> { "Click" }.AsReadOnly();

        public SelectOption()
        {
        }

        public SelectOption(string name, System.Action<TMgr, TArg> onClick, string style = null)
        {
            this.name = name;
            EventGestureConfirmed = (m, _, a) => onClick(m, a);
            this.style = style;
        }

        public SelectOption(
            string name,
            IReadOnlyList<string> candidateEventGestureNames,
            System.Action<TMgr, string, TArg> onEventGestureConfirmed,
            string style = null)
        {
            this.name = name;
            this.candidateEventGestureNames = candidateEventGestureNames;
            EventGestureConfirmed = onEventGestureConfirmed;
            this.style = style;
        }

        public void SetName(string name)
        {
            this.name = name ?? throw new System.ArgumentNullException(nameof(name));
            getName = null;
        }

        public void SetName(System.Func<TMgr, TArg, string> selector)
        {
            getName = selector ?? throw new System.ArgumentNullException(nameof(selector));
            name = null;
        }

        public void SetStyle(string style)
        {
            this.style = style;
            getStyle = null;
        }

        public void SetStyle(System.Func<TMgr, TArg, string> selector)
        {
            getStyle = selector;
            style = null;
        }

        string ISelectOption<TMgr, TArg>.GetName(TMgr manager, TArg arg) => getName?.Invoke(manager, arg) ?? name;
        string ISelectOption<TMgr, TArg>.GetStyle(TMgr manager, TArg arg) => getStyle?.Invoke(manager, arg) ?? style;
        IReadOnlyList<string> ISelectOption<TMgr, TArg>.GetCandidateEventGestureNames(TMgr manager, TArg arg)
            => candidateEventGestureNames;
        void ISelectOption<TMgr, TArg>.EventGestureConfirmed(TMgr manager, string eventGestureName, TArg arg)
            => EventGestureConfirmed?.Invoke(manager, eventGestureName, arg);
    }
}
