using System;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;

namespace Objforming
{
    /// <summary>
    /// ���̑������t�^���ꂽ�ϐ��� <see cref="Objforming"/> �ɂ��V���A�����̑ΏۊO�Ƃ���
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreMemberAttribute : Attribute
    {
    }
}
