using System.Collections.Generic;
using System.Linq;

namespace CodeFlowLibrary.Versions
{
    public class CodeFlowUpdater
    {
        private List<CodeFlowVersion> _versions;

        public CodeFlowUpdater(List<CodeFlowVersion> versions)
        {
            Versions = versions;
        }


        public Version Update(Version startingVersion)
        {
            Version maxVersion = startingVersion;
            foreach (CodeFlowVersion item in Versions.OrderBy(x => x.Version))
            {
                foreach (ICodeFlowChange change in item.Changes)
                {
                    if (startingVersion.IsBefore(item.Version))
                    {
                        if(change is ICodeFlowChangeCommand command)
                            command.Execute();
                        maxVersion = item.Version;
                    }
                }
            }
            return maxVersion;
        }

        public List<CodeFlowVersion> Versions { get => _versions; set => _versions = value; }
    }
}
