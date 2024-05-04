using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sokoban
{
    [Serializable]
    public class MoveCommand : ICommand
    {
        #region Fields and Properties

        #region Private

        [ShowInInspector, ReadOnly,]
        private MapData _mapData;

        [ShowInInspector, ReadOnly,]
        private Vector3 _to;

        [ShowInInspector, ReadOnly,]
        private Vector3 _from;

        [ShowInInspector, ReadOnly,]
        private Vector3 _moveDirection;

        #endregion

        #endregion

        #region Constructors

        public MoveCommand(MapData mapData, Vector3 to, Vector3 from, Vector3 moveDirection)
        {
            _mapData = mapData;
            _to = to;
            _from = from;
            _moveDirection = moveDirection;
        }

        #endregion

        #region Methods

        #region Public

        public void Execute()
        {
            Move(_to, _from, _moveDirection);
        }

        public void Undo()
        {
            Move(_from, _to, -_moveDirection);
        }

        #endregion

        #region Private

        private void Move(Vector3 to, Vector3 from, Vector3 moveDirection)
        {
            while (!to.Equals(from))
            {
                var next = to;
                next -= moveDirection;

                var tile = _mapData.GetMapTile(next);
                if (tile is not null)
                {
                    tile.Position += moveDirection;
                    tile.PlaySound();
                }

                _mapData.HandleSwapMapTiles(to, next);
                to = next;
            }
        }

        #endregion

        #endregion
    }
}
