using Sirenix.OdinInspector;
using UnityEngine;

namespace Sokoban
{
    [ShowOdinSerializedPropertiesInInspector]
    public class InputManager : MonoBehaviour
    {
        #region Fields and Properties

        #region Private

        [SerializeField]
        private MapManager _mapManager;

        [SerializeField]
        private InputButton _resetButton;

        [SerializeField]
        private InputButton _redoButton;

        [SerializeField]
        private InputButton _upButton;

        [SerializeField]
        private InputButton _undoButton;

        [SerializeField]
        private InputButton _leftButton;

        [SerializeField]
        private InputButton _downButton;

        [SerializeField]
        private InputButton _rightButton;

        [ShowInInspector, ReadOnly]
        private InputHandler _input;

        #endregion

        #endregion

        #region Unity Methods

        private void OnDestroy()
        {
            _resetButton.OnButtonPressed -= _input.HandleReset;
            _redoButton.OnButtonPressed -= _input.HandleRedo;
            _upButton.OnButtonPressed -= _input.HandleUp;
            _undoButton.OnButtonPressed -= _input.HandleUndo;
            _leftButton.OnButtonPressed -= _input.HandleLeft;
            _downButton.OnButtonPressed -= _input.HandleDown;
            _rightButton.OnButtonPressed -= _input.HandleRight;
        }

        private void Awake()
        {
            _input = new InputHandler(_mapManager.MapData, new CommandInvoker());

            _resetButton.OnButtonPressed += _input.HandleReset;
            _redoButton.OnButtonPressed += _input.HandleRedo;
            _upButton.OnButtonPressed += _input.HandleUp;
            _undoButton.OnButtonPressed += _input.HandleUndo;
            _leftButton.OnButtonPressed += _input.HandleLeft;
            _downButton.OnButtonPressed += _input.HandleDown;
            _rightButton.OnButtonPressed += _input.HandleRight;
        }

        #endregion
    }
}
