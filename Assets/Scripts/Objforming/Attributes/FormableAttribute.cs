using System;
using System.Reflection;

namespace Objforming
{
    /// <summary>
    /// この型が <see cref="Former"/> による自動シリアル化を許可することを宣言する。
    /// この属性を設定する型は private や internal であっても名前変更を避ける
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited = false)]
    public sealed class FormableAttribute : Attribute
    {
        public FormerMode Mode { get; }

        public FormableAttribute(FormerMode mode = FormerMode.Default)
        {
            Mode = mode;
        }

        public static FormerMode GetModeOrDefault(Type type)
        {
            var attribute = type.GetCustomAttribute<FormableAttribute>();
            return attribute?.Mode ?? FormerMode.Default;
        }
    }
}
