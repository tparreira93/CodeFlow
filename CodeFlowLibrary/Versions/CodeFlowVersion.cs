using CodeFlowLibrary.Versions;
using System;
using System.Collections.Generic;

namespace CodeFlowLibrary.Versions
{
    public class CodeFlowVersion
    {
        private Version _version;
        private List<ICodeFlowChange> _changes;

        public CodeFlowVersion()
        {
            _version = new Version(0, 0, 0);
            _changes = new List<ICodeFlowChange>();
        }
        public CodeFlowVersion(Version version, List<ICodeFlowChange> changes)
        {
            _version = version;
            _changes = changes;
        }
        public CodeFlowVersion(Version version, string change)
        {
            _version = version;
            _changes = new List<ICodeFlowChange>() { new CodeFlowChange(change) };
        }

        public Version Version { get => _version; set => _version = value; }
        public List<ICodeFlowChange> Changes { get => _changes; set => _changes = value;  }
        public void AddChange(ICodeFlowChange change)
        {
            Changes.Add(change);
        }
        public void AddChange(String description)
        {
            Changes.Add(new CodeFlowChange(description));
        }
    }
}
