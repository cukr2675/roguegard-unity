using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;

namespace Objforming
{
    public class RelationalComponentListBuilder<T> : IReadOnlyList<T>
        where T : IRelationalComponent
    {
        private readonly List<T> components;

        public T this[int index] => components[index];

        public int Count => components.Count;

        public RelationalComponentListBuilder()
        {
            components = new List<T>();
        }

        public RelationalComponentListBuilder(IEnumerable<T> components)
        {
            this.components = new List<T>(components);
        }

        public void Add(T component)
        {
            if (component.InstanceType == null) throw new Exception(
                $"{nameof(IRelationalComponent.InstanceType)} を null にすることはできません。 ({component})");

            // AddAuto で不要なコンポーネントが追加されることを防ぐため、このリスト内での上書きは禁止する
            if (OverrideAny(components, component, out var pre)) throw new Exception(
                $"{typeof(RelationalComponentListBuilder<>)} 内でコンポーネントを上書きすることはできません。 {component} は {pre} を上書きします。");

            components.Add(component);
        }

        private bool OverrideAny<TC>(IEnumerable<TC> list, TC component, out TC pre)
            where TC : IRelationalComponent
        {
            foreach (var item in list)
            {
                if (component.Overrides(item))
                {
                    pre = item;
                    return true;
                }
            }
            pre = default;
            return false;
        }

        public void AddRange(IEnumerable<T> components)
        {
            foreach (var component in components)
            {
                Add(component);
            }
        }

        /// <summary>
        /// <paramref name="assemblies"/> で出現する型に対して <paramref name="creator"/> を実行し、その戻り値を追加する。
        /// このリストに追加済みの要素または <paramref name="creator"/> の戻り値の、
        /// <see cref="IRelationalComponent.FieldTypes"/> で出現しない型に対しては実行・追加しない。
        /// </summary>
        public void AddAuto(IEnumerable<Assembly> assemblies, RelationalComponentCreator creator)
        {
            var instanceTypes = assemblies.SelectMany(x => x.GetTypes()).Where(type => !type.IsAbstract && !type.IsInterface).ToArray();
            var types = new HashSet<Type>();
            var tempComponents = new List<IRelationalComponent>(components.Select(x => (IRelationalComponent)x));
            for (int i = 0; i < tempComponents.Count; i++)
            {
                var component = tempComponents[i]; // このメソッド中で追加されたコンポーネントも処理する。
                var fieldTypes = component.FieldTypes;

                foreach (var fieldType in fieldTypes)
                {
                    if (fieldType == typeof(object)) continue;
                    if (!types.Add(fieldType)) continue;

                    if ((fieldType.IsAbstract || fieldType.IsInterface) && fieldType != typeof(Type))
                    {
                        // 変数型となる抽象クラスかインターフェースが存在する場合、以下の属性を必要とする。
                        if (!fieldType.IsDefined(typeof(RequireRelationalComponentAttribute)))
                        {
                            ObjformingLogger.LogWarning(
                                $"[{component.InstanceType}] {fieldType} に {typeof(RequireRelationalComponentAttribute)} が設定されていません。");
                        }
                    }

                    // 型が持つ変数に入る可能性がある型を取得する
                    var availableInstanceTypes = instanceTypes.Where(x => fieldType.IsAssignableFrom(x));
                    if (!fieldType.IsAbstract && !fieldType.IsInterface && (fieldType.IsGenericType || fieldType.IsArray))
                    {
                        // 上の条件だけでは処理できないジェネリック型と配列を加える。（非抽象クラス・非インターフェースのみ）
                        availableInstanceTypes = availableInstanceTypes.Append(fieldType);
                    }

                    foreach (var instanceType in availableInstanceTypes)
                    {
                        // 対応するコンポーネントがすでに存在していれば何もしない。
                        if (tempComponents.Any(x => x.InstanceType == instanceType)) continue;

                        // コンポーネントが必要な型であれば、コンポーネントを追加する。
                        var instanceTypeComponent = creator(instanceType);
                        if (instanceTypeComponent == null)
                        {
                            ObjformingLogger.LogError($"[{component.InstanceType}] {instanceType} のコンポーネントが必要です。");
                            continue;
                        }

                        // AddAuto で不要なコンポーネントが追加されることを防ぐため、このリスト内での上書きは禁止する
                        if (OverrideAny(tempComponents, instanceTypeComponent, out var pre)) throw new Exception(
                            $"{nameof(AddAuto)} 内でコンポーネントを上書きすることはできません。 {instanceTypeComponent} は {pre} を上書きします。");

                        tempComponents.Add(instanceTypeComponent);
                        if (instanceTypeComponent is T t) { Add(t); }
                    }
                }
            }
        }

        /// <summary>
        /// 要素の <see cref="IRelationalComponent.InstanceType"/> と <see cref="IRelationalComponent.FieldTypes"/> で出現するすべての型から、
        /// <see cref="object"/> 型を除いた配列を取得する。
        /// </summary>
        public Type[] GetTypesOtherThanObject()
        {
            var types = new HashSet<Type>();
            for (int i = 0; i < components.Count; i++)
            {
                var component = components[i];
                types.Add(component.InstanceType);
                types.UnionWith(component.FieldTypes);
            }
            types.Remove(typeof(object)); // object 型を除く
            return types.ToArray();
        }

        public IEnumerator<T> GetEnumerator() => components.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => components.GetEnumerator();
    }
}
