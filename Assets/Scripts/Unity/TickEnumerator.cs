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
            // 入力情報の更新と同時にターン経過可能かを取得
            var device = RogueDevice.Primary;
            if (!device.UpdateAndGetAllowStepTurn()) return;

            var location = RogueWorldInfo.GetWorld(RogueDevice.Primary.Player);
            //var location = RogueDevice.Primary.Player.Location;
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
            device.AfterStepTurn();
        }

        public static IEnumerator<int> UpdateTurns(
            RogueObj player, RogueObj subject, int maxTurns, int maxIteration, bool untilSavePoint)
        {
            var world = RogueWorldInfo.GetWorld(player);
            var worldInfo = RogueWorldInfo.GetByCharacter(player);
            var item = new Item();
            var iteration = 0;
            for (int turns = 0; turns < maxTurns; turns++)
            {
                for (int i = 0; i < world.Space.Objs.Count; i++) // Update と同じ時間経過順を保つ
                {
                    var location = world.Space.Objs[i];
                    if (location == null) continue;
                    if (untilSavePoint && location == worldInfo.Lobby) continue; // セーブポイントで止める場合、ロビーは空間そのものをセーブポイントとみなす
                    if (location != worldInfo.Lobby && ObjIsIn(player, location)) continue; // ロビー以外の空間にプレイヤーが存在するとき、その空間は時間経過させない
                    if (!LobbyMembersIsIn(player, location)) continue; // ロビーメンバーが存在しない空間は時間経過させない

                    // セーブポイントで止める場合、ロビー以外の空間に被写体が存在するとき、その空間は時間経過させない
                    if (untilSavePoint && location != worldInfo.Lobby && ObjIsIn(subject, location)) continue;

                    while (iteration < maxIteration)
                    {
                        var result = item.MoveNext(location);
                        if (result == Result.Next) break;

                        iteration++;
                        yield return turns;
                    }

                    // プレイヤーが存在する空間の IsTicked を変えるとセーブリセットリロード後の行動順に影響が出るため避ける
                    // world ではなく location をリセットする
                    ResetTick(location);
                }
                iteration++;
                if (iteration >= maxIteration) break;
                yield return turns;

                if (!untilSavePoint)
                {
                    // セーブポイントで止めない場合、デバイスでセーブポイントから復帰させる
                    RogueDevice.Primary.AfterStepTurn();
                }
            }
        }

        private static bool ObjIsIn(RogueObj obj, RogueObj space)
        {
            var objLocation = obj;
            while (objLocation != null)
            {
                if (objLocation == space) return true;

                objLocation = objLocation.Location;
            }
            return false;
        }

        private static bool LobbyMembersIsIn(RogueObj player, RogueObj space)
        {
            var worldInfo = RogueWorldInfo.GetByCharacter(player);
            var lobbyMembers = worldInfo.LobbyMembers.Members;
            for (int j = 0; j < lobbyMembers.Count; j++)
            {
                if (ObjIsIn(lobbyMembers[j], space)) return true;
            }
            return false;
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
                    if (self.Main.IsTicked || LobbyMemberList.GetMemberInfo(self)?.SavePoint != null)
                    {
                        // 行動済みまたはセーブポイントにいるとき行動しない
                        yield return Result.Next;
                        continue;
                    }
                    self.Main.IsTicked = true;

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
                    var updaterState = self.Main.GetRogueObjUpdaterState(self);
                    var aspectState = self.Main.GetRogueMethodAspectState(self);
                    aspectState.RemoveAllNull();

                    // スタックがゼロなら解体する。
                    if (self.Stack == 0) { Destruct(); }

                    // Updater を実行する。
                    while (true)
                    {
                        var continueType = updaterState.UpdateObjAndRemoveNull(self, activationDepth);
                        if (continueType == RogueObjUpdaterContinueType.Break) break;
                        if (RogueDevice.Primary.HasSynchronizedWork)
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
                        var locateMethod = self.Main.InfoSet.Locate;
                        var destructArg = new RogueMethodArgument(targetObj: null);
                        RogueMethodAspectState.Invoke(MainInfoKw.Locate, locateMethod, self, null, 0f, destructArg);
                    }
                }
            }
        }
    }
}
