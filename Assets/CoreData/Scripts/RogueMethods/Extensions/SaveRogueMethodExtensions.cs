using System.Diagnostics.CodeAnalysis;

namespace Roguegard.Extensions
{
    [SuppressMessage("Style", "IDE0060")]
    public static class SaveRogueMethodExtensions
    {
        public static bool LocateSavePoint(
            this IActiveRogueMethodCaller method, RogueObj player, RogueObj tool, float activationDepth, ISavePointInfo savePointInfo, bool force = false)
        {
            if (LobbyMemberList.GetMemberInfo(player) == null) return false;

            var result = RogueMethodAspectState.Invoke(
                CategoryKw.DownStairs, savePointInfo.BeforeSave, tool, player, activationDepth, RogueMethodArgument.Identity);
            if (!result)
            {
                if (!force) throw new System.InvalidOperationException();

                // 失敗した場合は割り込みなしで再試行する
                result = savePointInfo.BeforeSave.Invoke(tool, player, activationDepth, RogueMethodArgument.Identity);
                if (!result) throw new System.InvalidOperationException();
            }
            return result;
        }

        public static bool LoadSavePoint(
            this IActiveRogueMethodCaller method, RogueObj player, float activationDepth, ISavePointInfo savePointInfo)
        {
            if (LobbyMemberList.GetMemberInfo(player) == null) return false;

            var result = RogueMethodAspectState.Invoke(
                DeviceKw.AfterLoad, savePointInfo.AfterLoad, null, player, activationDepth, RogueMethodArgument.Identity);
            if (!result)
            {
                // 失敗した場合は割り込みなしで再試行する
                result = savePointInfo.AfterLoad.Invoke(null, player, activationDepth, RogueMethodArgument.Identity);
                if (!result) throw new System.InvalidOperationException();
            }
            return result;
        }
    }
}
