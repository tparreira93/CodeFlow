using System;
using System.Reflection;

namespace CodeFlow.Versions
{
    public class VersionChange
    {
        private string _description;
        private Func<OptionsPageGrid, bool> _defaultCommand;

        public VersionChange(string description, Func<OptionsPageGrid, bool> command)
        {
            Description = description;
            Command = command;
        }

        public VersionChange(string description)
        {
            Description = description;
        }
        public bool Execute(OptionsPageGrid options)
        {
            if(Command != null)
                return Command.Invoke(options);
            return true;
        }

        public string Description { get => _description; set => _description = value; }
        public Func<OptionsPageGrid, bool> Command { get => _defaultCommand; set => _defaultCommand = value; }
    }

    public class DefaultCommand
    {
        private string propertyName;
        private string propertyValue;

        public DefaultCommand()
        {
            PropertyName = "";
            PropertyValue = "";
        }

        public DefaultCommand(string propertyName, string propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }

        public string PropertyName { get => propertyName; set => propertyName = value; }
        public string PropertyValue { get => propertyValue; set => propertyValue = value; }

        public void Execute(OptionsPageGrid options)
        {
            if (String.IsNullOrEmpty(PropertyName)
                || String.IsNullOrEmpty(PropertyValue))
                return;

            PropertyInfo propertyInfo = options.GetType().GetProperty(PropertyName);
            propertyInfo?.SetValue(options, Convert.ChangeType(PropertyValue, propertyInfo?.PropertyType), null);
        }
    }
}
