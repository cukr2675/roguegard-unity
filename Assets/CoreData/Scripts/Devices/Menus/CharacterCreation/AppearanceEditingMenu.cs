using Lysionium;
using OchalikeSprites;
using Roguegard.CharacterCreation;
using System.Collections.Generic;

namespace Roguegard.Device
{
    public class AppearanceEditingMenu : RogueMenuScreen
    {
        private readonly List<object> elms = new();
        private static readonly object addLeftEyeElement = new();
        private static readonly object addRightEyeElement = new();
        private static readonly object addHairElement = new();
        private static readonly object addOtherElement = new();

        public CharacterCreationOptionMenu NextMenu { get; set; }
        public CharacterCreationAddMenu AddMenu { get; set; }

        private readonly ScrollViewData<object, MMgr, MArg> view = new()
        {
        };

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            if (arg.Arg.Other is not CharacterCreationData characterCreationData) throw new System.InvalidOperationException();

            elms.Clear();

            if (characterCreationData.Appearances.TryGetValue(BoneKeyword.LeftEye, out var leftEye))
            {
                elms.Add(leftEye);
            }
            else
            {
                elms.Add(addLeftEyeElement);
            }

            if (characterCreationData.Appearances.TryGetValue(BoneKeyword.RightEye, out var rightEye))
            {
                elms.Add(rightEye);
            }
            else
            {
                elms.Add(addRightEyeElement);
            }

            if (characterCreationData.Appearances.TryGetValue(BoneKeyword.Hair, out var hair))
            {
                elms.Add(hair);
            }
            else
            {
                elms.Add(addHairElement);
            }

            for (int i = 0; i < characterCreationData.Appearances.Count; i++)
            {
                var appearance = characterCreationData.Appearances[i];
                if (elms.Contains(appearance)) continue;

                elms.Add(appearance);
            }
            elms.Add(addOtherElement);

            view.Show(elms, manager, arg)
                ?
                .NameFrom((element, manager, arg) =>
                {
                    if (element is Appearance appearance)
                    {
                        return appearance.Name;
                    }
                    else
                    {
                        return "+ 見た目を追加";
                    }
                })

                .OnClick((element, manager, arg) =>
                {
                    if (element is Appearance appearance)
                    {
                        manager.PushMenuScreen(NextMenu, arg.Self, other: appearance);
                    }
                    else
                    {
                        manager.PushMenuScreen(AddMenu, arg.Self, other: typeof(Appearance));
                    }
                })

                .Build();
        }
    }
}
