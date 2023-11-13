using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;

namespace ObjectFormer
{
    public static class RelationalComponentDebugUtility
    {
        /// <summary>
        /// <see cref="RequireRelationalComponentAttribute"/> を設定された型を継承する型のうち、その答えが設定されていない型を取得する。
        /// </summary>
        public static Type[] GetNotAnsweredTypes(params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(x => x.GetTypes()).ToArray();
            var requirers = GetRequirers(types);
            var result = GetNotAnsweredTypes(types, requirers);
            return result;
        }

        /// <summary>
        /// <see cref="RequireRelationalComponentAttribute"/> を持つ型を取得する。
        /// </summary>
        private static Type[] GetRequirers(IReadOnlyList<Type> types)
        {
            var requirers = new List<Type>();
            foreach (var type in types)
            {
                if (!type.IsDefined(typeof(RequireRelationalComponentAttribute))) continue;

                requirers.Add(type);
            }
            return requirers.ToArray();
        }

        private static Type[] GetNotAnsweredTypes(IReadOnlyList<Type> types, IReadOnlyList<Type> requirers)
        {
            var notAnsweredTypes = new List<Type>();
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                if (type.IsAbstract || type.IsInterface) continue;
                if (!GetBeRequired(type)) continue;
                if (GetAnswered(type)) continue;

                notAnsweredTypes.Add(type);
            }
            return notAnsweredTypes.ToArray();

            bool GetBeRequired(Type type1)
            {
                for (int i = 0; i < requirers.Count; i++)
                {
                    if (requirers[i].IsAssignableFrom(type1)) return true;
                }
                return false;
            }

            bool GetAnswered(Type type1)
            {
                return
                    type1.IsDefined(typeof(FormableAttribute)) ||
                    type1.IsDefined(typeof(ReferableAttribute)) ||
                    type1.IsDefined(typeof(IgnoreRequireRelationalComponentAttribute));
            }
        }

        private static List<Type> GetFormableTypes(params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(x => x.GetTypes()).ToArray();
            var formableTypes = new List<Type>();
            foreach (var type in types)
            {
                if (!type.IsDefined(typeof(FormableAttribute))) continue;

                formableTypes.Add(type);
            }
            return formableTypes;
        }

        private static List<Type> GetReferableTypes(params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(x => x.GetTypes()).ToArray();
            var referableTypes = new List<Type>();
            foreach (var type in types)
            {
                if (!type.IsDefined(typeof(ReferableAttribute))) continue;

                referableTypes.Add(type);
            }
            return referableTypes;
        }

        /// <summary>
        /// 指定された <see cref="IRelationalComponent"/> のリストから無限再帰する値型を取得する。
        /// </summary>
        public static HashSet<Type> GetRecursiveValueTypes(IReadOnlyList<IRelationalComponent> components)
        {
            var typeStack = new List<Type>();
            var result = new HashSet<Type>();
            foreach (var component in components)
            {
                if (!component.InstanceType.IsValueType) continue;

                typeStack.Add(component.InstanceType);
                GetRecursiveValueTypes(typeStack, result, components);
                typeStack.RemoveAt(typeStack.Count - 1);
            }
            return result;
        }

        private static void GetRecursiveValueTypes(List<Type> typeStack, HashSet<Type> recursiveValueTypes, IReadOnlyList<IRelationalComponent> components)
        {
            var type = typeStack[typeStack.Count - 1];
            if (!type.IsValueType) throw new ArgumentException($"{type} は値型ではありません。");
            if (!TryGetComponentOf(type, out var component)) throw new Exception($"{type} の {nameof(IRelationalComponent)} が見つかりません。");

            foreach (var fieldType in component.FieldTypes)
            {
                if (!fieldType.IsValueType) continue;

                if (typeStack.Contains(fieldType))
                {
                    // 再帰発見
                    recursiveValueTypes.Add(fieldType);
                    continue;
                }

                typeStack.Add(fieldType);
                GetRecursiveValueTypes(typeStack, recursiveValueTypes, components);
                typeStack.RemoveAt(typeStack.Count - 1);
            }

            bool TryGetComponentOf(Type instanceType, out IRelationalComponent component)
            {
                foreach (var item in components)
                {
                    if (item.InstanceType == instanceType)
                    {
                        component = item;
                        return true;
                    }
                }
                component = null;
                return false;
            }
        }
    }
}
