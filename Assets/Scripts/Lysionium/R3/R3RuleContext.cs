using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.R3
{
    public abstract class R3RuleContext : System.IDisposable
    {
        private bool isOpening;
        
        /// <summary>
        /// いずれかのストリームで <see cref="ElementHandlerBuilderExtension.Handle"/> が実行されていれば true
        /// </summary>
        public bool IsHandled { get; private set; }

        public virtual R3RuleContext OpenSelf()
        {
            // 無限再帰対策
            if (isOpening) throw new System.InvalidOperationException($"{GetType()} はすでに開かれています。");

            IsHandled = false;
            return this;
        }

        public virtual void Dispose()
        {
            isOpening = false;
        }

        internal void Handle()
        {
            IsHandled = true;
        }
    }

    public abstract class R3RuleContext<T> : R3RuleContext
    {
        public T ReturnValue { get; set; }

        public override R3RuleContext OpenSelf()
        {
            base.OpenSelf();
            ReturnValue = default;
            return this;
        }
    }
}
