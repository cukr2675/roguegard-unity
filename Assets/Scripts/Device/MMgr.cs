using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public abstract class MMgr : StandardListMenuManager<MMgr, MArg>
    {
        public static string CharacterCreationName => "CharacterCreation";

        public abstract string TextEditorValue { get; }

        public abstract object Paint { get; }

        public abstract ISelectOption LoadPresetSelectOptionOfCharacterCreation { get; }

        public abstract void PushMenuScreen(
            MenuScreen<MMgr, MArg> menuScreen,
            RogueObj self = null,
            RogueObj user = null,
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            EffectableValue value = null,
            RogueObj tool = null,
            object other = null);

        public abstract void OpenTextEditor(string text);
        public abstract void SetObj(object obj);
        public abstract void SetResult(RogueObj player, RogueObj dungeon);
        public abstract void SetGameOver(RogueObj player, RogueObj dungeon);
        public abstract void SetQuest(RogueObj player, object quest, bool showSubmitButton);
        public abstract void SetPaint(
            object dotterBoards, RogueObj self, object other, int count, bool showSplitLine, Vector2[] pivots, HandleClickElement<MMgr, MArg> back);

        public abstract void AddInt(IKeyword keyword, int integer);
        public abstract void AddFloat(IKeyword keyword, float number);
        public abstract void AddObject(IKeyword keyword, object obj);
        public void AddWork(IKeyword keyword, in RogueCharacterWork work) => throw new System.NotSupportedException();
    }
}
