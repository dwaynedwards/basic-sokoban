using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sokoban
{
    [Serializable]
    public class InputHandler
    {
        [ShowInInspector, ReadOnly,]
        private MapData _mapData;

        [ShowInInspector, ReadOnly,]
        private Vector3 _moveDirection;

        [ShowInInspector, ReadOnly,]
        private CommandInvoker _commandInvoker;

        public InputHandler(MapData mapData, CommandInvoker commandInvoker)
        {
            _mapData = mapData;
            _commandInvoker = commandInvoker;
        }

        public InputHandler() { }

        public void HandleMoveInput()
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

        public void HandleUndoRedoInput()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _commandInvoker.UndoCommand();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _commandInvoker.RedoCommand();
            }
        }

        public void HandleMove()
        {
            if (_moveDirection == Vector3.zero)
            {
                return;
            }

            if (!TestMove(out var position))
            {
                return;
            }

            var command = new MoveCommand(_mapData,  position, _mapData.Player.Position, _moveDirection);
            _commandInvoker.ExecuteCommand(command);

            if (_mapData.CheckGoals())
            {
                Debug.Log("Success");
            }
        }

        private void DoMove(Vector3 fromPosition)
        {
            var playerPosition = _mapData.Player.Position;
            while (playerPosition != fromPosition)
            {
                var toPosition = fromPosition;
                toPosition -= _moveDirection;

                if (playerPosition != toPosition)
                {
                    var tile = _mapData.GetMapTile(toPosition);
                    if (tile is not null)
                    {
                        tile.Position += _moveDirection;
                    }
                }

                _mapData.HandleSwapMapTiles(toPosition, fromPosition);
                fromPosition = toPosition;
            }

            _mapData.Player.Position += _moveDirection;
        }

        private bool TestMove(out Vector3 position)
        {
            position = _mapData.Player.Position;
            position += _moveDirection;
            var isFirstBox = true;

            while (true)
            {
                if (_mapData.GetMapTile(position) is null)
                {
                    return true;
                }

                if (!_mapData.CanMove(position) || !isFirstBox)
                {
                    return false;
                }

                position += _moveDirection;
                isFirstBox = false;
            }
        }
    }
}
