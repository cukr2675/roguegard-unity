using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.CompilerServices;
using AOT;

namespace Save2IDB
{
    internal static class IDBCommon
    {
        public delegate void ThenCallback(System.IntPtr ohPtr);
        public delegate void CatchCallback(System.IntPtr ohPtr, string errorMsg);

        [MonoPInvokeCallback(typeof(ThenCallback))]
        public static void Then(System.IntPtr ohPtr)
        {
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle>(ref ohPtr);
            operationHandle.Done();
        }

        [MonoPInvokeCallback(typeof(CatchCallback))]
        public static void Catch(System.IntPtr ohPtr, string errorMsg)
        {
            Debug.LogError($"Save2IDB: {errorMsg}");
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle>(ref ohPtr);
            operationHandle.Error(errorMsg);
        }
    }
}
