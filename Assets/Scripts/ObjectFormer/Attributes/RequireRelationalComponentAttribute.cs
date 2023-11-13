using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectFormer
{
    /// <summary>
    /// この属性を付与された、クラスの子クラスまたはインターフェースを実装するクラスに
    /// <see cref="FormableAttribute"/> または <see cref="ReferableAttribute"/>
    /// または <see cref="IgnoreRequireRelationalComponentAttribute"/> が必要であることを示す。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public sealed class RequireRelationalComponentAttribute : Attribute
    {
    }
}
