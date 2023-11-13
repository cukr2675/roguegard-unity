using System;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace ObjectFormer
{
    [Formable]
    public class FormerMember
    {
        /// <summary>
        /// パスカルケースのメンバ名を取得する
        /// </summary>
        public string Name { get; }

        public Type FieldType { get; }

        public string RuntimeFieldName { get; }

        [NonSerialized] private FieldInfo fieldInfo;

        /// <summary>
        /// LowerCamelCase での <see cref="Name"/> を取得する
        /// </summary>
        public string CamelName => _camelName ??= Regex.Replace(Name, @"^[A-Z]+", x => x.Value.ToLowerInvariant()); // 先頭の連続した大文字をすべて小文字にする
        [NonSerialized] private string _camelName;

        private FormerMember() { }

        private FormerMember(string name, Type fieldType, FieldInfo fieldInfo)
        {
            Name = name;
            FieldType = fieldType;
            RuntimeFieldName = fieldInfo.Name;
            this.fieldInfo = fieldInfo;
        }

        /// <summary>
        /// 現在のビルドでの <paramref name="type"/> の <see cref="FormerMember"/> 配列を取得する。
        /// </summary>
        public static FormerMember[] Generate(Type type, bool force = false)
        {
            if (type.IsAbstract || type.IsInterface || type.IsGenericTypeDefinition) throw new ArgumentException();

            if (!force)
            {
                if (!type.IsDefined(typeof(FormableAttribute))) throw new ArgumentException($"{type} は Formable ではありません。");
            }

            var members = new List<FormerMember>();
            AddFields(type);
            return members.ToArray();

            void AddFields(Type type1)
            {
                if (type1.BaseType != null) { AddFields(type1.BaseType); }

                var fieldInfos = type1.GetRuntimeFields();
                foreach (var fieldInfo in fieldInfos)
                {
                    if (TryCreate(fieldInfo, out var member))
                    {
                        if (members.Select(x => x.Name).Contains(member.Name)) throw new Exception(
                            $"{type1} のメンバ名 {member.Name} は二つ以上存在します。");

                        members.Add(member);
                    }
                }
            }
        }

        private static bool TryCreate(FieldInfo fieldInfo, out FormerMember member)
        {
            if (fieldInfo.IsStatic || fieldInfo.IsNotSerialized)
            {
                member = default;
                return false;
            }

            // Name を自動設定する
            var name = fieldInfo.Name;
            if (name == "value__" && fieldInfo.DeclaringType.IsEnum)
            {
                // 列挙型のフィールドである場合は Value を使う
                name = "Value";
            }
            else if (name.StartsWith("<") && name.EndsWith(">k__BackingField"))
            {
                // BackingField (自動実装プロパティのフィールド) である場合は <> 内のみ使う
                var length = name.IndexOf('>') - 1;
                name = name.Substring(1, length);
            }

            // 名前の先頭から m_ を取り除く
            if (name.StartsWith("m_"))
            {
                name = name.Substring(2);
            }

            // 名前をパスカルケースにする
            var nameBuilder = new StringBuilder();
            var nextIsUpper = true; // 先頭は大文字
            foreach (var nameChar in name)
            {
                if (nameChar == '_')
                {
                    // アンダースコアは追加せず、次の文字を大文字にさせる
                    nextIsUpper = true;
                }
                else if (nextIsUpper)
                {
                    nameBuilder.Append(char.ToUpperInvariant(nameChar));
                    nextIsUpper = false;
                }
                else
                {
                    // もともと大文字だったものはそのまま追加
                    nameBuilder.Append(nameChar);
                }
            }
            name = nameBuilder.ToString();

            member = new FormerMember(name, fieldInfo.FieldType, fieldInfo);
            return true;
        }

        internal void Open(Type instanceType)
        {
            if (fieldInfo == null)
            {
                fieldInfo = instanceType.GetField(RuntimeFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        public object GetValue(object instance)
        {
            var value = fieldInfo.GetValue(instance);
            return value;
        }

        public void SetValue(object instance, object value)
        {
            fieldInfo.SetValue(instance, value);
        }
    }
}
