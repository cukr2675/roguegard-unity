using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [ObjectFormer.RequireRelationalComponent]
    public interface IAppearanceOption : IRogueDescription, IMemberableOption
    {
        /// <summary>
        /// この <see cref="IAppearanceOption"/> の前提となる <see cref="IBone"/> の名前を取得する。
        /// null のときは自由枠とする。
        /// </summary>
        IKeyword BoneName { get; }

        void UpdateMemberRange(IMember member, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData);

        void Affect(
            BoneNodeBuilder mainNode, AppearanceBoneSpriteTable boneSpriteTable, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData);
    }
}
