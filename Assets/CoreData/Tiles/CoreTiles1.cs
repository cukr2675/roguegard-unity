using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class CoreTiles1 : ScriptableLoader
    {
        private static CoreTiles1 instance;

        [SerializeField] private StandardRogueTileInfoAsset _floor;
        public static IRogueTileInfo Floor => instance._floor;

        [SerializeField] private StandardRogueTileInfoAsset _grass;
        public static IRogueTileInfo Grass => instance._grass;

        [SerializeField] private StandardRogueTileInfoAsset _paintTrap;
        public static IRogueTileInfo PaintTrap => instance._paintTrap;

        [SerializeField] private StandardRogueTileInfoAsset _pool;
        public static IRogueTileInfo Pool => instance._pool;

        [SerializeField] private StandardRogueTileInfoAsset _roomWall;
        public static IRogueTileInfo RoomWall => instance._roomWall;

        [SerializeField] private StandardRogueTileInfoAsset _thronsTrap;
        public static IRogueTileInfo ThronsTrap => instance._thronsTrap;

        [SerializeField] private StandardRogueTileInfoAsset _wall;
        public static IRogueTileInfo Wall => instance._wall;

        [SerializeField] private StandardRogueTileInfoAsset _woodenPlank;
        public static IRogueTileInfo WoodenPlank => instance._woodenPlank;

        public override IEnumerator LoadAsync()
        {
            instance = this;
            yield break;
        }

        public override void TestLoad()
        {
#if UNITY_EDITOR
            instance = this;
#else
            throw new RogueException("This method is Editor Only.");
#endif
        }
    }
}
