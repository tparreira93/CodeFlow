using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlow
{
    public class CodeFlowVersionInfo
    {
        private Version version;
        private List<VersionChange> changes;

        public CodeFlowVersionInfo()
        {
            Version = new Version("0.0.0");
            Changes = new List<VersionChange>();
        }
        public CodeFlowVersionInfo(Version version, List<VersionChange> changes)
        {
            Version = version;
            Changes = changes;
        }

        public Version Version { get => version; set => version = value; }
        public List<VersionChange> Changes { get => changes; set => changes = value; }
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
