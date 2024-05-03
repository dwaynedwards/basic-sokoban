using System;
using UnityEngine;

namespace Sokoban
{
    public enum MapTileType
    {
        None, Floor, Wall, Goal, Box, Player,
    }

    [Serializable]
    public class MapTileData
    {
        #region Fields and Properties

        #region Public

        public Vector3 Position
        {
            get => _position.Position;
            set => _position.Position = value;
        }

        public MapTileType Type { get; }

        #endregion

        #region Private

        private IPosition _position;

        #endregion

        #endregion

        #region Constructors

        public MapTileData(IPosition position, MapTileType type)
        {
            _position = position;
            Type = type;
        }

        #endregion
    }
}
