using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    public class AppearanceBuildersMenu : IModelsMenu
    {
        private readonly ItemController itemController;
        private readonly List<object> models = new List<object>();
        private static readonly object addLeftEyeModel = new object();
        private static readonly object addRightEyeModel = new object();
        private static readonly object addHairModel = new object();

        public CharacterCreationOptionMenu NextMenu { get; set; }
        public CharacterCreationAddMenu AddMenu { get; set; }

        public AppearanceBuildersMenu()
        {
            itemController = new ItemController() { parent = this };
        }

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (!(arg.Other is CharacterCreationDataBuilder builder)) throw new RogueException();

            models.Clear();

            if (builder.Appearances.TryGetBuilder(BoneKw.LeftEye, out var leftEyeBuilder))
            {
                models.Add(leftEyeBuilder);
            }
            else
            {
                models.Add(addLeftEyeModel);
            }

            if (builder.Appearances.TryGetBuilder(BoneKw.RightEye, out var rightEyeBuilder))
            {
                models.Add(rightEyeBuilder);
            }
            else
            {
                models.Add(addRightEyeModel);
            }

            if (builder.Appearances.TryGetBuilder(BoneKw.Hair, out var hairBuilder))
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
            models.Add(null);

            var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
            scroll.OpenView(itemController, models, root, self, null, arg);
            scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
        }

        private class ItemController : IModelsMenuItemController
        {
            public AppearanceBuildersMenu parent;

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
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

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is AppearanceBuilder builder)
                {
                    root.OpenMenu(parent.NextMenu, self, null, new(other: builder), arg);
                }
                else
                {
                    root.OpenMenu(parent.AddMenu, self, null, new(other: typeof(AppearanceBuilder)), arg);
                }
            }
        }
    }
}
