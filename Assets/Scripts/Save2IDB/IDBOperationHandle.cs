using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Save2IDB
{
    public class IDBOperationHandle : IEnumerator
    {
        public IDBOperationStatus Status { get; protected set; }
        internal protected string ErrorCode { get; protected set; }

        private readonly object _current;
        object IEnumerator.Current => _current;

        internal IDBOperationHandle()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            //_current = new WaitWhile(() => Status == IDBOperationStatus.InProgress);
            _current = null;
            Status = IDBOperationStatus.InProgress;
#else
            // WebGL でなければ最初から失敗扱い
            _current = null;
            Status = IDBOperationStatus.Failed;
#endif
        }

        internal void Done()
        {
            Status = IDBOperationStatus.Succeeded;
        }

        internal void Error(string errorCode)
        {
            ErrorCode = errorCode;
            Status = IDBOperationStatus.Failed;
        }

        bool IEnumerator.MoveNext()
        {
            return Status == IDBOperationStatus.InProgress;
        }

        void IEnumerator.Reset()
        {
            ErrorCode = default;
            Status = IDBOperationStatus.InProgress;
        }
    }

    public class IDBOperationHandle<T> : IDBOperationHandle, IEnumerator
    {
        public T Result { get; private set; }

        object IEnumerator.Current => null;

        internal IDBOperationHandle() { }

        internal void Done(T stream)
        {
            Result = stream;
            Status = IDBOperationStatus.Succeeded;
        }

        void IEnumerator.Reset()
        {
            Result = default;
            ErrorCode = default;
            Status = IDBOperationStatus.InProgress;
        }
    }
}
