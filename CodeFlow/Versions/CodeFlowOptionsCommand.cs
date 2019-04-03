using System;
using System.Reflection;
using CodeFlowLibrary.Versions;

namespace CodeFlow.Versions
{
    public class CodeFlowOptionsCommand : CodeFlowChange, ICodeFlowChangeCommand
    {
        private Func<bool> _command;

        public Func<bool> Command { get => _command; }

        public CodeFlowOptionsCommand(string description, Func<bool> command) : base(description)
        {
            _command = command;
        }

        public bool Execute()
        {
            if(Command != null)
                return Command.Invoke();
            return true;
        }
    }
}
