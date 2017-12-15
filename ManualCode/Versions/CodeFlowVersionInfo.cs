using System;
using System.Collections.Generic;

namespace CodeFlow.Versions
{
    public class CodeFlowVersionInfo
    {
        private Version _version;
        private List<VersionChange> _changes;

        public CodeFlowVersionInfo()
        {
            Version = new Version(0, 0, 0);
            Changes = new List<VersionChange>();
        }
        public CodeFlowVersionInfo(Version version, List<VersionChange> changes)
        {
            Version = version;
            Changes = changes;
        }

        public Version Version { get => _version; set => _version = value; }
        public List<VersionChange> Changes { get => _changes; set => _changes = value; }
        public void AddChange(VersionChange change)
        {
            Changes.Add(change);
        }
        public void AddChange(String description)
        {
            Changes.Add(new VersionChange(description));
        }
    }
}
