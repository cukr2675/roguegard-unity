using System;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;

namespace ObjectFormer
{
    /// <summary>
    /// この属性が付与された変数は <see cref="ObjectFormer"/> によるシリアル化の対象外とする
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreMemberAttribute : Attribute
    {
    }
}
