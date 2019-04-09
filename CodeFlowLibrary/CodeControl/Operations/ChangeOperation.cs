using System;
using CodeFlowLibrary.Genio;
using CodeFlowLibrary.CodeControl.Changes;
using CodeFlowLibrary.GenioCode;

namespace CodeFlowLibrary.CodeControl.Operations
{
    public class ChangeOperation : IOperation
    {
        private IChange operationData;
        private DateTime operationTime;
        private Profile operationProfile;

        public ChangeOperation()
        {

        }

        public ChangeOperation(IChange data, Profile operationProfile)
        {
            this.operationData = data ?? throw new ArgumentNullException(nameof(data));
            this.operationTime = DateTime.Now;
            this.operationProfile = operationProfile;
        }
        public IChange OperationChanges => operationData;
        public DateTime OperationTime => operationTime;
        public Profile OperationProfile => operationProfile;
        public string LocalFileName => operationData?.Mine?.LocalFileName ?? "";
        public string FullFileName => operationData?.Mine?.FullFileName ?? "";
        public string OperationType => "Modified";

        public bool Undo()
        {
            bool result;
            try
            {
                result = Execute(OperationChanges.Theirs);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(CodeFlowResources.Resources.UnableToExecuteOperation, e.Message));
            }
            return result;
        }
        public bool Redo()
        {
            return Execute();
        }
        public bool Execute()
        {
            bool result;
            try
            {
                result = Execute(OperationChanges.Merged);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(CodeFlowResources.Resources.UnableToExecuteOperation, e.Message));
            }

            return result;
        }
        private bool Execute(IManual man)
        {
            return man.Update(OperationProfile);
        }
    }
}

