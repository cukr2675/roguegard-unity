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

        private CharacterCreationDataBuilder builder;

        public AppearanceBuildersMenu()
        {
            itemController = new ItemController(this);
        }

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (!(arg.Other is CharacterCreationDataBuilder builder)) throw new RogueException();

            this.builder = builder;
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

            var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
            scroll.OpenView(itemController, models, root, self, null, arg);
            scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
        }

        private class ItemController : IModelsMenuItemController
        {
            private readonly BuilderMenu nextMenu;

            public ItemController(AppearanceBuildersMenu parent)
            {
                nextMenu = new BuilderMenu(parent);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is AppearanceBuilder builder)
                {
                    return builder.Name;
                }
                else
                {
                    return null;
                }
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is AppearanceBuilder builder)
                {
                    root.OpenMenu(nextMenu, self, null, new(other: builder), arg);
                }
                else
                {
                }
            }
        }

        private class BuilderMenu : IModelsMenu
        {
            private readonly object[] models;

            public BuilderMenu(AppearanceBuildersMenu parent)
            {
                models = new object[]
                {
                    new SelectOptionChoice(),
                    new RSlider(),
                    new GSlider(),
                    new BSlider(),
                    new ASlider(),
                    new RemoveChoice() { parent = parent }
                };
            }

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var builder = (AppearanceBuilder)arg.Other;
                var options = (OptionsMenuView)root.Get(DeviceKw.MenuOptions);
                var useModels = builder.Option.MemberSources.Contains(SingleItemMember.SourceInstance) ? models : models;
                options.OpenView(ChoicesModelsMenuItemController.Instance, useModels, root, self, null, new(other: builder));
                options.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            private class SelectOptionChoice : IModelsMenuChoice
            {
                private readonly SelectOptionMenu nextMenu = new SelectOptionMenu();

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var builder = (AppearanceBuilder)arg.Other;
                    return builder.Option.Name;
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    var builder = (AppearanceBuilder)arg.Other;
                    root.OpenMenu(nextMenu, self, null, new(other: builder), arg);
                }
            }

            private class RemoveChoice : IModelsMenuChoice
            {
                public AppearanceBuildersMenu parent;

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "<#f00>çÌèú";
                }

                public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    parent.builder.Appearances.Remove((AppearanceBuilder)arg.Other);
                    root.Back();
                }
            }

            private class RSlider : IModelsMenuOptionSlider
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "R";
                }

                public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var builder = (AppearanceBuilder)arg.Other;
                    return builder.Color.r / 255f;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
                {
                    var builder = (AppearanceBuilder)arg.Other;
                    var color = builder.Color;
                    color.r = (byte)(value * 255f);
                    builder.Color = color;
                }
            }

            private class GSlider : IModelsMenuOptionSlider
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "G";
                }

                public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var builder = (AppearanceBuilder)arg.Other;
                    return builder.Color.g / 255f;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
                {
                    var builder = (AppearanceBuilder)arg.Other;
                    var color = builder.Color;
                    color.g = (byte)(value * 255f);
                    builder.Color = color;
                }
            }

            private class BSlider : IModelsMenuOptionSlider
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "B";
                }

                public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var builder = (AppearanceBuilder)arg.Other;
                    return builder.Color.b / 255f;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
                {
                    var builder = (AppearanceBuilder)arg.Other;
                    var color = builder.Color;
                    color.b = (byte)(value * 255f);
                    builder.Color = color;
                }
            }

            private class ASlider : IModelsMenuOptionSlider
            {
                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    return "ïsìßñæìx";
                }

                public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var builder = (AppearanceBuilder)arg.Other;
                    return builder.Color.a / 255f;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
                {
                    var builder = (AppearanceBuilder)arg.Other;
                    var color = builder.Color;
                    color.a = (byte)(value * 255f);
                    builder.Color = color;
                }
            }
        }

        private class SelectOptionMenu : IModelsMenu, IModelsMenuItemController
        {
            private readonly List<object> models = new List<object>();

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var builder = (AppearanceBuilder)arg.Other;
                models.Clear();
                for (int i = 0; i < RoguegardSettings.CharacterCreationDatabase.AppearanceOptions.Count; i++)
                {
                    var appearanceOption = RoguegardSettings.CharacterCreationDatabase.AppearanceOptions[i];
                    if (appearanceOption.BoneName == builder.Option.BoneName)
                    {
                        models.Add(appearanceOption);
                    }
                }

                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, models, root, self, null, new(other: builder));
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ((IAppearanceOption)model).Name;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var builder = (AppearanceBuilder)arg.Other;
                builder.Option = (IAppearanceOption)model;

                root.Back();
            }
        }
    }
}
