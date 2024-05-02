using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sokoban
{
    [Serializable]
    public class Map
    {
        private List<(int, int)> _goals = new();

        private MapTile _player;

        private Vector3 _moveDirection;

        private MapData _mapData = new();
        private readonly Dictionary<(int, int), MapTile> _mapTiles = new();

        private IMap _map;

        public Map(IMap map)
        {
            _map = map;
        }

        public void Initialize()
        {
            _mapData.LoadMap(_map.FilePath);

            for (var row = 0; row < _mapData.Rows; row++)
            {
                for (var col = 0; col < _mapData.Cols; col++)
                {
                    var tileToken = _mapData.GetMapTile(row, col);
                    if (tileToken.Equals(" "))
                    {
                        continue;
                    }

                    CreateMapTile(tileToken, row, col);

                    switch (tileToken)
                    {
                        case "@":
                            _player = _mapTiles[(row, col)];
                            break;
                        case ".":
                            _goals.Add((row, col));
                            break;
                    }
                }
            }
        }

        private void CreateMapTile(string tileToken, int row, int col)
        {
            var position = GetTilePosition(row, col);

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

        public void Update()
        {
            HandleInput();
            HandleMove();
        }

        private void HandleInput()
        {
            _moveDirection = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _moveDirection = Vector3.up;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _moveDirection = Vector3.right;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _moveDirection = Vector3.down;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _moveDirection = Vector3.left;
            }
        }

        private void HandleMove()
        {
            if (_moveDirection == Vector3.zero)
            {
                return;
            }

            if (!TestMove(out var position))
            {
                return;
            }

            DoMove(position);

            if (CheckGoals())
            {
                Debug.Log("Success");
            }
        }

        private bool TestMove(out Vector3 position)
        {
            position = _player.Position;
            position += _moveDirection;
            var isFirstBox = true;

            while (true)
            {
                if (GetMapTile(position) is null)
                {
                    return true;
                }

                if (!CanMove(position) || !isFirstBox)
                {
                    return false;
                }

                position += _moveDirection;
                isFirstBox = false;
            }
        }

        private void DoMove(Vector3 fromPosition)
        {
            var playerPosition = _player.Position;
            while (playerPosition != fromPosition)
            {
                var toPosition = fromPosition;
                toPosition -= _moveDirection;

                if (playerPosition != toPosition)
                {
                    var tile = GetMapTile(toPosition);
                    if (tile is not null)
                    {
                        tile.Position += _moveDirection;
                    }
                }

                HandleSwapMapTiles(toPosition, fromPosition);
                fromPosition = toPosition;
            }

            _player.Position += _moveDirection;
        }

        private bool CheckGoals()
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

        private void HandleSwapMapTiles(Vector3 to, Vector3 from)
        {
            var hasTo = HasMapTile(to);
            var hasFrom = HasMapTile(from);

            if (hasTo && hasFrom)
            {
                SwapMapTiles(to, from);
            }
            else if (hasTo)
            {
                SwapMapTilesAndRemove(from, to);
            }
            else if (hasFrom)
            {
                SwapMapTilesAndRemove(to, from);
            }
        }

        private void SwapMapTiles(Vector3 to, Vector3 from)
        {
            var (toRow, toCol) = GetPositionToMapRowCol(to);
            var (fromRow, fromCol) = GetPositionToMapRowCol(from);
            (_mapTiles[(toRow, toCol)], _mapTiles[(fromRow, fromCol)]) =
                (_mapTiles[(fromRow, fromCol)], _mapTiles[(toRow, toCol)]);
        }

        private void SwapMapTilesAndRemove(Vector3 to, Vector3 from)
        {
            var (toRow, toCol) = GetPositionToMapRowCol(to);
            var (fromRow, fromCol) = GetPositionToMapRowCol(from);
            _mapTiles[(toRow, toCol)] = _mapTiles[(fromRow, fromCol)];
            _mapTiles.Remove((fromRow, fromCol));
        }

        private bool CanMove(Vector3 position)
        {
            return GetMapTile(position).Type == MapTileType.Box;
        }

        private static Vector3 GetTilePosition(int row, int col)
        {
            return new Vector3(col * 1, row * -1);
        }

        private static (int row, int col) GetPositionToMapRowCol(Vector3 position)
        {
            return (Mathf.Abs(Mathf.FloorToInt(position.y)), Mathf.Abs(Mathf.FloorToInt(position.x)));
        }

        private MapTile GetMapTile(Vector3 position)
        {
            var (row, col) = GetPositionToMapRowCol(position);
            return HasMapTile(position) ? _mapTiles[(row, col)] : null;
        }

        private bool HasMapTile(Vector3 position)
        {
            var (row, col) = GetPositionToMapRowCol(position);
            return _mapTiles.ContainsKey((row, col));
        }
    }
}
