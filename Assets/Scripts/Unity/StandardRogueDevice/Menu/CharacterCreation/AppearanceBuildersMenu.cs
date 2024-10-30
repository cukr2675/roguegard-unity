using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using ListingMF;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    public class AppearanceBuildersMenu : RogueMenuScreen
    {
        private readonly List<object> elms = new();
        private static readonly object addLeftEyeElement = new object();
        private static readonly object addRightEyeElement = new object();
        private static readonly object addHairElement = new object();
        private static readonly object addOtherElement = new object();

        public CharacterCreationOptionMenu NextMenu { get; set; }
        public CharacterCreationAddMenu AddMenu { get; set; }

        private readonly ScrollViewTemplate<object, RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
        };

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            if (!(arg.Arg.Other is CharacterCreationDataBuilder builder)) throw new RogueException();

            elms.Clear();

            if (builder.Appearances.TryGetBuilder(BoneKeyword.LeftEye, out var leftEyeBuilder))
            {
                elms.Add(leftEyeBuilder);
            }
            else
            {
                elms.Add(addLeftEyeElement);
            }

            if (builder.Appearances.TryGetBuilder(BoneKeyword.RightEye, out var rightEyeBuilder))
            {
                elms.Add(rightEyeBuilder);
            }
            else
            {
                elms.Add(addRightEyeElement);
            }

            if (builder.Appearances.TryGetBuilder(BoneKeyword.Hair, out var hairBuilder))
            {
                elms.Add(hairBuilder);
            }
            else
            {
                elms.Add(addHairElement);
            }

            for (int i = 0; i < builder.Appearances.Count; i++)
            {
                var appearanceBuilder = builder.Appearances[i];
                if (elms.Contains(appearanceBuilder)) continue;

                elms.Add(appearanceBuilder);
            }
            elms.Add(addOtherElement);

            view.ShowTemplate(elms, manager, arg)
                ?
                .ElementNameFrom((element, manager, arg) =>
                {
                    if (element is AppearanceBuilder builder)
                    {
                        return builder.Name;
                    }
                    else
                    {
                        return "+ Œ©‚½–Ú‚ð’Ç‰Á";
                    }
                })

                .OnClickElement((element, manager, arg) =>
                {
                    if (element is AppearanceBuilder builder)
                    {
                        manager.PushMenuScreen(NextMenu, arg.Self, other: builder);
                    }
                    else
                    {
                        manager.PushMenuScreen(AddMenu, arg.Self, other: typeof(AppearanceBuilder));
                    }
                })

                .Build();
        }
    }
}
