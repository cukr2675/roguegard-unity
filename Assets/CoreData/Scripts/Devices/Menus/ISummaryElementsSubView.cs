using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.CharacterCreation;

namespace Roguegard.Device
{
    public interface ISummaryElementsSubView : IElementsSubView
    {
        void SetObj(object obj, MMgr manager);
        void SetResult(RogueObj player, RogueObj dungeon, MMgr manager);
        void SetGameOver(RogueObj player, RogueObj dungeon, MMgr manager);
        void SetQuest(RogueObj player, DungeonQuest quest, bool showSubmitButton, MMgr manager);
    }
}
