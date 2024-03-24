using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class ReferableScript
    {
#if UNITY_EDITOR
        /// <summary>
        /// <para>
        ///     <see cref="SerializeReference"/> でオブジェクトを保存するとき、
        ///     オブジェクトにフィールドが存在しないと参照が切れるまたはフィールドを追加したとき変更の保存に不具合が生じる。
        ///     それを回避するためにこのフィールドを定義する。
        /// </para>
        /// <para>
        ///     <see cref="ScriptableObject"/> による .asset ファイルでは string が最短なので string 型。
        /// </para>
        /// </summary>
        [SerializeField, AssemblyReflectionAssetDummy, Objforming.IgnoreMember] private string _assemblyReflectionAssetDummy;
#endif

        /// <summary>
        /// <see cref="ScriptField{T}"/> とアセンブリを合わせる。
        /// </summary>
        public class AssemblyReflectionAssetDummyAttribute : PropertyAttribute
        {
        }
    }
}
