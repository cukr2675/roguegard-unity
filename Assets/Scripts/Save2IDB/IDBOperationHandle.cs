using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Save2IDB
{
    public class IDBOperationHandle : IEnumerator
    {
        public IDBOperationStatus Status { get; protected set; }
        public string ErrorMsg { get; protected set; }

        public virtual event System.Action<IDBOperationHandle> CompletedTypeless;

        private readonly object _current;
        object IEnumerator.Current => _current;

        internal IDBOperationHandle()
        {
            _current = new WaitWhile(() => Status == IDBOperationStatus.InProgress);
            Status = IDBOperationStatus.InProgress;
        }

        internal void Done()
        {
            Status = IDBOperationStatus.Succeeded;
            OnCompleted();
        }

        internal void Error(string errorMsg)
        {
            ErrorMsg = errorMsg;
            Status = IDBOperationStatus.Failed;
            OnCompleted();
        }

        protected virtual void OnCompleted()
        {
            CompletedTypeless?.Invoke(this);
        }

        bool IEnumerator.MoveNext()
        {
            return Status == IDBOperationStatus.InProgress;
        }

        void IEnumerator.Reset()
        {
            ErrorMsg = default;
            Status = IDBOperationStatus.InProgress;
        }
    }

    public class IDBOperationHandle<T> : IDBOperationHandle, IEnumerator
    {
        public virtual T Result { get; private set; }

        public event System.Action<IDBOperationHandle<T>> Completed;

        public override event System.Action<IDBOperationHandle> CompletedTypeless
        {
            add => Completed += value;
            remove => Completed -= value;
        }

        internal IDBOperationHandle() { }

        internal void Done(T result)
        {
            Result = result;
            Done();
        }

        protected override void OnCompleted()
        {
            base.OnCompleted();
            Completed?.Invoke(this);
        }

        void IEnumerator.Reset()
        {
            Result = default;
            ErrorMsg = default;
            Status = IDBOperationStatus.InProgress;
        }
    }
}
