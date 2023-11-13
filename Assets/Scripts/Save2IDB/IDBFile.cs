using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using AOT;

namespace Save2IDB
{
    public static class IDBFile
    {
        private delegate void DirectWriteAsyncThenCallback(System.IntPtr ohPtr);
        unsafe private delegate void OpenReadAsyncThenCallback(System.IntPtr ohPtr, byte* bytesPtr, long bytesLen);
        private delegate void DeleteAsyncThenCallback(System.IntPtr ohPtr);
        private delegate void GetFileInfosDescDateAsyncThenCallback(System.IntPtr ohPtr, string serial);
        private delegate void CommonCatchCallback(System.IntPtr ohPtr, string errorCode);

        [DllImport("__Internal")]
        unsafe private static extern void Save2IDB_DirectWriteAsync(
            System.IntPtr ohPtr, string path, byte* bytesPtr, int bytesLen, DirectWriteAsyncThenCallback thenCallback, CommonCatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_OpenReadAsync(
            System.IntPtr ohPtr, string path, OpenReadAsyncThenCallback thenCallback, CommonCatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_DeleteAsync(
            System.IntPtr ohPtr, string path, DeleteAsyncThenCallback thenCallback, CommonCatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_GetFileInfosDescDateAsync(
            System.IntPtr ohPtr, GetFileInfosDescDateAsyncThenCallback thenCallback, CommonCatchCallback catchCallback);

        private static readonly string invalidCharacterExceptionMessage = $"パスに無効な文字 [{string.Join(", ", invalidChars)}] が含まれています。";

        private static readonly string invalidChars = new string(Path.GetInvalidFileNameChars().Where(x => x != '/').ToArray());

        public static bool ContainsInvalidChar(string path)
        {
            return invalidChars.Contains(path);
        }

        [MonoPInvokeCallback(typeof(CommonCatchCallback))]
        private static void CommonCatch(System.IntPtr ohPtr, string errorCode)
        {
            Debug.LogError($"ErrorCode: {errorCode}");
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle>(ref ohPtr);
            operationHandle.Error(errorCode);
        }



        /// <summary>
        /// 指定のパスにファイルを書き込みます。
        /// </summary>
        unsafe private static IDBOperationHandle DirectWriteAsync(string path, byte[] buffer, long offset, long length)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new System.ArgumentException($"{path} は無効なパスです。", nameof(path));
            if (buffer == null) throw new System.ArgumentNullException(nameof(buffer));
            if (ContainsInvalidChar(path)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(path));
            if (length > int.MaxValue) throw new System.ArgumentOutOfRangeException(nameof(length));

            var operationHandle = new IDBOperationHandle();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle, System.IntPtr>(ref operationHandle);
            fixed (byte* ptr = &buffer[offset])
            {
                Save2IDB_DirectWriteAsync(ohPtr, path, ptr, (int)length, DirectWriteAsyncThen, CommonCatch);
            }
#endif
            return operationHandle;
        }

        /// <summary>
        /// 指定のパスにファイルを書き込みます。
        /// </summary>
        unsafe public static IDBOperationHandle DirectWriteAsync(string path, System.ReadOnlySpan<byte> source)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new System.ArgumentException($"{path} は無効なパスです。", nameof(path));
            if (source == null) throw new System.ArgumentNullException(nameof(source));
            if (ContainsInvalidChar(path)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(path));

            var operationHandle = new IDBOperationHandle();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle, System.IntPtr>(ref operationHandle);
            fixed (byte* ptr = source)
            {
                Save2IDB_DirectWriteAsync(ohPtr, path, ptr, source.Length, DirectWriteAsyncThen, CommonCatch);
            }
#endif
            return operationHandle;
        }

        [MonoPInvokeCallback(typeof(DirectWriteAsyncThenCallback))]
        unsafe private static void DirectWriteAsyncThen(System.IntPtr ohPtr)
        {
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle>(ref ohPtr);
            operationHandle.Done();
        }



        /// <summary>
        /// 指定のパスからファイルを読み込みます。
        /// </summary>
        unsafe public static Save2IDBOperationHandle<IDBStreamReader> OpenReadAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new System.ArgumentException($"{path} は無効なパスです。", nameof(path));
            if (ContainsInvalidChar(path)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(path));

            var operationHandle = new Save2IDBOperationHandle<IDBStreamReader>();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<Save2IDBOperationHandle<IDBStreamReader>, System.IntPtr>(ref operationHandle);
            Save2IDB_OpenReadAsync(ohPtr, path, OpenReadAsyncThen, CommonCatch);
#endif
            return operationHandle;
        }

        [MonoPInvokeCallback(typeof(OpenReadAsyncThenCallback))]
        unsafe private static void OpenReadAsyncThen(System.IntPtr ohPtr, byte* bytesPtr, long bytesLen)
        {
            var stream = new IDBStreamReader(bytesPtr, bytesLen);
            var operationHandle = Unsafe.As<System.IntPtr, Save2IDBOperationHandle<IDBStreamReader>>(ref ohPtr);
            operationHandle.Done(stream);
        }



        /// <summary>
        /// 指定のパスのファイルを削除します。
        /// </summary>
        public static IDBOperationHandle DeleteAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new System.ArgumentException($"{path} は無効なパスです。", nameof(path));
            if (ContainsInvalidChar(path)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(path));

            var operationHandle = new IDBOperationHandle();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle, System.IntPtr>(ref operationHandle);
            Save2IDB_DeleteAsync(ohPtr, path, DeleteAsyncThen, CommonCatch);
#endif
            return operationHandle;
        }

        [MonoPInvokeCallback(typeof(DeleteAsyncThenCallback))]
        private static void DeleteAsyncThen(System.IntPtr ohPtr)
        {
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle>(ref ohPtr);
            operationHandle.Done();
        }



        /// <summary>
        /// 保存されているファイルのパスの一覧を、更新日時の降順で取得します。
        /// </summary>
        public static Save2IDBOperationHandle<IDBFileInfo[]> GetFileInfosDescDateAsync()
        {
            var operationHandle = new Save2IDBOperationHandle<IDBFileInfo[]>();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<Save2IDBOperationHandle<IDBFileInfo[]>, System.IntPtr>(ref operationHandle);
            Save2IDB_GetFileInfosDescDateAsync(ohPtr, GetFileInfosDescDateAsyncThen, CommonCatch);
#endif
            return operationHandle;
        }

        [MonoPInvokeCallback(typeof(GetFileInfosDescDateAsyncThenCallback))]
        private static void GetFileInfosDescDateAsyncThen(System.IntPtr ohPtr, string serial)
        {
            var fileInfos = new List<IDBFileInfo>();
            string path = null;
            foreach (var item in serial.Split('|'))
            {
                if (path == null)
                {
                    path = item;
                }
                else
                {
                    var dateTime = System.DateTime.Parse(item);
                    var fileInfo = new IDBFileInfo(path, dateTime);
                    fileInfos.Add(fileInfo);
                    path = null;
                }
            }
            var operationHandle = Unsafe.As<System.IntPtr, Save2IDBOperationHandle<IDBFileInfo[]>>(ref ohPtr);
            operationHandle.Done(fileInfos.ToArray());
        }
    }
}
