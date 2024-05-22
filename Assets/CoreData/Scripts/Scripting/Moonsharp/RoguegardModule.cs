using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Text;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Roguegard.Extensions;

namespace Roguegard.Scripting.MoonSharp
{
    [MoonSharpModule(Namespace = moduleName)]
    internal class RoguegardModule
    {
        private const string moduleName = "roguegard";
        private static readonly StringBuilder stringBuilder = new();

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

            // require('roguegard') を必須にする
            globalTable.Remove(moduleName);
        }

        [MoonSharpModuleMethod]
        public static DynValue @ref(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "ref";
            var rgpackID = args.AsType(0, name, DataType.String, false).String;
            var assetID = args.AsType(1, name, DataType.String, false).String;
            if (rgpackID == ".")
            {
                rgpackID = executionContext.CurrentGlobalEnv.Get("__rgpack").String;
                rgpackID = "Playtest";
            }

            if (!RgpackReference.TryGetRgpack(rgpackID, out var rgpack)) throw new RogueException($"Rgpack ({rgpackID}) が見つかりません。");
            if (!rgpack.TryGetAsset<object>(assetID, out var asset)) throw new RogueException(
                $"Rgpack ({rgpackID}) に ID ({assetID}) のデータが見つかりません。");

            if (asset is KyarakuriFigurineInfo characterCreationInfo)
            {
                return UserData.Create(new CharacterCreationAsset(characterCreationInfo));
            }
            if (asset is MysteryDioramaInfo mysteryDioramaInfo)
            {
                return UserData.Create(new MysteryDioramaAsset(mysteryDioramaInfo));
            }
            throw new RogueException();
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
            var diorama = args.AsType(0, name, DataType.UserData, false).CheckUserDataType<MysteryDioramaAsset>(name);
            var player = RogueDevice.Primary.Player;
            diorama.Info.StartDungeon(player, RogueRandom.Primary);
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue msgf(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "msgf";
            var text = args.AsType(0, name, DataType.String, false).String;

            if (text.Length >= "@talk".Length && text.IndexOf("@talk", 0, "@talk".Length, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.StartTalk);
                return DynValue.Nil;
            }
            if (text.Length >= "@endtalk".Length && text.IndexOf("@endtalk", 0, "@endtalk".Length, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                RogueDevice.Add(DeviceKw.AppendText, DeviceKw.EndTalk);
                return DynValue.Nil;
            }

            stringBuilder.Clear();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '{' && text[i + 1] != '{')
                {
                    var length = text.IndexOf('}', i) - i;
                    if (text[i + 1] == '#')
                    {
                        var id = text.Substring(i + 2, length - 2);
                        var rgpackID = "Playtest";
                        if (!RgpackReference.TryGetRgpack(rgpackID, out var rgpack)) throw new RogueException($"Rgpack ({rgpackID}) が見つかりません。");
                        if (!rgpack.TryGetAsset<object>(id, out var asset)) throw new RogueException(
                            $"Rgpack ({rgpackID}) に ID ({id}) のデータが見つかりません。");

                        stringBuilder.Append(asset);
                        i += length;
                        continue;
                    }
                }

                if ((text[i] == '\n' || text[i] == '\r') && (text[i + 1] == '@'))
                {
                    var index = text.IndexOf("starttalk", i + 2, "starttalk".Length, System.StringComparison.OrdinalIgnoreCase);
                    if (index == 0)
                    {

                        continue;
                    }
                }

                stringBuilder.Append(text[i]);
            }
            RogueDevice.Add(DeviceKw.AppendText, stringBuilder.ToString());
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue getChart(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "getChart";
            var chartID = args.AsType(0, name, DataType.String, false).String;

            var rgpackID = "Playtest";
            var chartReference = new RgpackReference(rgpackID, chartID);

            var worldInfo = RogueWorldInfo.GetByCharacter(RogueDevice.Primary.Player);
            worldInfo.ChartState.MoveNext(chartReference);

            RogueDevice.Add(DeviceKw.AppendText, DeviceKw.EndTalk);
            return DynValue.Nil;
        }

        [MoonSharpModuleMethod]
        public static DynValue require_notepad(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "require_notepad";
            var modname = args.AsType(0, name, DataType.String, false).String;
            var script = ((AnonWrapper<MoonSharpRogueScript>)executionContext.CurrentGlobalEnv.Get("__rgenv").UserData.Object).Value;
            var files = executionContext.CurrentGlobalEnv.Get("__files");
            if (files.IsNotNil())
            {
                // 依存関係が指定されているとき、その中からインポートする
                var sources = ((AnonWrapper<Dictionary<string, NotepadQuote>>)files.UserData.Object).Value;
                if (!sources.TryGetValue(modname, out var source)) throw new RogueException($"モジュール {modname} が見つかりません。");

                return script.DoString(source);
            }

            var casterData = executionContext.CurrentGlobalEnv.Get("__caster");
            if (casterData.IsNotNil())
            {
                // プレイヤーが能動的に実行したとき、所持中のメモ帳アイテムからインポートする
                var caster = ((AnonWrapper<RogueObj>)casterData.UserData.Object).Value;
                var objs = caster.Space.Objs;
                for (int i = 0; i < objs.Count; i++)
                {
                    var obj = objs[i];
                    if (obj == null || NotepadInfo.GetQuoteName(obj) != modname) continue;

                    var source = NotepadInfo.GetQuote(obj);
                    return script.DoString(source, caster);
                }
                throw new RogueException($"モジュール {modname} が見つかりません。");
            }

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

            [MoonSharpModuleMethod]
            public static DynValue __new_type(ScriptExecutionContext executionContext, CallbackArguments args)
            {
                const string name = "__new_type";
                var typeName = args.AsType(0, name, DataType.String, false).String;
                var file = ((AnonWrapper<NotepadQuote>)executionContext.CurrentGlobalEnv.Get("__file").UserData.Object).Value;
                var loaded = ((AnonWrapper<List<NotepadQuote>>)executionContext.CurrentGlobalEnv.Get("__loaded").UserData.Object).Value;
                if (loaded.IndexOf(file) == -1) throw new RogueException();

                var type = MoonSharpScriptingType.Get(typeName, loaded.Skip(loaded.IndexOf(file)));
                return UserData.Create(type);
            }

            [MoonSharpModuleMethod]
            public static DynValue __new_event(ScriptExecutionContext executionContext, CallbackArguments args)
            {
                const string name = "__new_event";

                // lua 側のイベントテーブルを取得
                var ev = args.AsType(0, name, DataType.Table, false).Table;

                // イベントの型情報を取得
                var type = (MoonSharpScriptingType)ev.MetaTable.Get("__type").UserData.Object;

                // ローグガルド側のイベントインスタンスを生成してテーブルに設定
                var scriptingEvent = new MoonSharpScriptingCmn(type);
                var scriptingEventWrapper = new AnonWrapper<MoonSharpScriptingCmn>(scriptingEvent);
                var scriptingEventValue = UserData.Create(scriptingEventWrapper);
                ev.Set("__rgins", scriptingEventValue);

                return DynValue.Nil;
            }

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
    }
}
