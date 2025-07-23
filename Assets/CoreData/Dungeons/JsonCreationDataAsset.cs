using System.IO;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// Json テキストからなる <see cref="RogueObj"/> のクローンを生成する <see cref="CharacterCreationDataAsset"/>
    /// </summary>
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Character Creation Data/Json")]
    [Objforming.IgnoreRequireRelationalComponent]
    public class JsonCreationDataAsset : CharacterCreationDataAsset
    {
        [SerializeField] private TextAsset _json = null;

        private RogueObj CloneBase => _cloneBase ??= Deserialize(_json.text);
        private RogueObj _cloneBase;

        protected override bool HasNotInfoSet => true;
        public override string DescriptionName => CloneBase?.Main.InfoSet.Name ?? _json.name;
        public override Sprite Icon => CloneBase?.Main.InfoSet.Icon;
        public override Color Color => CloneBase.Main.InfoSet.Color;
        public override string Caption => CloneBase.Main.InfoSet.Caption;
        public override IRogueDetails Details => CloneBase.Main.InfoSet.Details;

        private static RogueObj Deserialize(string json)
        {
            if (RoguegardSettings.JsonSerialization == null) return null;

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();
            stream.Position = 0;
            return RoguegardSettings.JsonSerialization.Deserialize<RogueObj>(stream);
        }

        public override RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = CloneBase.Clone();
            if (!SpaceUtility.TryLocate(obj, location, position, stackOption)) throw new RogueException("生成したオブジェクトの移動に失敗しました。");

            obj.TrySetStack(startingItem.Stack);
            if (startingItem.OptionColor != null) { ColoringEffect.ColorChange(obj, startingItem.OptionColor.Value); }

            return obj;
        }

        protected override void GetCost(out float cost, out bool costIsUnknown)
        {
            cost = 0f;
            costIsUnknown = true;
        }
    }
}
