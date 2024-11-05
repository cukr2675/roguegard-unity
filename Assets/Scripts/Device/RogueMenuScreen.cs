using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public abstract class RogueMenuManager : StandardListMenuManager<RogueMenuManager, ReadOnlyMenuArg>
    {
        public static string CharacterCreationName => "CharacterCreation";

        public abstract string TextEditorValue { get; }

        public abstract ISelectOption LoadPresetSelectOptionOfCharacterCreation { get; }

        public abstract void PushMenuScreen(
            MenuScreen<RogueMenuManager, ReadOnlyMenuArg> menuScreen,
            RogueObj self = null, RogueObj user = null,
            RogueObj targetObj = null,
            int count = default,
            Vector2 vector = default,
            EffectableValue value = null,
            RogueObj tool = null,
            object other = null);

        public abstract void OpenTextEditor(string text);

        public void Reopen()
        {
            //currentMenuIsDialog = true;
            //Back();
        }

        public abstract void AddInt(IKeyword keyword, int integer);
        public abstract void AddFloat(IKeyword keyword, float number);
        public abstract void AddObject(IKeyword keyword, object obj);
        public void AddWork(IKeyword keyword, in RogueCharacterWork work) => throw new System.NotSupportedException();
    }

    public class MenuArg
    {
        public RogueObj Self { get; set; }
        public RogueObj User { get; set; }
        public RogueMethodArgument Arg { get; set; }
        public ReadOnlyMenuArg ReadOnly { get; }

        public MenuArg(RogueObj self = null, RogueObj user = null, RogueMethodArgument arg = default)
        {
            Self = self;
            User = user;
            Arg = arg;
            ReadOnly = new ReadOnlyMenuArg(this);
        }
    }

    public class ReadOnlyMenuArg : IListMenuArg
    {
        private readonly MenuArg source;

        public RogueObj Self => source.Self;
        public RogueObj User => source.User;
        public RogueMethodArgument Arg => source.Arg;

        public ReadOnlyMenuArg(MenuArg source)
        {
            this.source = source;
        }

        public void CopyTo(ref IListMenuArg dest)
        {
            if (!(dest is MenuArgCopy destArg)) { dest = destArg = new MenuArgCopy(); }

            destArg.Self = Self;
            destArg.User = User;
            destArg.Arg = Arg;
        }

        private class MenuArgCopy : MenuArg, IListMenuArg
        {
            public void CopyTo(ref IListMenuArg dest)
            {
                if (!(dest is MenuArgCopy destArg)) { dest = destArg = new MenuArgCopy(); }

                destArg.Self = Self;
                destArg.User = User;
                destArg.Arg = Arg;
            }
        }
    }

    public abstract class RogueMenuScreen : MenuScreen<RogueMenuManager, ReadOnlyMenuArg>
    {
    }
}
