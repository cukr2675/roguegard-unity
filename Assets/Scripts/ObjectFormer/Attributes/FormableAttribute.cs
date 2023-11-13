using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectFormer
{
    /// <summary>
    /// この型が <see cref="Former"/> による自動シリアル化を許可することを宣言する。
    /// この属性を設定する型は private や internal であっても名前変更を避ける
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited = false)]
    public sealed class FormableAttribute : Attribute
    {
    }
}
