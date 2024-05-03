using Sirenix.OdinInspector;
using UnityEngine;

namespace Sokoban
{
    [DefaultExecutionOrder(-50)]
    [ShowOdinSerializedPropertiesInInspector]
    public class MapManager : MonoBehaviour, ICreateMapTile
    {
        [SerializeField]
        private string _filePath;

        [SerializeField]
        private MapTile[] _tilePrefabs;

        [ShowInInspector, ReadOnly]
        public MapData MapData { get; private set; }

        private void Awake()
        {
            MapData = new MapData(this, _filePath);
        }

        private void Start()
        {
            MapData.Initialize();
        }

        public MapTileData CreateMapTile(string tileToken, Vector3 position)
        {
            var tilePrefab = tileToken switch
            {
                "=" => _tilePrefabs[0],
                "#" => _tilePrefabs[1],
                "." => _tilePrefabs[2],
                "$" => _tilePrefabs[3],
                "@" => _tilePrefabs[4],
                _ => null,
            };

            return tilePrefab is not null
                ? Instantiate(tilePrefab, position, Quaternion.identity, transform).MapTileData
                : null;
        }
    }
}
