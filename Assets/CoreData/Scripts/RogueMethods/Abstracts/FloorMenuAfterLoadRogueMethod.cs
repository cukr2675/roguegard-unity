using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    /// <summary>
    /// ダンジョン名と階層を表示してそこに移動させる <see cref="IRogueMethod"/>
    /// </summary>
    public abstract class FloorMenuAfterLoadRogueMethod : BaseApplyRogueMethod, IModelsMenu, IModelsMenuChoice
    {
        private IModelsMenuChoice[] choices;

        protected FloorMenuAfterLoadRogueMethod()
        {
            choices = new IModelsMenuChoice[] { this };
        }

        /// <summary>
        /// <see cref="IApplyRogueMethod"/> のため  user 引数がプレイヤーとなる
        /// </summary>
        public sealed override bool Invoke(RogueObj self, RogueObj player, float activationDepth, in RogueMethodArgument arg)
        {
            if (player != RogueDevice.Primary.Player)
            {
                Debug.LogError($"{player} はプレイヤーキャラではありません。");
                return false;
            }

            // ダンジョン名と階層を表示してそこに移動させる。
            RogueDevice.Primary.AddMenu(this, player, null, RogueMethodArgument.Identity);
            return true;
        }

        public void OpenMenu(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg)
        {
            root.Get(DeviceKw.MenuFloor).OpenView(ChoicesModelsMenuItemController.Instance, choices, root, player, empty, arg);
        }

        public abstract string GetName(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg);

        public abstract void Activate(IModelsMenuRoot root, RogueObj player, RogueObj empty, in RogueMethodArgument arg);
    }
}
