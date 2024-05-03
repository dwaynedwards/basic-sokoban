using UnityEngine;

namespace Sokoban
{
    public interface ICreateMapTile
    {
        public MapTileData CreateMapTile(string tileToken, Vector3 position);
    }
}
