using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sokoban
{
    [ShowOdinSerializedPropertiesInInspector]
    public class Map : MonoBehaviour
    {
        [SerializeField]
        private string _filename;

        [SerializeField]
        private Transform[] _tilePrefabs;

        [ShowInInspector]
        private MapData _mapData = new();

        private readonly Dictionary<(int, int), Transform> _mapTiles = new();

        private Transform _player;

        [ShowInInspector]
        private List<(int, int)> _goals = new();

        private Vector3 _moveDirection;

        private void Awake()
        {
            _mapData.LoadMap(_filename);
        }

        private void Start()
        {
            InitMap();
        }

        private void Update()
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
            position = _player.position;
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
            var playerPosition = _player.position;
            while (playerPosition != fromPosition)
            {
                var toPosition = fromPosition;
                toPosition -= _moveDirection;

                if (playerPosition != toPosition)
                {
                    GetMapTile(toPosition)?.Translate(_moveDirection);
                }

                HandleSwapMapTiles(toPosition, fromPosition);
                fromPosition = toPosition;
            }

            _player.Translate(_moveDirection);
        }

        private void InitMap()
        {
            for (var row = 0; row < _mapData.Rows; row++)
            {
                for (var col = 0; col < _mapData.Cols; col++)
                {
                    var tile = _mapData.GetMapTile(row, col);
                    if (tile.Equals(" "))
                    {
                        continue;
                    }

                    CreateMapTile(tile, row, col);
                    switch (tile)
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

        private void CreateMapTile(string tile, int row, int col)
        {
            var position = GetTilePosition(row, col);

            if (".".Equals(tile))
            {
                Instantiate(_tilePrefabs[2], position, Quaternion.identity, transform);
                return;
            }

            Instantiate(_tilePrefabs[0], position, Quaternion.identity, transform);

            var (tilePrefab, zPosition) = tile switch
            {
                "#" => (_tilePrefabs[1], -1f),
                "$" => (_tilePrefabs[3], -2f),
                "@" => (_tilePrefabs[4], -3f),
                _ => (null, 0f),
            };

            if (tilePrefab is null)
            {
                return;
            }

            position.z = zPosition;
            _mapTiles[(row, col)] = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        }

        private bool CheckGoals()
        {
            var goalCount = 0;

            foreach (var (row, col) in _goals)
            {
                var maptile = GetMapTile(new Vector3(col, row));
                if (!maptile || !maptile.tag.Equals("Box"))
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
            return GetMapTile(position).tag.Equals("Box");
        }

        private static Vector3 GetTilePosition(int row, int col)
        {
            return new Vector3(col * 1, row * -1);
        }

        private static (int row, int col) GetPositionToMapRowCol(Vector3 position)
        {
            return (Mathf.Abs(Mathf.FloorToInt(position.y)), Mathf.Abs(Mathf.FloorToInt(position.x)));
        }

        private Transform GetMapTile(Vector3 position)
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
