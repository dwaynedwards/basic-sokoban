using System;
using UnityEngine;

namespace Sokoban
{
    [Serializable]
    public class MapTile
    {
        public Vector3 Position
        {
            get => _mapTile.Position;
            set => _mapTile.Position = value;
        }

        public MapTileType Type { get => _mapTile.Type; }

        private IMapTile _mapTile;

        public MapTile(IMapTile mapTile)
        {
            _mapTile = mapTile;
        }
    }

    public enum MapTileType
    {
        None, Floor, Wall, Goal, Box, Player,
    }
}
