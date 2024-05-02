using UnityEngine;

namespace Sokoban
{
    public interface IMap
    {
        public string FilePath { get; }
        public MapTile CreateMapTile(string tileToken, Vector3 position);
    }
}
