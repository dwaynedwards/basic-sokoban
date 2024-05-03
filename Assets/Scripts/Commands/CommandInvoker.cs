using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Sokoban
{
    [Serializable]
    public class CommandInvoker
    {
        #region Fields and Properties

        #region Private

        [ShowInInspector]
        private Stack<ICommand> _undoStack = new();

        [ShowInInspector]
        private Stack<ICommand> _redoStack = new();

        #endregion

        #endregion

        #region Constructors

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        #endregion

        #region Methods

        #region Public

        public void UndoCommand()
        {
            if (_undoStack.Count <= 0)
            {
                return;
            }

            var activeCommand = _undoStack.Pop();
            _redoStack.Push(activeCommand);
            activeCommand.Undo();
        }

        public void RedoCommand()
        {
            if (_redoStack.Count <= 0)
            {
                return;
            }

            var activeCommand = _redoStack.Pop();
            _undoStack.Push(activeCommand);
            activeCommand.Execute();
        }

        public void Reset()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        #endregion

        #endregion
    }
}
