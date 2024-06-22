using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Reflection;
using Objforming;

namespace Roguegard.Rgpacks
{
    internal static class RgpackLoader
    {
        private static Dictionary<System.Type, Former> formers;

        //public static T Get<T>(RogueObj world)
        //{
        //    if (formers == null)
        //    {
        //        formers = GetFormers();
        //    }

        //    GetRecursive
        //}

        //private static T GetRecursive<T>()
        //{
        //}

        private static Dictionary<System.Type, Former> GetFormersFor(System.Type type)
        {
            var assemblies = new[]
            {
                Assembly.Load("UnityEngine.CoreModule"),
                Assembly.Load("RuntimeDotter"),
                Assembly.Load("Roguegard"),
                Assembly.Load("Roguegard.CharacterCreation"),
                Assembly.Load("Roguegard.Device"),
                Assembly.Load("Roguegard.CoreData"),
                Assembly.Load("Roguegard.Rgpacks"),
                Assembly.Load("Roguegard.Rgpacks.MoonSharp")
            };
            var forms = new RelationalComponentListBuilder<Form>();
            forms.Add(Form.Create(typeof(RogueObj)));
            forms.AddAuto(assemblies, instanceType =>
            {
                if (instanceType.IsArray)
                {
                    var fieldElementType = instanceType.GetElementType();
                    return new RelationOnlyComponent(instanceType, fieldElementType);
                }
                if (instanceType.IsGenericType)
                {
                    var instanceTypeDefinition = instanceType.GetGenericTypeDefinition();
                    if (instanceTypeDefinition == typeof(List<>) || instanceTypeDefinition == typeof(Dictionary<,>))
                    {
                        return new RelationOnlyComponent(instanceType, instanceType.GenericTypeArguments);
                    }
                }
                if (instanceType.IsDefined(typeof(IgnoreRequireRelationalComponentAttribute)) ||
                    instanceType.IsDefined(typeof(ReferableAttribute)))
                {
                    return new RelationOnlyComponent(instanceType);
                }
                if (instanceType.IsDefined(typeof(FormableAttribute)))
                {
                    return Form.Create(instanceType);
                }
                return new RelationOnlyComponent(instanceType);
            });

            // T Çï€éùÇµÇ§ÇÈå^ÇæÇØÇéÊÇËèoÇ∑
            var relationalTypes = new List<System.Type>();
            var lastCount = relationalTypes.Count;
            relationalTypes.Add(type);
            while (true)
            {
                var count = relationalTypes.Count;
                for (int i = lastCount; i < count; i++)
                {
                    // í«â¡Ç≥ÇÍÇΩå^ÇéÊìæ
                    var addedRelationalType = relationalTypes[i];

                    // Ç‹Çæí«â¡Ç≥ÇÍÇƒÇ¢Ç»Ç¢å^ÇÃÇ§ÇøÅAí«â¡Ç≥ÇÍÇΩå^ÇÉÅÉìÉoÅ[Ç…éùÇ¬å^Çí«â¡Ç∑ÇÈÅB
                    foreach (var form in forms)
                    {
                        if (relationalTypes.Contains(form.InstanceType)) continue;
                        if (Contains(form, addedRelationalType))
                        {
                            relationalTypes.Add(form.InstanceType);
                        }
                    }
                }

                // âΩÇ‡í«â¡Ç≥ÇÍÇ»Ç≠Ç»Ç¡ÇΩÇÁèIóπ
                if (lastCount == count) break;

                lastCount = count;
            }

            var table = new Dictionary<System.Type, Former>();
            foreach (var form in forms)
            {
                if (!relationalTypes.Contains(form.InstanceType)) continue;

                table.Add(form.InstanceType, form.Former);
            }
            return table;
        }

        private static bool Contains(Form form, System.Type type)
        {
            for (int i = 0; i < form.FieldTypes.Count; i++)
            {
                if (form.FieldTypes[i] == type) return true;
            }
            return false;
        }

        private class Form : IRelationalComponent
        {
            public Former Former { get; }

            public System.Type InstanceType => Former.InstanceType;

            public IReadOnlyList<System.Type> FieldTypes { get; }

            public Form(Former former)
            {
                Former = former;
                FieldTypes = former.Members.Select(x => x.FieldType).ToArray();
            }

            public static Form Create(System.Type type, bool force = false, bool includeObjectMember = false)
            {
                var members = FormerMember.Generate(type, force, includeObjectMember);
                var former = new Former(type, members);
                return new Form(former);
            }

            public bool Overrides(IRelationalComponent other)
            {
                return false;
            }
        }
    }
}
