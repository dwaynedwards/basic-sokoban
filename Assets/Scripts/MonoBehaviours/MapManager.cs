using Sirenix.OdinInspector;
using Sokoban.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace Sokoban
{
    [DefaultExecutionOrder(-50)]
    [ShowOdinSerializedPropertiesInInspector]
    public class MapManager : MonoBehaviour, IMap
    {
        #region Fields and Properties

        #region Public

        [ShowInInspector, ReadOnly]
        public MapData MapData { get; private set; }

        #endregion

        #region Private

        [SerializeField]
        private string _filePath;

        [SerializeField]
        private MapTile[] _tilePrefabs;

        private IObjectPool<MapTile> _floorObjectPool;
        private IObjectPool<MapTile> _wallObjectPool;
        private IObjectPool<MapTile> _goalObjectPool;
        private IObjectPool<MapTile> _boxObjectPool;
        private IObjectPool<MapTile> _playerObjectPool;

        private Camera _camera;

        #endregion

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            MapData.OnInitialize -= OnInitialize;
        }

        private void Awake()
        {
            MapData = new MapData(this, _filePath);
            MapData.OnInitialize += OnInitialize;

            _floorObjectPool = new ObjectPool<MapTile>(OnCreateFloor, OnGet, OnRelease, OnDelete, true, 150, 200);
            _wallObjectPool = new ObjectPool<MapTile>(OnCreateWall, OnGet, OnRelease, OnDelete, true, 50, 100);
            _goalObjectPool = new ObjectPool<MapTile>(OnCreateGoal, OnGet, OnRelease, OnDelete, true, 5, 10);
            _boxObjectPool = new ObjectPool<MapTile>(OnCreateBox, OnGet, OnRelease, OnDelete, true, 5, 10);
            _playerObjectPool = new ObjectPool<MapTile>(OnCreatePlayer, OnGet, OnRelease, OnDelete, true, 1, 1);

            _camera = Camera.main;
        }

        private void Start()
        {
            MapData.Initialize();
        }

        #endregion

        #region Methods

        #region Public

        public MapTileData CreateMapTile(string tileToken, Vector3 position)
        {
            var mapTile = tileToken switch
            {
                "=" => _floorObjectPool.Get(),
                "#" => _wallObjectPool.Get(),
                "." => _goalObjectPool.Get(),
                "$" => _boxObjectPool.Get(),
                "@" => _playerObjectPool.Get(),
                _ => null,
            };

            if (mapTile is null)
            {
                return null;
            }

            mapTile.Position = position;
            return mapTile.MapTileData;
        }

        public void Release()
        {
            transform.ForEveryChild(child => child.GetComponent<MapTile>().Release());
        }

        #endregion

        #region Private

        private void OnInitialize()
        {
            var halfRows = MapData.Rows / 2f;
            _camera.orthographicSize = MapData.Rows + 1;
            _camera.transform.position = new Vector3(halfRows, -halfRows, -10f);
        }

        private MapTile OnCreateFloor()
        {
            var mapTile = Instantiate(_tilePrefabs[0], Vector3.zero, Quaternion.identity, transform);
            mapTile.ObjectPool = _floorObjectPool;
            return mapTile;
        }

        private MapTile OnCreateWall()
        {
            var mapTile = Instantiate(_tilePrefabs[1], Vector3.zero, Quaternion.identity, transform);
            mapTile.ObjectPool = _wallObjectPool;
            return mapTile;
        }

        private MapTile OnCreateGoal()
        {
            var mapTile = Instantiate(_tilePrefabs[2], Vector3.zero, Quaternion.identity, transform);
            mapTile.ObjectPool = _goalObjectPool;
            return mapTile;
        }

        private MapTile OnCreateBox()
        {
            var mapTile = Instantiate(_tilePrefabs[3], Vector3.zero, Quaternion.identity, transform);
            mapTile.ObjectPool = _boxObjectPool;
            return mapTile;
        }

        private MapTile OnCreatePlayer()
        {
            var mapTile = Instantiate(_tilePrefabs[4], Vector3.zero, Quaternion.identity, transform);
            mapTile.ObjectPool = _playerObjectPool;
            return mapTile;
        }

        private void OnGet(MapTile tile)
        {
            tile.gameObject.SetActive(true);
            tile.transform.localPosition = Vector3.zero;
        }

        private void OnRelease(MapTile tile)
        {
            tile.gameObject.SetActive(false);
            tile.transform.localPosition = Vector3.zero;
        }

        private void OnDelete(MapTile tile)
        {
            Destroy(tile.gameObject);
        }

        #endregion

        #endregion
    }
}
