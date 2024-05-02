using UnityEngine;

namespace Sokoban
{
    public class MapTileView : MonoBehaviour, IMapTile
    {
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        [SerializeField]
        private MapTileType _type;

        public MapTileType Type => _type;

        public MapTile MapTile { get; private set; }

        private void Awake()
        {
            MapTile = new MapTile(this);
        }
    }
}
