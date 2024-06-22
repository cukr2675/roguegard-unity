using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [Objforming.Formable]
    public class ObjStartingItemOption : IStartingItemOption
    {
        public RogueObj Obj { get; set; }

        public IMainInfoSet InfoSet => Obj.Main.InfoSet;
        public int Lv => Obj.Main.Stats.Lv;
        public Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => Spanning<IWeightedRogueObjGeneratorList>.Empty;

        public string Name => Obj.Main.InfoSet.Name;
        public Sprite Icon => Obj.Main.InfoSet.Icon;
        public Color Color => Obj.Main.InfoSet.Color;
        public string Caption => Obj.Main.InfoSet.Caption;
        public IRogueDetails Details => Obj.Main.InfoSet.Details;
        public Spanning<IMemberSource> MemberSources => Spanning<IMemberSource>.Empty;

        public float GetCost(IReadOnlyStartingItem startingItem, out bool costIsUnknown)
        {
            costIsUnknown = Obj.Main.InfoSet.CostIsUnknown;
            return Obj.Main.InfoSet.Cost;
        }

        public void UpdateMemberRange(IMember member, IReadOnlyStartingItem startingItem, ICharacterCreationData characterCreationData)
        {
        }

        public RogueObj CreateObj(
            IReadOnlyStartingItem startingItem, RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
        {
            var obj = Obj.Clone();
            obj.TrySetStack(startingItem.Stack);
            if (!SpaceUtility.TryLocate(obj, location, position, stackOption)) throw new RogueException();

            return obj;
        }
    }
}
