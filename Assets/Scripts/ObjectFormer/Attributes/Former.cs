using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;

namespace ObjectFormer
{
    [Formable]
    public class Former
    {
        public Type InstanceType { get; }
        private readonly FormerMember[] _members;

        [NonSerialized] private MemberList memberList;
        public IReadOnlyList<FormerMember> Members => memberList ??= new MemberList() { parent = this };

        [NonSerialized] private bool constructorIsInitialized;
        [NonSerialized] private ConstructorInfo constructorInfo;
        [NonSerialized] private object[] constructorParameters;

        private const BindingFlags constructorBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private static readonly Type[] singleParametersTypes = new Type[0];
        private static readonly object[] singleParameters = new object[0];

        private Former() { }

        public Former(Type instanceType, IEnumerable<FormerMember> members)
        {
            InstanceType = instanceType;
            _members = members.ToArray();
        }

        public FormerMember GetMemberByCamel(string camelName)
        {
            foreach (var item in Members)
            {
                if (item.CamelName == camelName) return item;
            }
            throw new Exception($"{InstanceType} に {camelName} のメンバーは存在しません。");
        }

        public bool TryGetMemberByCamel(string camelName, out FormerMember member)
        {
            foreach (var item in Members)
            {
                if (item.CamelName == camelName)
                {
                    member = item;
                    return true;
                }
            }
            member = null;
            return false;
        }

        public object CreateInstance()
        {
            if (!constructorIsInitialized)
            {
                constructorInfo = GetConstructorInfo(InstanceType, out constructorParameters);
                constructorIsInitialized = true;
            }

            if (constructorInfo != null)
            {
                // 専用のコンストラクタを持つ構造体・クラスの場合
                var instance = constructorInfo.Invoke(constructorParameters);
                return instance;
            }
            else
            {
                // 専用のコンストラクタを持たない構造体の場合
                var instance = Activator.CreateInstance(InstanceType);
                return instance;
            }
        }

        private static ConstructorInfo GetConstructorInfo(Type instanceType, out object[] parameters)
        {
            var constructorInfos = instanceType.GetConstructors(constructorBindingFlags);
            foreach (var constructorInfo in constructorInfos)
            {
                if (constructorInfo.IsDefined(typeof(CreateInstanceAttribute), false))
                {
                    // 属性を持つコンストラクタが見つかった場合、それを使用する。
                    var parametersLength = constructorInfo.GetParameters().Length;
                    parameters = new object[parametersLength];
                    return constructorInfo;
                }
            }

            // 属性を持つコンストラクタが見つからなかった場合、引数なしコンストラクタを使用する
            if (instanceType.IsValueType)
            {
                // 構造体のデフォルトコンストラクタはリフレクションで取得できないため null にする
                parameters = null;
                return null;
            }
            else
            {
                var singleConstructorInfo = instanceType.GetConstructor(constructorBindingFlags, null, singleParametersTypes, null);
                if (singleConstructorInfo != null)
                {
                    parameters = singleParameters;
                    return singleConstructorInfo;
                }
                else
                {
                    throw new Exception(
                        $"{instanceType} で {nameof(CreateInstanceAttribute)} を持つコンストラクタまたは引数なしコンストラクタが見つかりません。");
                }
            }
        }

        private class MemberList : IReadOnlyList<FormerMember>
        {
            public Former parent;

            public FormerMember this[int index]
            {
                get
                {
                    parent._members[index].Open(parent.InstanceType);
                    return parent._members[index];
                }
            }

            public int Count => parent._members.Length;

            public IEnumerator<FormerMember> GetEnumerator()
            {
                foreach (var member in parent._members)
                {
                    member.Open(parent.InstanceType);
                    yield return member;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
