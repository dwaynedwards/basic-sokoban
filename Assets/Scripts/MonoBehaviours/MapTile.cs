using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace Sokoban
{
    [ShowOdinSerializedPropertiesInInspector]
    public class MapTile : MonoBehaviour, IPosition
    {
        #region Fields and Properties

        #region Public

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public MapTileData MapTileData { get; private set; }

        public IObjectPool<MapTile> ObjectPool;

        #endregion

        #region Private

        [SerializeField]
        private MapTileType _type;

        #endregion

        #endregion

        #region Unity Methods

        private void Awake()
        {
            MapTileData = new MapTileData(this, _type);
        }

        #endregion

        #region Methods

        #region Public

        public void Release()
        {
            ObjectPool?.Release(this);
        }

        #endregion

        #endregion
    }
}
