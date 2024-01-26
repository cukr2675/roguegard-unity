using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    internal class PaintPaletteMenu : IModelsMenu, IModelsMenuItemController
    {
        private static readonly object[] models = Enumerable.Range(0, 16).Select(x => new Choice() { colorIndex = x }).ToArray();
        private static readonly NextMenu nextMenu = new NextMenu();

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var data = (SewedEquipmentData)arg.Other;
            var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
            scroll.OpenView(this, models, root, null, null, new(other: data));
            scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
        }

        public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var choice = (Choice)model;
            return $"カラー{choice.colorIndex}";
        }

        public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

            var data = (SewedEquipmentData)arg.Other;
            var choice = (Choice)model;
            root.OpenMenu(nextMenu, null, null, new(other: data, count: choice.colorIndex), arg);
        }

        private class Choice : IModelsMenuIcon
        {
            public int colorIndex;

            public void GetIcon(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, out Sprite sprite, out Color color)
            {
                var data = (SewedEquipmentData)arg.Other;
                sprite = data.Palette[colorIndex].ToIcon();
                color = data.MainColor;
            }
        }

        private class NextMenu : IModelsMenu
        {
            private static readonly object[] models = new object[]
            {
                new RSlider(),
                new GSlider(),
                new BSlider(),
                new ASlider(),
                new Shift()
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var options = (IScrollModelsMenuView)root.Get(DeviceKw.MenuOptions);
                options.OpenView(ChoicesModelsMenuItemController.Instance, models, root, null, null, arg);
                options.ShowExitButton(ExitModelsMenuChoice.Instance);
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
                var data = (SewedEquipmentData)arg.Other;
                return data.Palette[arg.Count].ROrShade / 255f;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                var data = (SewedEquipmentData)arg.Other;
                var color = data.Palette[arg.Count];
                color.ROrShade = (byte)(value * 255f);
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
                var data = (SewedEquipmentData)arg.Other;
                return data.Palette[arg.Count].GOrSaturation / 255f;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                var data = (SewedEquipmentData)arg.Other;
                var color = data.Palette[arg.Count];
                color.GOrSaturation = (byte)(value * 255f);
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
                var data = (SewedEquipmentData)arg.Other;
                return data.Palette[arg.Count].BOrValue / 255f;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                var data = (SewedEquipmentData)arg.Other;
                var color = data.Palette[arg.Count];
                color.BOrValue = (byte)(value * 255f);
            }
        }

        private class ASlider : IModelsMenuOptionSlider
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "不透明度";
            }

            public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var data = (SewedEquipmentData)arg.Other;
                return data.Palette[arg.Count].A;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                var data = (SewedEquipmentData)arg.Other;
                var color = data.Palette[arg.Count];
                color.A = value;
            }
        }

        private class Shift : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var data = (SewedEquipmentData)arg.Other;
                var color = data.Palette[arg.Count];
                return $"着色: {color.IsShift}";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var data = (SewedEquipmentData)arg.Other;
                var color = data.Palette[arg.Count];
                color.IsShift = !color.IsShift;
            }
        }
    }
}
