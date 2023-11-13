using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectFormer
{
    /// <summary>
    /// この型が参照によってシリアル化されることを宣言する。
    /// この属性を付与した型の名前変更は極力避ける。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ReferableAttribute : Attribute
    {
    }
}
