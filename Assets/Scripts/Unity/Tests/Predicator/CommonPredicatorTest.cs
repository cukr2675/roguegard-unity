using NUnit.Framework;
using Roguegard;
using Roguegard.CharacterCreation;
using UnityEngine;

namespace RoguegardUnity.Tests
{
    public class CommonPredicatorTest : ScriptableObject
    {
        [SerializeField] private RoguegardSettingsData _settings = null;
        [SerializeField] private CharacterCreationDataAsset _player = null;
        [SerializeField] private CharacterCreationDataAsset _enemy = null;
        [SerializeField] private CharacterCreationDataAsset _tool = null;

        [Test]
        public void PoolingTest()
        {
            _settings.TestLoad();
            StaticId.Next();

            var random = new RogueRandom(0);
            var player = _player.CreateObj(null, Vector2Int.zero, random);
            var enemy = _enemy.CreateObj(null, Vector2Int.zero, random);
            var tool = _tool.CreateObj(null, Vector2Int.zero, random);

            var assemblies = RoguegardTestReflectionUtility.GetTestTargetAssemblies();
            var methodTargets = RoguegardTestReflectionUtility.GetInstancesOfInherited<IRogueMethodTarget>(assemblies);
            foreach (var methodTarget in methodTargets)
            {
                StaticId.Next();

                using var predicator1 = methodTarget.GetPredicator(player, 0f, tool);
                if (predicator1 == null)
                {
                    Debug.LogWarning($"{methodTarget.GetType()} の {nameof(IRoguePredicator)} を取得できませんでした。");
                    continue;
                }

                predicator1.Predicate(player, enemy, Vector2Int.zero);
                predicator1.EndPredicate();
                var predicator1Count = predicator1.Positions.Length;

                using var predicator2 = methodTarget.GetPredicator(player, 0f, tool);
                predicator2.Predicate(player, enemy, Vector2Int.zero);
                predicator2.EndPredicate();
                var predicator2Count = predicator2.Positions.Length;

                if (predicator1.Positions.Length != predicator1Count && predicator2.Positions.Length != predicator2Count)
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
