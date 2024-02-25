using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Roguegard.Extensions;

namespace Roguegard.Scripting.MoonSharp
{
    [MoonSharpModule(Namespace = moduleName)]
    internal class RoguegardModule
    {
        private const string moduleName = "roguegard";

        public static void MoonSharpInit(Table globalTable, Table roguegardTable)
        {
            globalTable.RegisterModuleType<Internal>();

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
