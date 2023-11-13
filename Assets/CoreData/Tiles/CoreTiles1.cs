using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CoreTiles1 : ScriptableLoader
    {
        private static CoreTiles1 instance;

        [SerializeField] private RogueTileInfoData _floor;
        public static IRogueTileInfo Floor => instance._floor;

        [SerializeField] private RogueTileInfoData _grass;
        public static IRogueTileInfo Grass => instance._grass;

        [SerializeField] private RogueTileInfoData _paintTrap;
        public static IRogueTileInfo PaintTrap => instance._paintTrap;

        [SerializeField] private RogueTileInfoData _pool;
        public static IRogueTileInfo Pool => instance._pool;

        [SerializeField] private RogueTileInfoData _roomWall;
        public static IRogueTileInfo RoomWall => instance._roomWall;

        [SerializeField] private RogueTileInfoData _thronsTrap;
        public static IRogueTileInfo ThronsTrap => instance._thronsTrap;

        [SerializeField] private RogueTileInfoData _wall;
        public static IRogueTileInfo Wall => instance._wall;

        [SerializeField] private RogueTileInfoData _woodenPlank;
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
