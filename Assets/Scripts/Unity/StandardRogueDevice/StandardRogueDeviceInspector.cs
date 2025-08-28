using Objforming;
using Objforming.Unity.RuntimeInspector;
using Roguegard;
using Roguegard.Objforming.RuntimeInspector;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
            var forms = new RelationalComponentListBuilder<RelationalForm>
            {
                new Int32Form(_inputElementPrefab),
                new SingleForm(_inputElementPrefab),
                new BooleanForm(_toggleElementPrefab),
                new StringForm(_inputElementPrefab),
                FormerForm.Create(typeof(Vector2Int), _linkElementPrefab, true),
                FormerForm.Create(typeof(RectInt), _linkElementPrefab, true),
                FormerForm.Create(typeof(Color32), _linkElementPrefab, true),
                FormerForm.Create(typeof(Color32?), _linkElementPrefab, true),
                RogueObjForm.Create(_linkElementPrefab, _buttonElementPrefab, x => SerializeRogueObj(x)),
                RogueObjListForm.Create(_rogueObjListItemElementPrefab, _linkElementPrefab),
                FormerForm.Create(typeof(StandardRogueDeviceData), _linkElementPrefab),
                FormerForm.Create(typeof(RogueOptions), _linkElementPrefab)
            };
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
