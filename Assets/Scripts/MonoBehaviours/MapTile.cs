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

        private AudioSource _audio;
        private SpriteRenderer _renderer;

        #endregion

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            MapTileData.OnPlaySound -= OnPlaySound;
            MapTileData.OnChangeColour += OnChangeColour;
        }

        private void Awake()
        {
            _audio = GetComponent<AudioSource>();
            _renderer = GetComponent<SpriteRenderer>();
            MapTileData = new MapTileData(this, _type);
            MapTileData.OnPlaySound += OnPlaySound;
            MapTileData.OnChangeColour += OnChangeColour;
        }

        #endregion

        #region Methods

        #region Public

        public void Release()
        {
            ObjectPool?.Release(this);
        }

        #endregion

        #region Private

        private void OnPlaySound()
        {
            _audio.Stop();
            _audio.Play();
        }

        private void OnChangeColour(bool isReset)
        {
            _renderer.color = isReset ? Color.white : new Color(0.6f, 0.6f, 0.6f);
        }

        #endregion

        #endregion
    }
}
