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

        private Transform[,] _mapTiles;

        private Transform _player;

        [ShowInInspector]
        private List<(int, int)> _goals = new();

        private Vector3 _moveDirection;

        private void Awake()
        {
            _mapData.LoadMap(_filename);
            _mapTiles = new Transform[_mapData.Rows, _mapData.Cols];
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

                SwapMapTilesInMap(toPosition, fromPosition);
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
                    CreateMapTile(tile, row, col);
                    switch (tile)
                    {
                        case "@":
                            _player = _mapTiles[row, col];
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
            _mapTiles[row, col] = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        }

        private bool CheckGoals()
        {
            var goalCount = 0;

            foreach (var (row, col) in _goals)
            {
                var maptile = _mapTiles[row, col];
                if (!maptile)
                {
                    continue;
                }

                if (maptile.tag.Equals("Box"))
                {
                    goalCount++;
                }
            }

            return goalCount == _goals.Count;
        }

        private void SwapMapTilesInMap(Vector3 to, Vector3 from)
        {
            var (toRow, toCol) = GetMapRowCol(to);
            var (fromRow, fromCol) = GetMapRowCol(from);
            (_mapTiles[toRow, toCol], _mapTiles[fromRow, fromCol]) =
                (_mapTiles[fromRow, fromCol], _mapTiles[toRow, toCol]);
        }

        private bool CanMove(Vector3 position)
        {
            return GetMapTile(position).tag.Equals("Box");
        }

        private static Vector3 GetTilePosition(int row, int col)
        {
            return new Vector3(col * 1, row * -1);
        }

        private static (int row, int col) GetMapRowCol(Vector3 position)
        {
            var row = Mathf.Abs(Mathf.FloorToInt(position.y));
            var col = Mathf.Abs(Mathf.FloorToInt(position.x));
            return (row, col);
        }

        private Transform GetMapTile(Vector3 position)
        {
            var (row, col) = GetMapRowCol(position);
            return _mapTiles[row, col];
        }
    }
}
