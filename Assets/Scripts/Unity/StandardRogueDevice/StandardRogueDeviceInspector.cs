using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Reflection;
using System.IO;
using Objforming;
using Objforming.Unity.RuntimeInspector;
using Roguegard;
using Roguegard.Objforming.RuntimeInspector;

namespace RoguegardUnity
{
    public class StandardRogueDeviceInspector : MonoBehaviour
    {
        [SerializeField] private FormInspector _inspector = null;

        [SerializeField] private InputElement _inputElementPrefab = null;
        [SerializeField] private ToggleElement _toggleElementPrefab = null;
        [SerializeField] private LinkElement _linkElementPrefab = null;
        [SerializeField] private ButtonElement _buttonElementPrefab = null;
        [SerializeField] private RogueObjListItemElement _rogueObjListItemElementPrefab = null;

        private static readonly string[] invalidFileNameChars = Path.GetInvalidFileNameChars().Select(x => x.ToString()).ToArray();

        public void Initialize()
        {
            var config = GetInspectorConfig();
            _inspector.Initialize(config);
        }

        public void SetRoot(object root)
        {
            _inspector.SetRoot(root);
        }

        private InspectorModule[] GetInspectorModules()
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
            var forms = new RelationalComponentListBuilder<RelationalForm>();
            forms.Add(new Int32Form(_inputElementPrefab));
            forms.Add(new SingleForm(_inputElementPrefab));
            forms.Add(new BooleanForm(_toggleElementPrefab));
            forms.Add(new StringForm(_inputElementPrefab));
            forms.Add(FormerForm.Create(typeof(Vector2Int), _linkElementPrefab, true));
            forms.Add(FormerForm.Create(typeof(RectInt), _linkElementPrefab, true));
            forms.Add(FormerForm.Create(typeof(Color32), _linkElementPrefab, true));
            forms.Add(RogueObjForm.Create(_linkElementPrefab, _buttonElementPrefab, x => SerializeRogueObj(x)));
            forms.Add(RogueObjListForm.Create(_rogueObjListItemElementPrefab, _linkElementPrefab));
            forms.Add(FormerForm.Create(typeof(StandardRogueDeviceData), _linkElementPrefab));
            forms.Add(FormerForm.Create(typeof(RogueOptions), _linkElementPrefab));
            forms.AddAuto(assemblies, instanceType =>
            {
                if (instanceType.IsArray)
                {
                    var fieldElementType = instanceType.GetElementType();
                    return Array1Form.Create(instanceType, _linkElementPrefab);
                }
                if (instanceType.IsGenericType)
                {
                    var instanceTypeDefinition = instanceType.GetGenericTypeDefinition();
                    if (instanceTypeDefinition == typeof(List<>))
                    {
                        return ListForm.Create(instanceType, _linkElementPrefab);
                    }
                    if (instanceTypeDefinition == typeof(Dictionary<,>))
                    {
                        return DictionaryForm.Create(instanceType, _linkElementPrefab);
                    }
                }
                if (instanceType.IsDefined(typeof(IgnoreRequireRelationalComponentAttribute)) ||
                    instanceType.IsDefined(typeof(ReferableAttribute)))
                {
                    return new RelationOnlyComponent(instanceType);
                }
                if (instanceType.IsDefined(typeof(FormableAttribute)))
                {
                    return FormerForm.Create(instanceType, _linkElementPrefab);
                }
                return new RelationOnlyComponent(instanceType);
            });

            var name = "Core";
            var version = new System.Version(0, 1, 0);
            var module = new InspectorModule(name, version, forms);

            return new[] { module };
        }

        private InspectorConfig GetInspectorConfig()
        {
            var modules = GetInspectorModules();
            var moduleTable = new DependencyModuleTable<InspectorModule>();
            var fallbackForm = new FallbackForm(_inputElementPrefab);
            var config = new InspectorConfig(modules, moduleTable, fallbackForm);
            return config;
        }

        private static void SerializeRogueObj(RogueObj obj)
        {
            var name = obj.GetName();
            foreach (var invalidFileNameChar in invalidFileNameChars)
            {
                name = name.Replace(invalidFileNameChar, "");
            }
            var path = $"Temp/{name}.json";
            var clone = obj.Clone();
            using (var stream = RogueFile.Create(path))
            {
                RoguegardSettings.JsonSerialization.Serialize(stream, clone);
            }
            RogueFile.Export(path);
        }
    }
}
