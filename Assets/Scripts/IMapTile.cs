namespace Sokoban
{
    public interface IMapTile : IPositionable
    {
        MapTileType Type { get; }
    }
}
