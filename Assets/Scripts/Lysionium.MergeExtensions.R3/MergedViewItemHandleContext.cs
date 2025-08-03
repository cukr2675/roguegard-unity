using UnityEngine;

namespace Lysionium.MergeExtensions.R3
{
    public abstract class MergedViewItemHandleContext : System.IDisposable
    {
        private bool isOpening;

        public virtual MergedViewItemHandleContext OpenSelf()
        {
            // 無限再帰対策（lock ステートメントは再入可能ロックのためチェックが必要）
            if (isOpening) throw new System.InvalidOperationException($"{GetType()} が再帰呼び出しされました。");

            return this;
        }

        public virtual void Dispose()
        {
            isOpening = false;
        }
    }

    /// <summary>
    /// 非同期の Result はサポートしない。非同期で取得したい場合は ViewItem のバインドを使用する
    /// </summary>
    public abstract class MergedViewItemHandleContext<T> : MergedViewItemHandleContext
    {
        public bool HasResult { get; private set; }

        private T _result;
        public T Result
        {
            get
            {
                if (!HasResult) throw new System.InvalidOperationException($"設定されていない {GetType()} の Result を取得しようとしました。");
                return _result;
            }
            set
            {
                if (HasResult) { Debug.LogWarning($"{GetType()} の Result が多重設定されました。"); }
                else { HasResult = true; }
                _result = value;
            }
        }

        public bool TryGetResult(out T result)
        {
            result = HasResult ? _result : default;
            return HasResult;
        }

        public override MergedViewItemHandleContext OpenSelf()
        {
            base.OpenSelf();
            HasResult = false;
            _result = default;
            return this;
        }

        public override void Dispose()
        {
            HasResult = false;
            _result = default;
            base.Dispose();
        }
    }
}
