using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal class TickEnumerator
    {
        private Item item;

        public void Update(int maxIteration)
        {
            // 入力情報を更新
            RogueDevice.Primary.Update();

            var device = RogueDevice.Primary;
            if (device.NextStay) return;

            var location = RogueDevice.Primary.Player.Location;
            if (location != null)
            {
                if (item == null) { item = new Item(); }
                for (int i = 0; i < maxIteration; i++)
                {
                    var result = item.MoveNext(location);
                    if (result == Result.View || location.Stack == 0) break;

                    ResetTick(location);
                }
            }
            device.Next();
        }

        private static void ResetTick(RogueObj self)
        {
            // 自身の更新フラグをリセット
            self.Main.IsTicked = false;



            // 子オブジェクトの処理

            // 空間のオブジェクトの更新フラグリセット処理をする。
            var objs = self.Space.Objs;
            for (int i = 0; i < objs.Count; i++)
            {
                var obj = objs[i];
                if (obj == null) continue;

                ResetTick(obj);
            }
        }

        private enum Result
        {
            Next,
            View
        }

        private class Item
        {
            private IEnumerator<Result> enumerator;

            private RogueObj self;

            private Item child;

            public Item()
            {
                enumerator = GetEnumerator();
            }

            public Result MoveNext(RogueObj obj)
            {
                self = obj;
                var next = enumerator.MoveNext();
                if (!next)
                {
                    // 例外など何らかの理由で停止したとき、最初から再試行する
                    enumerator = GetEnumerator();
                }

                var result = enumerator.Current;
                return result;
            }

            private IEnumerator<Result> GetEnumerator()
            {
                child = new Item();
                var activationDepth = 0f;

                while (true)
                {
                    var status = self.Main;
                    if (status == null || status.IsTicked)
                    {
                        yield return Result.Next;
                        continue;
                    }
                    status.IsTicked = true;

                    // 子オブジェクトの処理
                    // 子オブジェクトの動作には親オブジェクトが IsTicked == false であることが必要。
                    // （そうしたほうがダンジョンのフロアを生成したとき全オブジェクトを同時に行動開始させるのに便利）

                    // 無限再帰対策として、子オブジェクトは親オブジェクトより先に処理する。

                    // 空間のオブジェクトの時間経過処理をする。
                    self.Space.RemoveAllNull();
                    var objsCount = self.Space.Objs.Count; // 無限再帰対策として、オブジェクト数を固定しておく。
                    for (int i = 0; i < objsCount; i++)
                    {
                        if (i >= self.Space.Objs.Count)
                        {
                            break;
                        }

                        var obj = self.Space.Objs[i];
                        if (obj == null) continue;

                        while (true)
                        {
                            var result = child.MoveNext(obj);
                            if (result == Result.Next) break;

                            yield return result;
                        }
                    }
                    self.Space.RemoveAllNull();



                    // 行動前にオブジェクトエフェクトを更新する。
                    var state = self.Main;
                    var updaterState = state.GetRogueObjUpdaterState(self);
                    var aspectState = state.GetRogueMethodAspectState(self);
                    aspectState.RemoveAllNull();

                    // スタックがゼロなら解体する。
                    if (self.Stack == 0) { Destruct(); }

                    // Updater を実行する。
                    while (true)
                    {
                        var continueType = updaterState.UpdateObjAndRemoveNull(self, activationDepth);
                        if (continueType == RogueObjUpdaterContinueType.Break) break;
                        if (RogueDevice.Primary.CalledSynchronizedView)
                        {
                            yield return Result.View;
                        }
                    }
                    aspectState.RemoveAllNull();

                    // スタックがゼロなら解体する。
                    if (self.Stack == 0) { Destruct(); }



                    yield return Result.Next;

                    void Destruct()
                    {
                        var locateMethod = status.InfoSet.Locate;
                        var destructArg = new RogueMethodArgument(targetObj: null);
                        RogueMethodAspectState.Invoke(MainInfoKw.Locate, locateMethod, self, null, 0f, destructArg);
                    }
                }
            }
        }
    }
}
