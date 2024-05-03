using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sokoban
{
    [Serializable]
    public class MapData
    {
        #region Fields and Properties

        #region Public

        public MapTileData Player { get; private set; }

        public bool IsLevelComplete
        {
            get
            {
                var goalCount = 0;

                foreach (var (row, col) in _goals)
                {
                    var mapTile = GetMapTile(new Vector3(col, row));
                    if (mapTile is null || mapTile.Type != MapTileType.Box)
                    {
                        continue;
                    }

                    goalCount++;
                }

                return _goals.Count.Equals(goalCount);
            }
        }

        #endregion

        #region Private

        private MapFileData _mapFileData = new();
        private readonly Dictionary<(int, int), MapTileData> _mapTiles = new();
        private List<(int, int)> _goals = new();

        private IMap _map;
        private string _filePath;

        #endregion

        #endregion

        #region Constructors

        public MapData(IMap map, string filePath)
        {
            _map = map;
            _filePath = filePath;
        }

        #endregion

        #region Methods

        #region Public

        public void Initialize()
        {
            _mapFileData.LoadMap(_filePath);

            for (var row = 0; row < _mapFileData.Rows; row++)
            {
                for (var col = 0; col < _mapFileData.Cols; col++)
                {
                    var tileToken = _mapFileData.GetMapTile(row, col);
                    if (tileToken.Equals(" "))
                    {
                        continue;
                    }

                    CreateMapTile(tileToken, row, col);

                    switch (tileToken)
                    {
                        case "@":
                            Player = _mapTiles[(row, col)];
                            break;
                        case ".":
                            _goals.Add((row, col));
                            break;
                    }
                }
            }
        }

        public void Reset()
        {
            _map.Release();
            _mapTiles.Clear();
            _mapFileData.Reset();
        }

        public void HandleSwapMapTiles(Vector3 to, Vector3 from)
        {
            var hasTo = HasMapTile(to);
            var hasFrom = HasMapTile(from);

            if (hasTo && hasFrom)
            {
                SwapMapTiles(to, from);
            }
            else if (hasTo)
            {
                SwapMapTilesAndRemoveFrom(from, to);
            }
            else if (hasFrom)
            {
                SwapMapTilesAndRemoveFrom(to, from);
            }
        }

        public bool CanMove(Vector3 position)
        {
            return GetMapTile(position).Type == MapTileType.Box;
        }

        public MapTileData GetMapTile(Vector3 position)
        {
            var (row, col) = GetRowColFromPosition(position);
            return HasMapTile(position) ? _mapTiles[(row, col)] : null;
        }

        #endregion

        #region Private

        private static Vector3 GetTilePositionFromRowCol(int row, int col)
        {
            return new Vector3(col * 1, row * -1);
        }

        private static (int row, int col) GetRowColFromPosition(Vector3 position)
        {
            return (Mathf.Abs(Mathf.FloorToInt(position.y)), Mathf.Abs(Mathf.FloorToInt(position.x)));
        }

        private void CreateMapTile(string tileToken, int row, int col)
        {
            var position = GetTilePositionFromRowCol(row, col);

            if (".".Equals(tileToken))
            {
                _map.CreateMapTile(tileToken, position);
                return;
            }

            _map.CreateMapTile("=", position);

            var zPosition = tileToken switch
            {
                "#" => -1f,
                "$" => -2f,
                "@" => -3f,
                _ => 0f,
            };

            if (zPosition.Equals(0))
            {
                return;
            }

            position.z = zPosition;
            _mapTiles[(row, col)] = _map.CreateMapTile(tileToken, position);
        }

        private void SwapMapTiles(Vector3 to, Vector3 from)
        {
            var (toRow, toCol) = GetRowColFromPosition(to);
            var (fromRow, fromCol) = GetRowColFromPosition(from);
            (_mapTiles[(toRow, toCol)], _mapTiles[(fromRow, fromCol)]) =
                (_mapTiles[(fromRow, fromCol)], _mapTiles[(toRow, toCol)]);
        }

        private void SwapMapTilesAndRemoveFrom(Vector3 to, Vector3 from)
        {
            var (toRow, toCol) = GetRowColFromPosition(to);
            var (fromRow, fromCol) = GetRowColFromPosition(from);
            _mapTiles[(toRow, toCol)] = _mapTiles[(fromRow, fromCol)];
            _mapTiles.Remove((fromRow, fromCol));
        }

        private bool HasMapTile(Vector3 position)
        {
            var (row, col) = GetRowColFromPosition(position);
            return _mapTiles.ContainsKey((row, col));
        }

        #endregion

        #endregion
    }
}
