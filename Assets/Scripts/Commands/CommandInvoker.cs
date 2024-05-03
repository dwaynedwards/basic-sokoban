using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Sokoban
{
    [Serializable]
    public class CommandInvoker
    {
        [ShowInInspector]
        private Stack<ICommand> _undoStack = new();

        [ShowInInspector]
        private Stack<ICommand> _redoStack = new();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);

            _redoStack.Clear();
        }

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
    }
}
