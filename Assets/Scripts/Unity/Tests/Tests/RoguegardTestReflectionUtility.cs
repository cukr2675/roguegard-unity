using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using System.Linq;
using System.Reflection;

namespace RoguegardUnity.Tests
{
    public static class RoguegardTestReflectionUtility
    {
        public static Assembly[] GetTestTargetAssemblies()
        {
            var assemblies = new[]
            {
                Assembly.Load("Roguegard"),
                Assembly.Load("Roguegard.CharacterCreation"),
                Assembly.Load("Roguegard.Device"),
                Assembly.Load("Roguegard.CoreData")
            };
            return assemblies;
        }

        public static List<T> GetInstancesOfInherited<T>(params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(x => x.GetTypes()).ToArray();
            var instances = new List<T>();
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface) continue;
                if (!typeof(T).IsAssignableFrom(type)) continue;

                var instance = (T)System.Activator.CreateInstance(type, true);
                instances.Add(instance);
            }
            return instances;
        }
    }
}
