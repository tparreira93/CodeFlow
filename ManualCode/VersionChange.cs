using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow
{
    public class VersionChange
    {
        private string description;
        private DefaultCommand defaultCommand;

        public VersionChange(string description, DefaultCommand command)
        {
            Description = description;
            Command = command;
        }

        public VersionChange(string description)
        {
            Description = description;
            Command = new DefaultCommand();
        }

        public string Description { get => description; set => description = value; }
        public DefaultCommand Command { get => defaultCommand; set => defaultCommand = value; }
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
