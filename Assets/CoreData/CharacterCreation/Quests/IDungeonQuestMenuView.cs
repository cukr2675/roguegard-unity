using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard.Device
{
    public interface IDungeonQuestMenuView : IModelsMenuView
    {
        void SetQuest(RogueObj player, DungeonQuest quest, bool showSubmitButton);
    }
}
