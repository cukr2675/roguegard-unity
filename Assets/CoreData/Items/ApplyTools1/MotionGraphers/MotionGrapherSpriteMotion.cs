using OchalikeSprites;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Roguegard
{
    public class MotionGrapherSpriteMotion : ISpriteMotion
    {
        private readonly MotionGrapherInfo info;
        private List<Pose> poses;

        public MotionGrapherSpriteMotion(MotionGrapherInfo info)
        {
            this.info = info;
        }

        private void Initialize()
        {
            // このモーションの同期再生部分のキータイム一覧を取得する（SubTimelineTrack などの非同期再生部分は除く）
            var keyTimes = new List<float>();
            foreach (var track in info.Tracks)
            {
                if (track is SpriteMotionGrapherTrack spriteMotionTrack)
                {
                    keyTimes.AddRange(spriteMotionTrack.SelectKeyTimes());
                }
            }
            keyTimes.Sort();

            poses = new List<Pose>();
            Pose beforePose = null;
            var boneReorderTable = new Dictionary<BoneKeyword, float>();
            foreach (var keyTime in keyTimes)
            {
                var pose = new Pose(info, keyTime, beforePose, boneReorderTable);
                poses.Add(pose);
                beforePose = pose;
            }
        }

        public void ApplyTo(int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion)
        {
            if (poses == null) { Initialize(); }

            // 再生速度適用
            var floatTime = animationTime * info.PlaybackSpeed / 60f;
            var asynchronousFloatTime = floatTime;

            // ループ回数適用
            if (poses.Count >= 2 && poses[^1].KeyTime != 0f)
            {
                var loopIndex = floatTime / poses[^1].KeyTime;
                if (info.LoopCount >= 1 && loopIndex >= info.LoopCount)
                {
                    // ループ終了したら最後のポーズで停止
                    floatTime = poses[^1].KeyTime;
                }
                else
                {
                    // ループ終了していない場合のループ処理
                    floatTime = Mathf.Repeat(floatTime, poses[^1].KeyTime);
                }
            }

            // 再生時間に対応するポーズを取得
            var index = poses.Count;
            for (int i = 1; i < poses.Count; i++)
            {
                var pose = poses[i];
                if (pose.KeyTime > floatTime)
                {
                    index = i;
                    break;
                }
            }

            {
                var pose = poses[index - 1];
                pose.AsynchronousKeyTime = asynchronousFloatTime;
                var degreeRotation = Quaternion.Euler(0f, 0f, direction.Degree);
                transform.Position = (degreeRotation * Vector3.right) / RoguegardSettings.PixelsPerUnit;
                transform.Direction = direction;
                transform.PoseSource = pose;
                endOfMotion = false;
            }
        }

        private class Pose : IDirectionalSpritePoseSource
        {
            public float KeyTime { get; }

            public float AsynchronousKeyTime { get; set; }

            private readonly SpritePose baseSpritePose;
            private readonly ImmutableSymmetricalSpritePoseSource spritePoseSource;

            private readonly List<ISpriteMotion> subSpriteMotions;
            private readonly SpritePose resultSpritePose;
            private readonly List<BoneReorder> resultBoneReorders;
            private readonly Dictionary<BoneKeyword, int> resultBoneReorderIndexTable;

            private static readonly StaticInitializable<Stack<MotionGrapherInfo>> recursionStack = new(() => new Stack<MotionGrapherInfo>());

            public Pose(MotionGrapherInfo info, float keyTime, Pose beforePose, Dictionary<BoneKeyword, float> boneReorderTable)
            {
                if (recursionStack.Value.Contains(info)) throw new RogueException($"無限再帰が発生しました。\n{string.Join(" -> ", recursionStack.Value)}");
                recursionStack.Value.Push(info);

                KeyTime = keyTime;
                subSpriteMotions = new List<ISpriteMotion>();
                resultSpritePose = new SpritePose();
                resultBoneReorders = new List<BoneReorder>();
                resultBoneReorderIndexTable = new Dictionary<BoneKeyword, int>();

                baseSpritePose = new SpritePose();
                foreach (var track in info.Tracks)
                {
                    if (track is SpriteMotionGrapherTrack spriteMotionTrack)
                    {
                        foreach (var bone in spriteMotionTrack.Bones)
                        {
                            var boneName = new BoneKeyword(bone.BoneName);
                            var beforeTransform = beforePose?.baseSpritePose.BoneTransforms[boneName];
                            var positionX = beforeTransform?.LocalPosition.x ?? 0f;
                            var positionY = beforeTransform?.LocalPosition.y ?? 0f;
                            var positionZ = beforeTransform?.LocalPosition.z ?? 0f;
                            var rotationX = beforeTransform?.LocalRotation.eulerAngles.x ?? 0f;
                            var rotationY = beforeTransform?.LocalRotation.eulerAngles.y ?? 0f;
                            var rotationZ = beforeTransform?.LocalRotation.eulerAngles.z ?? 0f;

                            if (bone.Position.XKeys.TryGetValue(keyTime, out var value)) { positionX = value; }
                            if (bone.Position.YKeys.TryGetValue(keyTime, out value)) { positionY = value; }
                            if (bone.Position.ZKeys.TryGetValue(keyTime, out value)) { positionZ = value; }
                            if (bone.Rotation.XKeys.TryGetValue(keyTime, out value)) { rotationX = value; }
                            if (bone.Rotation.YKeys.TryGetValue(keyTime, out value)) { rotationY = value; }
                            if (bone.Rotation.ZKeys.TryGetValue(keyTime, out value)) { rotationZ = value; }

                            BoneSprite boneSprite = null;
                            if (bone.Sprite.TryGetValue(keyTime, out var sprite) && sprite is PaintBoneSprite paint)
                            {
                                boneSprite = paint.ToBoneSprite(info.Palette);
                            }

                            if (bone.Reorder.TryGetValue(keyTime, out var reorder))
                            {
                                boneReorderTable[new BoneKeyword(bone.BoneName)] = reorder;
                            }

                            var transform = new SpritePoseBoneTransform(
                                boneSprite, info.MainColor,
                                new Vector3(positionX, positionY, positionZ) / RoguegardSettings.PixelsPerUnit,
                                Quaternion.Euler(rotationX, rotationY, rotationZ),
                                Vector3.one, false, false, false);
                            baseSpritePose.SetBoneTransform(transform, boneName);
                        }
                    }
                    else if (track is SubTimelineMotionGrapherTrack subTimelineTrack &&
                        subTimelineTrack.TryGetValue(keyTime, out var clip) &&
                        clip.TryGet<ISpriteMotion>(out var spriteMotion))
                    {
                        subSpriteMotions.Add(spriteMotion);
                    }
                }

                // ImmutableSymmetricalSpritePoseSource の方向変更に対応するため Body が存在しなければ追加する
                if (!baseSpritePose.BoneTransforms.Keys.Contains(BoneKeyword.Body) && subSpriteMotions.Count == 0)
                {
                    baseSpritePose.AddBoneTransform(new SpritePoseBoneTransform(
                        null, null, Vector3.zero, Quaternion.identity, Vector3.one, false, false, false), BoneKeyword.Body);
                }

                // 並べ替え情報を適用
                var reorders = boneReorderTable
                    .Where(x => x.Value != 0f)
                    .OrderBy(x => Mathf.Abs(x.Value))
                    .Select(x => new BoneReorder(x.Key, x.Value >= 0 ? BoneReorder.Type.Front : BoneReorder.Type.Rear));
                var boneOrder = new BoneOrder(System.Array.Empty<BoneBack>(), reorders);
                baseSpritePose.SetBoneOrder(boneOrder);

                baseSpritePose.SetImmutable();
                spritePoseSource = new ImmutableSymmetricalSpritePoseSource(baseSpritePose);

                recursionStack.Value.Pop();
            }

            public SpritePose GetSpritePose(SpriteDirection direction)
            {
                if (subSpriteMotions.Count == 0) return spritePoseSource.GetSpritePose(direction);

                resultSpritePose.Clear();
                resultBoneReorders.Clear();

                // 外部参照モーションを読み込む
                for (int i = 0; i < subSpriteMotions.Count; i++)
                {
                    var subSpriteMotion = subSpriteMotions[i];
                    var animationTime = Mathf.FloorToInt(AsynchronousKeyTime * 60f);
                    var transform = OchalikeSpriteTransform.Identity;
                    subSpriteMotion.ApplyTo(animationTime, direction, ref transform, out _);

                    var subSpritePose = transform.PoseSource.GetSpritePose(direction);
                    resultSpritePose.SetBoneTransforms(subSpritePose);
                    for (int j = 0; j < subSpritePose.BoneOrder.Reorders.Count; j++)
                    {
                        var reorder = subSpritePose.BoneOrder.Reorders[j];
                        resultBoneReorders.Add(reorder);
                    }
                }

                // 内部モーションを読み込む
                {
                    var originalPose = spritePoseSource.GetSpritePose(direction);
                    resultSpritePose.SetBoneTransforms(originalPose);
                    for (int j = 0; j < originalPose.BoneOrder.Reorders.Count; j++)
                    {
                        var reorder = originalPose.BoneOrder.Reorders[j];
                        resultBoneReorders.Add(reorder);
                    }
                    resultSpritePose.SetBack(originalPose.Back);
                }

                // 並べ替え情報を生成
                resultBoneReorderIndexTable.Clear();
                for (int i = 0; i < resultBoneReorders.Count; i++)
                {
                    var reorder = resultBoneReorders[i];
                    resultBoneReorderIndexTable[reorder.Name] = i; // ボーン名ごとに最後に上書きされたインデックスを取得
                }
                var reorders = resultBoneReorderIndexTable.Values.OrderBy(i => i).Select(i => resultBoneReorders[i]); // 最後に上書きされたインデックスの値のみ抽出

                // 並べ替え情報を適用
                var boneOrder = new BoneOrder(System.Array.Empty<BoneBack>(), reorders);
                resultSpritePose.SetBoneOrder(boneOrder);

                return resultSpritePose;
            }
        }
    }
}
