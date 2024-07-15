using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;

namespace Roguegard.CharacterCreation
{
    public abstract class AppearanceOption : RogueDescriptionData, IAppearanceOption
    {
        /// <summary>
        /// この <see cref="AppearanceOption"/> の前提となる <see cref="NodeBone"/> の名前を取得する。
        /// null のときは自由枠とする。
        /// </summary>
        public abstract BoneKeyword BoneName { get; }

        public virtual Spanning<IMemberSource> MemberSources => Spanning<IMemberSource>.Empty;

        public virtual void UpdateMemberRange(IMember member, IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData)
        {
        }

        public abstract void Affect(
            NodeBone mainNode, AppearanceBoneSpriteTable boneSpriteTable,
            IReadOnlyAppearance appearance, ICharacterCreationData characterCreationData);
    }
}
