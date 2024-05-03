using Sirenix.OdinInspector;
using UnityEngine;

namespace Sokoban
{
    [ShowOdinSerializedPropertiesInInspector]
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private MapManager _mapManager;

        [ShowInInspector, ReadOnly]
        private InputHandler _input;

        private void Awake()
        {
            _input = new InputHandler(_mapManager.MapData, new CommandInvoker());
        }

        public void Update()
        {
            _input.HandleMoveInput();
            _input.HandleUndoRedoInput();
            _input.HandleMove();
        }
    }
}
