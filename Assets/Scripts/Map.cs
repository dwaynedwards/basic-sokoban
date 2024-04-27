using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LBAPG
{
    [ShowOdinSerializedPropertiesInInspector]
    public class Map : MonoBehaviour
    {
        [SerializeField]
        public string Filename;

        [SerializeField]
        public Transform[] Tiles;

        [ShowInInspector]
        private MapData _mapData = new();

        private Grid _grid;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
            _mapData.LoadMap(Filename);
        }

        private void Start()
        {
            for (var row = 0; row < _mapData.Rows; row++)
            {
                for (var col = 0; col < _mapData.Cols; col++)
                {
                    var tile = _mapData.GetMapTile(row, col);
                    CreateMapTile(tile, row, col);
                }
            }
        }

        private void CreateMapTile(string tile, int row, int col)
        {
            var nonGoalTiles = new[] { "#", "$", "@", " ", };
            var position = _grid.GetCellCenterWorld(new Vector3Int(row, col));

            if (Array.IndexOf(nonGoalTiles, tile) >= 0)
            {
                Instantiate(Tiles[0], position, Quaternion.identity, transform);
            }
            else
            {
                Instantiate(Tiles[2], position, Quaternion.identity, transform);
                return;
            }

            var mapTile = tile switch
            {
                "#" => Tiles[1],
                "$" => Tiles[3],
                "@" => Tiles[4],
                _ => null,
            };

            if (mapTile is null)
            {
                return;
            }

            position += Vector3.back;
            Instantiate(mapTile, position, Quaternion.identity, transform);
        }
    }
}
