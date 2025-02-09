using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;
using OchalikeSprites;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    public class RogueObjUserData
    {
        public RogueObj Obj { get; }

        public string evtId
        {
            get
            {
                if (!(Obj.Main.InfoSet is EvtFairyReference evt)) return null;

                return evt.AssetID;
            }
        }

        public int x => Obj.Position.x;
        public int y => Obj.Position.y;

        public RogueObjUserData(RogueObj obj, ScriptExecutionContext executionContext = null)
        {
            Obj = obj;

            if (executionContext != null)
            {
                _says = executionContext.OwnerScript.DoString(@"
                    return function(self, ...)
                        if type(self) ~= 'userdata' then __rg.__errorSelfIsNil() end
                        self.__says(...)
                        return coroutine.yield()
                    end
");
            }
        }

        public void walkUP(int steps) => walk(RogueDirection.Up, steps);
        public void walkDN(int steps) => walk(RogueDirection.Down, steps);
        public void walkRT(int steps) => walk(RogueDirection.Right, steps);
        public void walkLT(int steps) => walk(RogueDirection.Left, steps);
        public void walkUR(int steps) => walk(RogueDirection.UpperRight, steps);
        public void walkDR(int steps) => walk(RogueDirection.LowerRight, steps);
        public void walkUL(int steps) => walk(RogueDirection.UpperLeft, steps);
        public void walkDL(int steps) => walk(RogueDirection.LowerLeft, steps);

        private void walk(RogueDirection direction, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                default(IActiveRogueMethodCaller).Walk(Obj, direction, 1f);
            }
        }

        public void moveTo(int x, int y, RogueDirectionUserData direction)
        {
            //SpaceUtility.TryLocate(Obj, new Vector2Int(x, y));
            var movement = MovementCalculator.Get(Obj);
            if (Obj.TryLocate(new Vector2Int(x, y), movement.AsTile, false, false, movement.HasSightCollider))
            {
                Obj.Main.Stats.Direction = direction.Direction;
            }
        }

        public void addEffect(string id)
        {
            //var envRgpackID = executionContext.OwnerScript.DoString("return __rgpack").String;
            var envRgpackID = "Playtest";
            var rgpackID = RgpackReference.GetRgpackID(id, envRgpackID);
            var assetID = RgpackReference.GetAssetID(id);

            if (!RgpackReference.TryGetRgpack(rgpackID, out var rgpack)) throw new RogueException($"Rgpack ({rgpackID}) が見つかりません。");
            if (!rgpack.TryGetAsset<EffectStickerAsset>(assetID, out var asset)) throw new RogueException(
                $"Rgpack ({rgpackID}) に ID ({assetID}) のデータが見つかりません。");

            Obj.Main.RogueEffects.AddOpen(Obj, new EffectStickerReference(id, rgpackID));
        }

        private readonly DynValue _says;

        public DynValue says => _says;

        public void __says(ScriptExecutionContext executionContext, CallbackArguments args)
        {
            const string name = "says";
            if (args.Count == 1)
            {
                var text = args.AsType(0, name, DataType.String, false).String;
                MoonSharpUIUtility.Say(text, executionContext, Obj);
            }
            else if (args.Count == 2)
            {
                var facialId = args.AsType(0, name, DataType.String, false).String;
                var text = args.AsType(1, name, DataType.String, false).String;
                MoonSharpUIUtility.Say(text, executionContext, Obj, facialId);
            }
            else
            {
                throw new System.ArgumentException();
            }
        }

        public void setMotion(string id)
        {
            //var envRgpackID = executionContext.OwnerScript.DoString("return __rgpack").String;
            var envRgpackID = "Playtest";
            var rgpackID = RgpackReference.GetRgpackID(id, envRgpackID);
            var assetID = RgpackReference.GetAssetID(id);

            if (!RgpackReference.TryGetRgpack(rgpackID, out var rgpack)) throw new RogueException($"Rgpack ({rgpackID}) が見つかりません。");
            if (!rgpack.TryGetAsset<ISpriteMotion>(assetID, out var asset)) throw new RogueException(
                $"Rgpack ({rgpackID}) に ID ({assetID}) のデータが見つかりません。");

            RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateSpriteMotion(Obj, asset, true));
        }

        public override string ToString()
        {
            return Obj.GetName();
        }

        [MoonSharpUserDataMetamethod("__concat")] public static string Concat(RogueObjUserData o, string v) => o.ToString() + v;
        [MoonSharpUserDataMetamethod("__concat")] public static string Concat(string v, RogueObjUserData o) => o.ToString() + v;
        [MoonSharpUserDataMetamethod("__concat")] public static string Concat(RogueObjUserData o1, RogueObjUserData o2) => o1.ToString() + o2.ToString();
    }
}
