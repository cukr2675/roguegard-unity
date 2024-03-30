using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    public class AppearanceBuildersMenu : BaseScrollModelsMenu<object>
    {
        private readonly List<object> models = new();
        private static readonly object addLeftEyeModel = new object();
        private static readonly object addRightEyeModel = new object();
        private static readonly object addHairModel = new object();
        private static readonly object addOtherModel = new object();

        public CharacterCreationOptionMenu NextMenu { get; set; }
        public CharacterCreationAddMenu AddMenu { get; set; }

        protected override Spanning<object> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (!(arg.Other is CharacterCreationDataBuilder builder)) throw new RogueException();

            models.Clear();

            if (builder.Appearances.TryGetBuilder(BoneKeyword.LeftEye, out var leftEyeBuilder))
            {
                models.Add(leftEyeBuilder);
            }
            else
            {
                models.Add(addLeftEyeModel);
            }

            if (builder.Appearances.TryGetBuilder(BoneKeyword.RightEye, out var rightEyeBuilder))
            {
                models.Add(rightEyeBuilder);
            }
            else
            {
                models.Add(addRightEyeModel);
            }

            if (builder.Appearances.TryGetBuilder(BoneKeyword.Hair, out var hairBuilder))
            {
                models.Add(hairBuilder);
            }
            else
            {
                models.Add(addHairModel);
            }

            for (int i = 0; i < builder.Appearances.Count; i++)
            {
                var appearanceBuilder = builder.Appearances[i];
                if (models.Contains(appearanceBuilder)) continue;

                models.Add(appearanceBuilder);
            }
            models.Add(addOtherModel);
            return models;
        }

        protected override string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (model is AppearanceBuilder builder)
            {
                return builder.Name;
            }
            else
            {
                return "+ Œ©‚½–Ú‚ð’Ç‰Á";
            }
        }

        protected override void ItemActivate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (model is AppearanceBuilder builder)
            {
                root.OpenMenu(NextMenu, self, null, new(other: builder), arg);
            }
            else
            {
                root.OpenMenu(AddMenu, self, null, new(other: typeof(AppearanceBuilder)), arg);
            }
        }
    }
}
