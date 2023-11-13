using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Roguegard;
using Roguegard.CharacterCreation;

namespace RoguegardUnity.Tests
{
    public class CommonPredicatorTest : ScriptableObject
    {
        [SerializeField] private RoguegardSettingsData _settings = null;
        [SerializeField] private ScriptableCharacterCreationData _player = null;
        [SerializeField] private ScriptableCharacterCreationData _enemy = null;
        [SerializeField] private ScriptableCharacterCreationData _tool = null;

        [Test]
        public void PoolingTest()
        {
            _settings.TestLoad();
            StaticID.Next();

            var random = new RogueRandom(0);
            var player = _player.CreateObj(null, Vector2Int.zero, random);
            var enemy = _enemy.CreateObj(null, Vector2Int.zero, random);
            var tool = _tool.CreateObj(null, Vector2Int.zero, random);

            var assemblies = RoguegardTestReflectionUtility.GetTestTargetAssemblies();
            var methodTargets = RoguegardTestReflectionUtility.GetInstancesOfInherited<IRogueMethodTarget>(assemblies);
            foreach (var methodTarget in methodTargets)
            {
                StaticID.Next();

                using var predicator1 = methodTarget.GetPredicator(player, 0f, tool);
                if (predicator1 == null)
                {
                    Debug.LogWarning($"{methodTarget.GetType()} の {nameof(IRoguePredicator)} を取得できませんでした。");
                    continue;
                }

                predicator1.Predicate(player, enemy, Vector2Int.zero);
                predicator1.EndPredicate();
                var predicator1Count = predicator1.Positions.Count;

                using var predicator2 = methodTarget.GetPredicator(player, 0f, tool);
                predicator2.Predicate(player, enemy, Vector2Int.zero);
                predicator2.EndPredicate();
                var predicator2Count = predicator2.Positions.Count;

                if (predicator1.Positions.Count != predicator1Count && predicator2.Positions.Count != predicator2Count)
                {
                    Debug.LogError($"[{methodTarget.GetType()}] プーリング：失敗");
                }
                else
                {
                    Debug.Log($"[{methodTarget.GetType()}] プーリング：成功");
                }
            }
        }
    }
}
