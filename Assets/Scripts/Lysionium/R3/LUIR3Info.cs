using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.R3
{
    public abstract class LUIR3Info : System.IDisposable
    {
        private bool isOpening;
        
        /// <summary>
        /// いずれかのストリームで <see cref="ElementHandlerBuilderExtension.Handle"/> が実行されていれば true
        /// </summary>
        public bool IsHandled { get; private set; }

        public virtual LUIR3Info OpenSelf()
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

    public abstract class LUIR3Info<T> : LUIR3Info
    {
        public T ReturnValue { get; set; }

        public override LUIR3Info OpenSelf()
        {
            base.OpenSelf();
            ReturnValue = default;
            return this;
        }
    }
}
