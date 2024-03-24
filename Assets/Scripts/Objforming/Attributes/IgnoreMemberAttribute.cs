using System;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;

namespace Objforming
{
    /// <summary>
    /// この属性が付与された変数は <see cref="Objforming"/> によるシリアル化の対象外とする
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreMemberAttribute : Attribute
    {
    }
}
