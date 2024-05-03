using Sirenix.OdinInspector;
using UnityEngine;

namespace Sokoban
{
    [ShowOdinSerializedPropertiesInInspector]
    public class MapTile : MonoBehaviour, IPosition
    {
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        [SerializeField]
        private MapTileType _type;

        public MapTileData MapTileData { get; private set; }

        private void Awake()
        {
            MapTileData = new MapTileData(this, _type);
        }
    }
}
