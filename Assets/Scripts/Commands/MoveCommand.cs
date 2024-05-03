using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Sokoban
{
    [Serializable]
    public class MoveCommand : ICommand
    {
        [ShowInInspector, ReadOnly,]
        private MapData _mapData;

        [ShowInInspector, ReadOnly,]
        private Vector3 _to;

        [ShowInInspector, ReadOnly,]
        private Vector3 _from;

        [ShowInInspector, ReadOnly,]
        private Vector3 _moveDirection;

        public MoveCommand(MapData mapData, Vector3 to, Vector3 from, Vector3 moveDirection)
        {
            _mapData = mapData;
            _to = to;
            _from = from;
            _moveDirection = moveDirection;
        }

        public void Execute()
        {
            Move(_to, _from, _moveDirection);
        }

        public void Undo()
        {
            Move(_from, _to, -_moveDirection);
        }

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
                }

                _mapData.HandleSwapMapTiles(to, next);
                to = next;
            }
        }
    }
}
