using UnityEngine;

namespace Sokoban
{
    public class MapView : MonoBehaviour, IMap
    {
        [SerializeField]
        private string _filePath;

        public string FilePath => _filePath;

        [SerializeField]
        private Transform[] _tilePrefabs;

        private Map _map;

        private void Awake()
        {
            _map = new Map(this);
        }

        private void Start()
        {
            _map.Initialize();
        }

        private void Update()
        {
            _map.Update();
        }

        public MapTile CreateMapTile(string tileToken, Vector3 position)
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

            if (tilePrefab is null)
            {
                return null;
            }

            var mapTile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
            return mapTile.GetComponent<MapTileView>().MapTile;
        }
    }
}
