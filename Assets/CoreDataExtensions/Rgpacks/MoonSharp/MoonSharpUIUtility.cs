using Lysionium;
using MoonSharp.Interpreter;
using OchalikeSprites;
using Roguegard.Device;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Roguegard.Rgpacks.MoonSharp
{
    public static class MoonSharpUIUtility
    {
        private static readonly StringBuilder stringBuilder = new();
        private static readonly SpeechMenu speechMenu = new();
        private static readonly ChoicesMenu choicesMenu = new();
        private static readonly FadeOutMenu fadeOutMenu = new();

        public static void Say(string text, ScriptExecutionContext executionContext, RogueObj faceObj = null, string facialId = null)
        {
            stringBuilder.Clear();
            var lines = Regex.Matches(text, @"\S.*(\r\n|\r|\n)?");
            foreach (Match match in lines)
            {
                var value = match.Value;
                for (int i = 0; i < match.Length; i++)
                {
                    if (value[i] == '{' && value[i + 1] != '{')
                    {
                        var length = value.IndexOf('}', i) - i;
                        if (value[i + 1] == '>')
                        {
                            stringBuilder.Append("<link=\"HorizontalArrow\"></link>");
                            i += length;
                            continue;
                        }
                        if (value[i + 1] == 'v')
                        {
                            stringBuilder.Append("<link=\"VerticalArrow\"></link><link=\"PageBreak\"></link>");
                            i += length;
                            if (value.Length >= i + 2 && (value[i + 1] == '\r' || value[i + 1] == '\n'))
                            {
                                break;
                            }
                            continue;
                        }
                        if (value[i + 1] == '#')
                        {
                            var id = value.Substring(i + 2, length - 2);
                            var rgpackId = id[..id.IndexOf('.')];
                            var assetId = id[(rgpackId.Length + 1)..];
                            if (!RgpackReference.TryGetRgpack(rgpackId, out var rgpack)) throw new System.InvalidOperationException($"Rgpack ({rgpackId}) が見つかりません。");
                            if (!rgpack.TryGetAsset<object>(assetId, out var asset)) throw new System.InvalidOperationException(
                                $"Rgpack ({rgpackId}) に ID ({assetId}) のデータが見つかりません。");

                            stringBuilder.Append(asset);
                            i += length;
                            continue;
                        }
                        {
                            continue;
                        }
                    }

                    stringBuilder.Append(value[i]);
                }
            }
            if (stringBuilder.Length == 0) { stringBuilder.Append(" "); }

            ISpriteMotion facial = null;
            if (!string.IsNullOrWhiteSpace(facialId)) {
                //var envRgpackId = executionContext.OwnerScript.DoString("return __rgpack").String;
                var envRgpackId = "Playtest";
                var rgpackId = RgpackReference.GetRgpackId(facialId, envRgpackId);
                var assetId = RgpackReference.GetAssetId(facialId);

                if (!RgpackReference.TryGetRgpack(rgpackId, out var rgpack)) throw new System.InvalidOperationException($"Rgpack ({rgpackId}) が見つかりません。");
                if (!rgpack.TryGetAsset<ISpriteMotion>(assetId, out facial)) throw new System.InvalidOperationException(
                    $"Rgpack ({rgpackId}) に ID ({assetId}) のデータが見つかりません。");

                RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateSpriteMotion(faceObj, facial, true));
            }

            // 会話が読まれるまで待機
            speechMenu.coroutine = executionContext.GetCallingCoroutine();
            speechMenu.message = stringBuilder.ToString();
            RogueDevice.Primary.AddMenu(speechMenu, null, null, new(targetObj: faceObj, other: facial));
        }

        public static void Choices(Table selectOptions, ScriptExecutionContext executionContext)
        {
            // 選択肢が選択されるまで待機
            choicesMenu.coroutine = executionContext.GetCallingCoroutine();
            choicesMenu.selectOptions.Clear();
            foreach (var selectOption in selectOptions.Values)
            {
                choicesMenu.selectOptions.Add(selectOption.CastToString());
            }
            RogueDevice.Primary.AddMenu(choicesMenu, null, null, RogueMethodArgument.Identity);
        }

        public static void FadeOut(ScriptExecutionContext executionContext)
        {
            // 選択肢が選択されるまで待機
            fadeOutMenu.coroutine = executionContext.GetCallingCoroutine();
            fadeOutMenu.fadeIn = false;
            RogueDevice.Primary.AddMenu(fadeOutMenu, null, null, RogueMethodArgument.Identity);
        }

        public static void FadeIn(ScriptExecutionContext executionContext)
        {
            // 選択肢が選択されるまで待機
            fadeOutMenu.coroutine = executionContext.GetCallingCoroutine();
            fadeOutMenu.fadeIn = true;
            fadeOutMenu.fadeInAction();
        }

        private class SpeechMenu : RogueMenuScreen
        {
            public global::MoonSharp.Interpreter.Coroutine coroutine;
            public string message;

            public static bool isOpened;

            private ISubviewStateProvider faceStateProvider;

            public override bool IsIncremental => true;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                if (!isOpened)
                {
                    manager.SpeechBox.Clear();
                    isOpened = true;
                }

                manager.SpeechBox.Append(message);
                manager.SpeechBox.Show();

                var showFace = arg.Arg.TargetObj != null;
                if (showFace)
                {
                    manager.Face.Show(null, null, manager, arg, ref faceStateProvider);
                }

                manager.SpeechBox.DoScheduledAfterCompletion((iManager, iArg) =>
                {
                    try
                    {
                        coroutine.Resume();
                    }
                    catch (InterpreterException ex)
                    {
                        Debug.LogError(string.Join("\n", ex.CallStack));
                        throw;
                    }

                    var manager = (MMgr)iManager;
                    if (coroutine.State == CoroutineState.Suspended)
                    {
                        // 次のアニメーションやメニューを受け取るためにいったん閉じる
                        manager.Done();

                        // スピーチボックスと顔グラフィックは表示したままにする
                        manager.ResetDone();
                        manager.SpeechBox.Show();
                        if (showFace)
                        {
                            manager.Face.Show();
                        }
                    }
                    else
                    {
                        // 次のアニメーションやメニューがない場合はスピーチボックスを閉じる

                        // AdvanceText のリセット用に VerticalArrow が余分に必要
                        manager.SpeechBox.Append("<link=\"VerticalArrow\"></link><link=\"VerticalArrow\"></link>");
                        manager.SpeechBox.DoScheduledAfterCompletion((iManager, arg) =>
                        {
                            var manager = (MMgr)iManager;
                            manager.Done();
                            isOpened = false;
                        });
                    }
                });
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                manager.MessageBox.Hide(back);
            }
        }

        private class ChoicesMenu : RogueMenuScreen
        {
            public global::MoonSharp.Interpreter.Coroutine coroutine;
            public List<string> selectOptions = new();
            private static readonly DynValue[] args = new DynValue[1];

            private readonly CommandListMenuViewData<string, MMgr, MArg> view = new()
            {
                SecondaryCommandSubviewSelector = m => m.Choices,
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                view.Show(selectOptions, manager, arg)
                    ?
                    .OnClick((selectOption, manager, arg) =>
                    {
                        manager.Done();
                        manager.SpeechBox.Clear();
                        args[0] = DynValue.NewNumber(selectOptions.IndexOf(selectOption) + 1);
                        try
                        {
                            coroutine.Resume(args);
                        }
                        catch (InterpreterException ex)
                        {
                            Debug.LogError(string.Join("\n", ex.CallStack));
                            throw;
                        }

                        if (coroutine.State == CoroutineState.Dead)
                        {
                            SpeechMenu.isOpened = false;
                        }
                    })

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                if (!back) return;

                view.Hide(manager, back);
            }
        }

        private class FadeOutMenu : RogueMenuScreen
        {
            public global::MoonSharp.Interpreter.Coroutine coroutine;
            public bool fadeIn;
            public System.Action fadeInAction;

            private readonly FadeOutInViewData<MMgr, MArg> view = new()
            {
            };

            public override bool IsIncremental => true;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                if (fadeInAction == null)
                {
                    var actionManager = manager;
                    fadeInAction = () => actionManager.PopMenuScreen();
                }

                view.FadeOut(manager, arg)
                    ?
                    .OnFadeOutCompleted((manager, arg) =>
                    {
                        coroutine.Resume();
                    })

                    .OnFadeInCompleted((manager, arg) =>
                    {
                        if (!fadeIn) return;

                        coroutine.Resume();
                    })

                    .Build();
            }

            public override void CloseScreenView(MMgr manager, bool back)
            {
                view.FadeIn(manager, back);
            }
        }
    }
}
