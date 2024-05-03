using UnityEngine;

namespace Sokoban
{
    public interface IMap
    {
        public MapTileData CreateMapTile(string tileToken, Vector3 position);
        public void Release();
    }
}
