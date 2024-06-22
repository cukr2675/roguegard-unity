using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using System.Reflection;
using Objforming;

namespace RoguegardUnity.Tests
{
    public class ObjformingTests
    {
        [Test]
        public void GetNotAnsweredTypes()
        {
            var asms = new[]
            {
                //Assembly.Load("mscorlib"),
                Assembly.Load("UnityEngine.CoreModule"),
                Assembly.Load("RuntimeDotter"),
                Assembly.Load("Roguegard"),
                Assembly.Load("Roguegard.CharacterCreation"),
                Assembly.Load("Roguegard.Device"),
                Assembly.Load("Roguegard.CoreData"),
                Assembly.Load("Roguegard.Rgpacks"),
                Assembly.Load("Roguegard.Rgpacks.MoonSharp")
            };

            var notAnsweredTypes = RelationalComponentDebugUtility.GetNotAnsweredTypes(asms);
            if (notAnsweredTypes.Length >= 1)
            {
                foreach (var type in notAnsweredTypes)
                {
                    Debug.LogError(
                        $"{type.Assembly.GetName().Name} の {type} は {nameof(RequireRelationalComponentAttribute)}" +
                        $"を設定された型を継承しますが、その答えは設定されていません。");
                }
            }
            else
            {
                Debug.Log($"{nameof(RequireRelationalComponentAttribute)} の答えが設定されていない型は見つかりませんでした。");
            }
        }

        [Test]
        public void CreateJsonSerializationModules()
        {
            ObjformingLogger.Primary = new RoguegardObjformingLogger();
            StandardRogueDeviceSave.GetJsonSerializationModules();
        }
    }
}
