using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface ISortedIntrinsic
    {
        int Lv { get; }

        /// <summary>
        /// <see cref="Lv"/> に到達したとき実行する。付与順を維持するため <see cref="Lv"/> に達したときだけ実行する。
        /// </summary>
        void LevelUpToLv(RogueObj self, MainInfoSetType infoSetType);

        /// <summary>
        /// <see cref="Lv"/> から下がったとき実行する。付与順を維持するため <see cref="Lv"/> から下がったときだけ実行する。
        /// </summary>
        void LevelDownFromLv(RogueObj self, MainInfoSetType infoSetType);

        void Open(RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base);

        void Close(RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph);
    }
}
