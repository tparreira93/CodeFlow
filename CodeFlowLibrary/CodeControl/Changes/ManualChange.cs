using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeFlowLibrary.GenioCode;
using CodeFlowLibrary.CodeControl.Rules;
using CodeFlowLibrary.CodeControl.Operations;

namespace CodeFlowLibrary.CodeControl.Changes
{
    public abstract class ManualChange : IChange
    {
        IManual local;
        IManual bd;
        IManual merged;
        bool isMerged = false;
        CodeRule rule;

        /*
        * Merged defaults to mine
        */
        public ManualChange(IManual mine, IManual theirs)
        {
            Mine = mine ?? throw new ArgumentNullException(nameof(mine));
            Theirs = theirs;
            Merged = Mine;
            rule = null;
        }
        public virtual IChange Merge()
        {
            IChange change = this;
            Merged = Manual.Merge(Theirs, Merged);

            if (!String.IsNullOrWhiteSpace(Mine.Code) && String.IsNullOrWhiteSpace(Merged.Code))
                change = new CodeEmpty(Merged, Theirs);

            else if (String.IsNullOrWhiteSpace(Mine.Code) && !String.IsNullOrWhiteSpace(Merged.Code))
                change = new CodeChange(Merged, Theirs);

            change.IsMerged = true;

            return change;
        }
        public virtual void Compare()
        {
            Manual.Compare(Theirs, Merged);
        }
        public virtual bool HasDifference()
        {
            return !Merged.Code.Equals(Theirs.Code);
        }

        public abstract IOperation GetOperation();
        public abstract string GetDescription();

        public IManual Mine { get => local; set => local = value; }
        public IManual Theirs { get => bd; set => bd = value; }
        public bool IsMerged { get => isMerged; set => isMerged = value; }
        public IManual Merged { get => merged; set => merged = value; }
        CodeRule IChange.FlagedRule { get => rule; set => rule = value; }
    }
}
