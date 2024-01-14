using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace RoguegardUnity
{
    public static class StandardRogueDeviceUtility
    {
        private static readonly List<object> formatArgs = new List<object>();
        private static readonly RogueNameBuilder nameBuilder = new RogueNameBuilder();

        public static Color GetColor(RogueObj self, RogueObj obj)
        {
            if (StatsEffectedValues.AreVS(self, obj)) return new Color(1f, .5f, 1f);
            if (RogueParty.Equals(obj, self)) return new Color(.5f, 1f, 1f);
            else return new Color(1f, 1f, .5f);
        }

        public static bool TryLocalize(string text, out string localizedText)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                localizedText = null;
                return false;
            }

            var index = 0;
            while (text[index] == '<')
            {
                var closeIndex = text.IndexOf('>', index);
                if (closeIndex == -1) break;

                index = closeIndex + 1;
            }

            if (text[index] == ':')
            {
                var head = text.Substring(0, index);
                text = text.Substring(index + 1);
                if (!RogueLocalizedStringTable.TryGetEntry(text, out var entry, false))
                {
                    localizedText = null;
                    return false;
                }

                localizedText = head + entry.GetLocalizedString();
                return true;
            }
            else
            {
                localizedText = null;
                return false;
            }
        }

        public static string Localize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Debug.LogWarning("空白のみのテキストをローカライズしようとしました。");
                return text;
            }

            var index = 0;
            while (text[index] == '<')
            {
                var closeIndex = text.IndexOf('>', index);
                if (closeIndex == -1) break;

                index = closeIndex + 1;
            }

            if (text[index] == ':')
            {
                var head = text.Substring(0, index);
                text = text.Substring(index + 1);
                if (!RogueLocalizedStringTable.TryGetEntry(text, out var entry)) return text;

                return head + entry.GetLocalizedString();
            }
            else
            {
                return text;
            }
        }

        public static void Localize(RogueNameBuilder refName)
        {
            for (int i = 0; i < refName.Texts.Count; i++)
            {
                refName.Set(i, Localize(refName.Texts[i]));
            }
        }

        internal static object LocalizeMessage(object other, RogueObj player, MessageWorkQueue messageWorkQueue)
        {
            if (other is RogueObj obj)
            {
                var color = GetColor(player, obj);
                var rgba = ColorUtility.ToHtmlStringRGBA(color);
                obj.GetName(nameBuilder);
                Localize(nameBuilder);
                return $"<color=#{rgba}>{nameBuilder}</color>";
            }
            else if (other is IRogueDescription description)
            {
                return Localize(description.Name);
            }
            else if (other is string text)
            {
                if (messageWorkQueue != null && text.StartsWith(':'))
                {
                    return LocalizeMessage(text, player, messageWorkQueue);
                }
                else
                {
                    return Localize(text);
                }
            }
            else
            {
                return other;
            }
        }

        private static object LocalizeMessage(string text, RogueObj player, MessageWorkQueue messageWorkQueue)
        {
            text = text.Substring(1);
            if (!RogueLocalizedStringTable.TryGetEntry(text, out var entry)) return text;

            formatArgs.Clear();
            var separatorIndex = text.LastIndexOf("::");
            if (separatorIndex >= 0)
            {
                var formatCountText = text.Substring(separatorIndex + 2);
                var formatCount = int.Parse(formatCountText);

                for (int i = 0; i < formatCount; i++)
                {
                    messageWorkQueue.Dequeue(out var other, out var work, out var integer, out var number, out var stackTrace);
                    if (other == DeviceKw.EnqueueInteger)
                    {
                        formatArgs.Add(integer);
                    }
                    else if (other == DeviceKw.EnqueueNumber)
                    {
                        formatArgs.Add(number);
                    }
                    else
                    {
                        formatArgs.Add(LocalizeMessage(other, player, null));
                    }
                }
            }

            return entry.GetLocalizedString(formatArgs);
        }
    }
}
