using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sokoban
{
    [Serializable]
    public class InputHandler
    {
        #region Fields and Properties

        #region Public

        public event Action OnLevelComplete = delegate { };

        #endregion

        #region Private

        [ShowInInspector, ReadOnly,]
        private MapData _mapData;

        [ShowInInspector, ReadOnly,]
        private CommandInvoker _commandInvoker;

        #endregion

        #endregion

        #region Constructors

        public InputHandler(MapData mapData, CommandInvoker commandInvoker)
        {
            _mapData = mapData;
            _commandInvoker = commandInvoker;
        }

        #endregion

        #region Methods

        #region Public

        public void HandleReset()
        {
            _commandInvoker.Reset();
            _mapData.Reset();
            _mapData.Initialize();
        }

        public void HandleUndo()
        {
            _commandInvoker.UndoCommand();
        }

        public void HandleRedo()
        {
            _commandInvoker.RedoCommand();
        }

        public void HandleUp()
        {
            HandleMove(Vector3.up);
        }

        public void HandleLeft()
        {
            HandleMove(Vector3.left);
        }

        public void HandleDown()
        {
            HandleMove(Vector3.down);
        }

        public void HandleRight()
        {
            HandleMove(Vector3.right);
        }

        #endregion

        #region Private

        private void HandleMove(Vector3 moveDirection)
        {
            if (moveDirection == Vector3.zero)
            {
                return;
            }

            if (!TestMove(out var position, moveDirection))
            {
                return;
            }

            var command = new MoveCommand(_mapData, position, _mapData.Player.Position, moveDirection);
            _commandInvoker.ExecuteCommand(command);

            if (_mapData.IsLevelComplete)
            {
                OnLevelComplete();
            }
        }

        private void DoMove(Vector3 fromPosition, Vector3 moveDirection)
        {
            var playerPosition = _mapData.Player.Position;
            while (playerPosition != fromPosition)
            {
                var toPosition = fromPosition;
                toPosition -= moveDirection;

                if (playerPosition != toPosition)
                {
                    var tile = _mapData.GetMapTile(toPosition);
                    if (tile is not null)
                    {
                        tile.Position += moveDirection;
                    }
                }

                _mapData.HandleSwapMapTiles(toPosition, fromPosition);
                fromPosition = toPosition;
            }

            _mapData.Player.Position += moveDirection;
        }

        private bool TestMove(out Vector3 position, Vector3 moveDirection)
        {
            position = _mapData.Player.Position;
            position += moveDirection;
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

                position += moveDirection;
                isFirstBox = false;
            }
        }

        #endregion

        #endregion
    }
}
