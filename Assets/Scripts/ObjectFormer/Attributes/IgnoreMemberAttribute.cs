using System;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;

namespace ObjectFormer
{
    /// <summary>
    /// ���̑������t�^���ꂽ�ϐ��� <see cref="ObjectFormer"/> �ɂ��V���A�����̑ΏۊO�Ƃ���
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class IgnoreMemberAttribute : Attribute
    {
    }
}
