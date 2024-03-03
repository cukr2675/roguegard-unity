using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectFormer.Unity.RuntimeInspector
{
    public class InspectorConfig : MonoBehaviour
    {
        private readonly List<RelationalForm> forms;
        private readonly RelationalForm fallbackForm;
        
        public object UpdateCoroutineWait { get; }

        public InspectorConfig(
            IEnumerable<InspectorModule> modules, IDependencyModuleTable<InspectorModule> moduleTable,
            RelationalForm fallbackForm = null, object updateCoroutineWait = null)
        {
            forms = new List<RelationalForm>();
            foreach (var module in modules)
            {
                var forms = module.GetAllForms(moduleTable);
                foreach (var form in forms)
                {
                    this.forms.Add(form);
                }
            }
            this.fallbackForm = fallbackForm;
            UpdateCoroutineWait = updateCoroutineWait;
        }

        public void SetFormTo(FormInspector inspector, object value)
        {
            var type = value?.GetType();
            if (type != null)
            {
                foreach (var form in forms)
                {
                    if (!form.CanConvert(type)) continue;

                    form.SetPageTo(inspector, value);
                    return;
                }
            }

            if (fallbackForm != null)
            {
                fallbackForm.SetPageTo(inspector, value);
            }
            else
            {
                throw new System.Exception($"{type} に対応するフォームが見つかりません。");
            }
        }

        public void AppendElementTo(FormInspector inspector, string key, System.Type type, ElementValueGetter getter, ElementValueSetter setter)
        {
            foreach (var form in forms)
            {
                if (!form.CanConvert(type)) continue;

                form.AppendElementTo(inspector, key, getter, setter);
                return;
            }

            var instanceType = getter()?.GetType();
            if (instanceType != null)
            {
                foreach (var form in forms)
                {
                    if (!form.CanConvert(instanceType)) continue;

                    form.AppendElementTo(inspector, key, getter, setter);
                    return;
                }
            }

            if (fallbackForm != null)
            {
                fallbackForm.AppendElementTo(inspector, key, getter, setter);
            }
            else
            {
                throw new System.Exception($"{type} に対応するフォームが見つかりません。");
            }
        }
    }
}
