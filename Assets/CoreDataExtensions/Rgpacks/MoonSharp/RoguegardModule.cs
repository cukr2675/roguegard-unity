using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Roguegard.Extensions;
using Roguegard.Device;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpModule(Namespace = moduleName)]
    internal class RoguegardModule
    {
        private const string moduleName = "roguegard";
        private static readonly StringBuilder stringBuilder = new();
        private static readonly SelectMenu selectMenu = new();

        public static void MoonSharpInit(Table globalTable, Table roguegardTable)
        {
            globalTable.RegisterModuleType<Internal>();

            roguegardTable.Set("implements_event", roguegardTable.OwnerScript.DoString(@"
return function(type_name)
    type = {}
    type.__type = __rg.__new_type(type_name)

    function type:new (o)
        o = o or {}
        setmetatable(o, self)
        self.__index = self
        __rg.__new_event(o)
        return o
    end

    return type
end
"));

            // エフェクトクラスのテンプレートを生成する関数
            // クラスを継承する感覚で実装させる
            roguegardTable.Set("implements_effect", roguegardTable.OwnerScript.DoString(@"
return function(type_name)
    type = {}
    type.__type = __rg.__new_type(type_name)

    function type:new (o)
        o = o or {}
        setmetatable(o, self)
        self.__index = self
        __rg.__new_effect(o)
        return o
    end

    return type
end
"));

            roguegardTable.Set("select", roguegardTable.OwnerScript.DoString(@"
return function(a, b, c)
    rg.__select(a, b, c)
    return coroutine.yield()
end
"));

            // エフェクトクラスのテンプレートを生成する関数
            // クラスを継承する感覚で実装させる
            roguegardTable.Set("Cmn", roguegardTable.OwnerScript.DoString(@"
return {
    ['new'] = function(self)
        local o = o or {}
        setmetatable(o, self)
        self.__index = self
        return o
    end,

    ['__type'] = 'Cmn'
}
"));

            // require('roguegard') を必須にする
            globalTable.Remove(moduleName);
        }

        [MoonSharpModuleMethod]
        public static DynValue @ref(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "ref";
            var id = args.AsType(0, name, DataType.String, false).String;
            var envRgpackID = executionContext.CurrentGlobalEnv.Get("__rgpack").String;
            envRgpackID = "Playtest";
            var rgpackID = RgpackReference.GetRgpackID(id, envRgpackID);
            var assetID = RgpackReference.GetAssetID(id);

            if (!RgpackReference.TryGetRgpack(rgpackID, out var rgpack)) throw new RogueException($"Rgpack ({rgpackID}) が見つかりません。");
            if (!rgpack.TryGetAsset<object>(assetID, out var asset)) throw new RogueException(
                $"Rgpack ({rgpackID}) に ID ({assetID}) のデータが見つかりません。");

            if (asset is CharacterCreationPresetAsset characterCreationPresetAsset)
            {
                return UserData.Create(new CharacterCreationPresetUserData(characterCreationPresetAsset));
            }
            if (asset is KyarakuriClayAsset kyarakuriClayAsset)
            {
                return UserData.Create(new KyarakuriClayUserData(kyarakuriClayAsset.Reference));
            }
            if (asset is MysteryDioramaAsset mysteryDioramaAsset)
            {
                return UserData.Create(new MysteryDioramaUserData(mysteryDioramaAsset));
            }
            if (asset is ChartPadAsset chartPadAsset)
            {
                return UserData.Create(new RogueChartUserData(chartPadAsset.ChartSource));
            }
            throw new RogueException();
        }

        [MoonSharpModuleMethod]
        public static DynValue find(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "find";
            var evtID = args.AsType(0, name, DataType.String, false).String;

            var location = RogueDevice.Primary.Player.Location;
            var locationObjs = location.Space.Objs;
            for (int i = 0; i < locationObjs.Count; i++)
            {
                var obj = locationObjs[i];
                if (obj == null || !(obj.Main.InfoSet is EvtFairyReference infoSet) || infoSet.EvtID != evtID) continue;

                return UserData.Create(new RogueObjUserData(obj));
            }
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue changePlayer(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "changePlayer";
            var newPlayer = args.AsType(0, name, DataType.UserData, false).CheckUserDataType<AnonWrapper<RogueObj>>(name).Value;

            // 空間移動
            var self = RogueDevice.Primary.Player;
            var worldInfo = RogueWorldInfo.GetByCharacter(self);
            var location = self.Location;
            var position = self.Position;
            SpaceUtility.TryLocate(self, null);
            SpaceUtility.TryLocate(newPlayer, location, position);
            newPlayer.Main.Stats.Direction = RogueDirection.Down;

            worldInfo.LobbyMembers.Add(newPlayer);

            // パーティ移動
            var party = self.Main.Stats.Party;
            self.Main.Stats.UnassignParty(self, party);
            newPlayer.Main.Stats.TryAssignParty(newPlayer, party);

            RogueDevice.Add(DeviceKw.ChangePlayer, newPlayer);
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue locateDiorama(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "locateDiorama";
            var diorama = args.AsType(0, name, DataType.UserData, false).CheckUserDataType<MysteryDioramaUserData>(name);
            var player = RogueDevice.Primary.Player;
            diorama.Asset.StartDungeon(player, RogueRandom.Primary);
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue talkf(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "talkf";
            var text = args.AsType(0, name, DataType.String, false).String;
            var lines = Regex.Matches(text, @"\S.*(\r\n|\r|\n)?");

            stringBuilder.Clear();
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
                            stringBuilder.Append('\t');
                            i += length;
                            continue;
                        }
                        if (value[i + 1] == 'v')
                        {
                            stringBuilder.Append('\v');
                            i += length;
                            if (value[i + 1] == '\r' || value[i + 1] == '\n') break;
                            continue;
                        }
                        if (value[i + 1] == '#')
                        {
                            var id = value.Substring(i + 2, length - 2);
                            var rgpackID = "Playtest";
                            if (!RgpackReference.TryGetRgpack(rgpackID, out var rgpack)) throw new RogueException($"Rgpack ({rgpackID}) が見つかりません。");
                            if (!rgpack.TryGetAsset<object>(id, out var asset)) throw new RogueException(
                                $"Rgpack ({rgpackID}) に ID ({id}) のデータが見つかりません。");

                            stringBuilder.Append(asset);
                            i += length;
                            continue;
                        }
                    }

                    stringBuilder.Append(value[i]);
                }
            }
            if (stringBuilder.Length == 0) { stringBuilder.Append(" "); }
            RogueDevice.Add(DeviceKw.AppendText, DeviceKw.StartTalk);
            RogueDevice.Add(DeviceKw.AppendText, stringBuilder.ToString());
            RogueDevice.Add(DeviceKw.AppendText, DeviceKw.EndTalk);
            //RogueDevice.Add(DeviceKw.AppendText, DeviceKw.WaitEndOfTalk);
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue __select(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "__select";
            selectMenu.coroutine = executionContext.GetCallingCoroutine();
            selectMenu.choices.Clear();
            for (int i = 0; i < args.Count; i++)
            {
                var choice = args.AsType(i, name, DataType.String, false).String;
                selectMenu.choices.Add(choice);
            }
            RogueDevice.Primary.AddMenu(selectMenu, null, null, RogueMethodArgument.Identity);
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue numberField(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            return UserData.Create(new NumberCmnPropertyUserData());
        }

        [MoonSharpModuleMethod]
        public static DynValue require_notepad(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            //const string name = "require_notepad";
            //var modname = args.AsType(0, name, DataType.String, false).String;
            //var script = ((AnonWrapper<MoonSharpRogueScript>)executionContext.CurrentGlobalEnv.Get("__rgenv").UserData.Object).Value;
            //var files = executionContext.CurrentGlobalEnv.Get("__files");
            //if (files.IsNotNil())
            //{
            //    // 依存関係が指定されているとき、その中からインポートする
            //    var sources = ((AnonWrapper<Dictionary<string, NotepadQuote>>)files.UserData.Object).Value;
            //    if (!sources.TryGetValue(modname, out var source)) throw new RogueException($"モジュール {modname} が見つかりません。");

            //    return script.DoString(source);
            //}

            //var casterData = executionContext.CurrentGlobalEnv.Get("__caster");
            //if (casterData.IsNotNil())
            //{
            //    // プレイヤーが能動的に実行したとき、所持中のメモ帳アイテムからインポートする
            //    var caster = ((AnonWrapper<RogueObj>)casterData.UserData.Object).Value;
            //    var objs = caster.Space.Objs;
            //    for (int i = 0; i < objs.Count; i++)
            //    {
            //        var obj = objs[i];
            //        if (obj == null || NotepadInfo.GetQuoteName(obj) != modname) continue;

            //        var source = NotepadInfo.GetQuote(obj);
            //        return script.DoString(source, caster);
            //    }
            //    throw new RogueException($"モジュール {modname} が見つかりません。");
            //}

            throw new RogueException("モジュールの参照先が見つかりません。");
        }

        /// <summary>
        /// 指定の <see cref="RogueObj"/> に指定のエフェクトを付与
        /// </summary>
        [MoonSharpModuleMethod]
        public static DynValue add_open(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "add_open";
            var obj = args.AsUserData<AnonWrapper<RogueObj>>(0, name, false).Value;
            var effect = args.AsType(1, name, DataType.Table, false).Table.Get("__rgins").CheckUserDataType<AnonWrapper<MoonSharpRogueEffect>>(name).Value;
            obj.Main.RogueEffects.AddOpen(obj, effect);
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue open_value(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            var value = AffectableValue.Get();
            return UserData.Create(value);
        }

        [MoonSharpModuleMethod]
        public static DynValue close_value(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "close_value";
            var value = args.AsUserData<AnonWrapper<AffectableValue>>(0, name, false).Value;
            value.Dispose();
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue affect_value(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "affect_value";
            var keyword = args.AsUserData<AnonWrapper<IKeyword>>(0, name, false).Value;
            var value = args.AsUserData<AnonWrapper<AffectableValue>>(1, name, false).Value;
            var obj = args.AsUserData<AnonWrapper<RogueObj>>(2, name, false).Value;
            ValueEffectState.AffectValue(keyword, value, obj);
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue hurt(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "hurt";
            var target = args.AsUserData<AnonWrapper<RogueObj>>(0, name, false).Value;
            var user = args.AsUserData<AnonWrapper<RogueObj>>(1, name, false).Value;
            var activationDepth = (float)args.AsType(2, name, DataType.Number, false).Number;
            var value = args.AsUserData<AnonWrapper<AffectableValue>>(3, name, false).Value;
            default(IActiveRogueMethodCaller).Hurt(target, user, activationDepth, value);
            return DynValue.Nil;
        }

        [MoonSharpModule(Namespace = moduleName)]
        public class Internal
        {
            private const string moduleName = "__rg";

            //[MoonSharpModuleMethod]
            //public static DynValue __new_type(ScriptExecutionContext executionContext, CallbackArguments args)
            //{
            //    const string name = "__new_type";
            //    var typeName = args.AsType(0, name, DataType.String, false).String;
            //    var file = ((AnonWrapper<NotepadQuote>)executionContext.CurrentGlobalEnv.Get("__file").UserData.Object).Value;
            //    var loaded = ((AnonWrapper<List<NotepadQuote>>)executionContext.CurrentGlobalEnv.Get("__loaded").UserData.Object).Value;
            //    if (loaded.IndexOf(file) == -1) throw new RogueException();

            //    var type = MoonSharpScriptingType.Get(typeName, loaded.Skip(loaded.IndexOf(file)));
            //    return UserData.Create(type);
            //}

            //[MoonSharpModuleMethod]
            //public static DynValue __new_event(ScriptExecutionContext executionContext, CallbackArguments args)
            //{
            //    const string name = "__new_event";

            //    // lua 側のイベントテーブルを取得
            //    var ev = args.AsType(0, name, DataType.Table, false).Table;

            //    // イベントの型情報を取得
            //    var type = (MoonSharpScriptingType)ev.MetaTable.Get("__type").UserData.Object;

            //    // ローグガルド側のイベントインスタンスを生成してテーブルに設定
            //    var scriptingEvent = new MoonSharpScriptingCmn(type);
            //    var scriptingEventWrapper = new AnonWrapper<MoonSharpScriptingCmn>(scriptingEvent);
            //    var scriptingEventValue = UserData.Create(scriptingEventWrapper);
            //    ev.Set("__rgins", scriptingEventValue);

            //    return DynValue.Nil;
            //}

            [MoonSharpModuleMethod]
            public static DynValue __new_effect(ScriptExecutionContext executionContext, CallbackArguments args)
            {
                const string name = "__new_effect";

                // lua 側のエフェクトテーブルを取得
                var effect = args.AsType(0, name, DataType.Table, false).Table;

                // エフェクトの型情報を取得
                var type = (MoonSharpScriptingType)effect.MetaTable.Get("__type").UserData.Object;

                // ローグガルド側のエフェクトインスタンスを生成してテーブルに設定
                var rogueEffect = new MoonSharpRogueEffect(type);
                var rogueEffectWrapper = new AnonWrapper<MoonSharpRogueEffect>(rogueEffect);
                var rogueEffectValue = UserData.Create(rogueEffectWrapper);
                effect.Set("__rgins", rogueEffectValue);

                return DynValue.Nil;
            }
        }

        private class SelectMenu : IModelsMenu, IModelListPresenter
        {
            public global::MoonSharp.Interpreter.Coroutine coroutine;
            public List<string> choices = new();
            private static readonly DynValue[] args = new DynValue[1];

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var view = root.Get(DeviceKw.MenuTalkChoices);
                view.OpenView<string>(this, choices, root, self, user, arg);
            }

            public string GetItemName(object modelListItem, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return (string)modelListItem;
            }

            public void ActivateItem(object modelListItem, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.Done();
                args[0] = DynValue.NewNumber(choices.IndexOf((string)modelListItem) + 1);
                coroutine.Resume(args);
            }
        }
    }
}
