using System;
using UnityEngine;

namespace Sokoban
{
    [Serializable]
    public class MapTileData
    {
        public Vector3 Position
        {
            get => _position.Position;
            set => _position.Position = value;
        }

        public MapTileType Type { get; }

        private IPosition _position;

        public MapTileData(IPosition position, MapTileType type)
        {
            _position = position;
            Type = type;
        }
    }

    public enum MapTileType
    {
        None, Floor, Wall, Goal, Box, Player,
    }
}
