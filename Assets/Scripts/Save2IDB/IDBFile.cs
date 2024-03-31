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
        unsafe private delegate void OpenReadAllBytesAsyncThenCallback(System.IntPtr ohPtr, byte* bytesPtr, long bytesLen);
        private delegate void ExistsAsyncThenCallback(System.IntPtr ohPtr, bool exists);
        private delegate void GetFileInfosDescDateAsyncThenCallback(System.IntPtr ohPtr, string serial);
        private delegate void ImportAsyncThenCallback(System.IntPtr ohPtr, string path);
        private delegate void CommonThenCallback(System.IntPtr ohPtr);
        private delegate void CommonCatchCallback(System.IntPtr ohPtr, string errorCode);

        [DllImport("__Internal")]
        unsafe private static extern void Save2IDB_WriteAllBytesAsync(
            System.IntPtr ohPtr, string path, byte* bytesPtr, int bytesLen, CommonThenCallback thenCallback, CommonCatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_OpenReadAllBytesAsync(
            System.IntPtr ohPtr, string path, OpenReadAllBytesAsyncThenCallback thenCallback, CommonCatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_DeleteAsync(
            System.IntPtr ohPtr, string path, CommonThenCallback thenCallback, CommonCatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_CopyMoveAsync(
            System.IntPtr ohPtr, string sourcePath, string destPath, bool overwrite, bool moveMode, CommonThenCallback thenCallback, CommonCatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_ExistsAsync(
            System.IntPtr ohPtr, string path, ExistsAsyncThenCallback thenCallback, CommonCatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_GetFileInfosDescDateAsync(
            System.IntPtr ohPtr, GetFileInfosDescDateAsyncThenCallback thenCallback, CommonCatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_ExportAsync(
            System.IntPtr ohPtr, string path, CommonThenCallback thenCallback, CommonCatchCallback catchCallback);

        [DllImport("__Internal")]
        private static extern void Save2IDB_ImportAsync(
            System.IntPtr ohPtr, string prefix, bool overwrite, ImportAsyncThenCallback thenCallback, CommonCatchCallback catchCallback);

        private static readonly string invalidCharacterExceptionMessage = $"パスに無効な文字 [{string.Join(", ", invalidChars)}] が含まれています。";

        private static readonly string invalidChars = new string(Path.GetInvalidFileNameChars().Where(x => x != '/').ToArray());

        public static bool ContainsInvalidChar(string path)
        {
            return invalidChars.Contains(path);
        }

        [MonoPInvokeCallback(typeof(CommonThenCallback))]
        private static void CommonThen(System.IntPtr ohPtr)
        {
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle>(ref ohPtr);
            operationHandle.Done();
        }

        [MonoPInvokeCallback(typeof(CommonCatchCallback))]
        private static void CommonCatch(System.IntPtr ohPtr, string errorMsg)
        {
            Debug.LogError($"ErrorMsg: {errorMsg}");
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle>(ref ohPtr);
            operationHandle.Error(errorMsg);
        }



        /// <summary>
        /// 指定のパスにファイルを書き込みます。
        /// </summary>
        unsafe private static IDBOperationHandle WriteAllBytesAsync(string path, byte[] buffer, long offset, long length)
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
                Save2IDB_WriteAllBytesAsync(ohPtr, path, ptr, (int)length, CommonThen, CommonCatch);
            }
#endif
            return operationHandle;
        }

        /// <summary>
        /// 指定のパスにファイルを書き込みます。
        /// </summary>
        unsafe public static IDBOperationHandle WriteAllBytesAsync(string path, System.ReadOnlySpan<byte> bytes)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new System.ArgumentException($"{path} は無効なパスです。", nameof(path));
            if (bytes == null) throw new System.ArgumentNullException(nameof(bytes));
            if (ContainsInvalidChar(path)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(path));

            var operationHandle = new IDBOperationHandle();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle, System.IntPtr>(ref operationHandle);
            fixed (byte* ptr = bytes)
            {
                Save2IDB_WriteAllBytesAsync(ohPtr, path, ptr, bytes.Length, CommonThen, CommonCatch);
            }
#endif
            return operationHandle;
        }



        /// <summary>
        /// 指定のパスからファイルを読み込みます。
        /// </summary>
        unsafe public static IDBOperationHandle<IDBStreamReader> OpenReadAllBytesAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new System.ArgumentException($"{path} は無効なパスです。", nameof(path));
            if (ContainsInvalidChar(path)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(path));

            var operationHandle = new IDBOperationHandle<IDBStreamReader>();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle<IDBStreamReader>, System.IntPtr>(ref operationHandle);
            Save2IDB_OpenReadAllBytesAsync(ohPtr, path, OpenReadAllBytesAsyncThen, CommonCatch);
#endif
            return operationHandle;
        }

        [MonoPInvokeCallback(typeof(OpenReadAllBytesAsyncThenCallback))]
        unsafe private static void OpenReadAllBytesAsyncThen(System.IntPtr ohPtr, byte* bytesPtr, long bytesLen)
        {
            var stream = new IDBStreamReader(bytesPtr, bytesLen);
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle<IDBStreamReader>>(ref ohPtr);
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
            Save2IDB_DeleteAsync(ohPtr, path, CommonThen, CommonCatch);
#endif
            return operationHandle;
        }



        /// <summary>
        /// <paramref name="sourcePath"/> のファイルを <paramref name="destPath"/> へ移動します。
        /// </summary>
        public static IDBOperationHandle CopyAsync(string sourcePath, string destPath, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(sourcePath)) throw new System.ArgumentException($"{sourcePath} は無効なパスです。", nameof(sourcePath));
            if (ContainsInvalidChar(sourcePath)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(sourcePath));
            if (string.IsNullOrWhiteSpace(destPath)) throw new System.ArgumentException($"{destPath} は無効なパスです。", nameof(destPath));
            if (ContainsInvalidChar(destPath)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(destPath));

            var operationHandle = new IDBOperationHandle();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle, System.IntPtr>(ref operationHandle);
            Save2IDB_CopyMoveAsync(ohPtr, sourcePath, destPath, overwrite, false, CommonThen, CommonCatch);
#endif
            return operationHandle;
        }



        /// <summary>
        /// <paramref name="sourcePath"/> のファイルを <paramref name="destPath"/> へ移動します。
        /// </summary>
        public static IDBOperationHandle MoveAsync(string sourcePath, string destPath, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(sourcePath)) throw new System.ArgumentException($"{sourcePath} は無効なパスです。", nameof(sourcePath));
            if (ContainsInvalidChar(sourcePath)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(sourcePath));
            if (string.IsNullOrWhiteSpace(destPath)) throw new System.ArgumentException($"{destPath} は無効なパスです。", nameof(destPath));
            if (ContainsInvalidChar(destPath)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(destPath));

            var operationHandle = new IDBOperationHandle();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle, System.IntPtr>(ref operationHandle);
            Save2IDB_CopyMoveAsync(ohPtr, sourcePath, destPath, overwrite, true, CommonThen, CommonCatch);
#endif
            return operationHandle;
        }



        /// <summary>
        /// <paramref name="path"/> にファイルが存在するかを取得します。
        /// </summary>
        public static IDBOperationHandle<bool> ExistsAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new System.ArgumentException($"{path} は無効なパスです。", nameof(path));
            if (ContainsInvalidChar(path)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(path));

            var operationHandle = new IDBOperationHandle<bool>();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle<bool>, System.IntPtr>(ref operationHandle);
            Save2IDB_ExistsAsync(ohPtr, path, ExistsAsyncThen, CommonCatch);
#endif
            return operationHandle;
        }

        [MonoPInvokeCallback(typeof(ExistsAsyncThenCallback))]
        private static void ExistsAsyncThen(System.IntPtr ohPtr, bool exists)
        {
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle<bool>>(ref ohPtr);
            operationHandle.Done(exists);
        }



        /// <summary>
        /// 保存されているファイルのパスの一覧を、更新日時の降順で取得します。
        /// </summary>
        public static IDBOperationHandle<IDBFileInfo[]> GetFileInfosDescDateAsync()
        {
            var operationHandle = new IDBOperationHandle<IDBFileInfo[]>();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle<IDBFileInfo[]>, System.IntPtr>(ref operationHandle);
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
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle<IDBFileInfo[]>>(ref ohPtr);
            operationHandle.Done(fileInfos.ToArray());
        }



        public static IDBOperationHandle ExportAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new System.ArgumentException($"{path} は無効なパスです。", nameof(path));
            if (ContainsInvalidChar(path)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(path));

            var operationHandle = new IDBOperationHandle();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle, System.IntPtr>(ref operationHandle);
            Save2IDB_ExportAsync(ohPtr, path, CommonThen, CommonCatch);
#endif
            return operationHandle;
        }



        public static IDBOperationHandle<string> ImportAsync(string prefix, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(prefix)) throw new System.ArgumentException($"{prefix} は無効なパスです。", nameof(prefix));
            if (ContainsInvalidChar(prefix)) throw new System.ArgumentException(invalidCharacterExceptionMessage, nameof(prefix));

            var operationHandle = new IDBOperationHandle<string>();
#if UNITY_WEBGL && !UNITY_EDITOR
            var ohPtr = Unsafe.As<IDBOperationHandle<string>, System.IntPtr>(ref operationHandle);
            Save2IDB_ImportAsync(ohPtr, prefix, overwrite, ImportAsyncThen, CommonCatch);
#endif
            return operationHandle;
        }

        [MonoPInvokeCallback(typeof(ImportAsyncThenCallback))]
        private static void ImportAsyncThen(System.IntPtr ohPtr, string path)
        {
            var operationHandle = Unsafe.As<System.IntPtr, IDBOperationHandle<string>>(ref ohPtr);
            operationHandle.Done(path);
        }
    }
}
