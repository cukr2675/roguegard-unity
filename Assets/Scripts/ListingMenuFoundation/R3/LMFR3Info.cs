using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF.R3
{
    public abstract class LMFR3Info : System.IDisposable
    {
        private bool isOpening;
        
        /// <summary>
        /// いずれかのストリームで <see cref="ElementHandlerBuilderExtension.Handle"/> が実行されていれば true
        /// </summary>
        public bool IsHandled { get; private set; }

        public virtual LMFR3Info OpenSelf()
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

    public abstract class LMFR3Info<T> : LMFR3Info
    {
        public T ReturnValue { get; set; }

        public override LMFR3Info OpenSelf()
        {
            base.OpenSelf();
            ReturnValue = default;
            return this;
        }
    }
}
